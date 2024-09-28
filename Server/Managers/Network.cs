using LiteNetLib;
using MessagePack;
using Serilog;
using YuchiGames.POM.Shared.DataObjects;
using System.Net;
using System.Net.Sockets;
using System.Text;
using YuchiGames.POM.Shared;
using System.Diagnostics.CodeAnalysis;

namespace YuchiGames.POM.Server.Managers
{
    public class Network
    {
        public struct ConnectedUser
        {
            public ConnectedUser() { }
            public string GUID = "";
            public int ID = NextID();


            static int s_idCounter;
            static int NextID() => s_idCounter++;
        }

        EventBasedNetListener _listener = new();

        Dictionary<NetPeer, ConnectedUser> _authorizedUsers = new();
        bool IsAuthorized(NetPeer peer) => _authorizedUsers.ContainsKey(peer);

        List<NetPeer> _waitingForAuthUsers = new();

        Task? _pollEventsTask = null;

        public int MaxPlayersCount => ServerSettings.MaxPlayers; // Can be chaged to variable later 

        public ServerSettings ServerSettings;

        NetManager _server;

        public Network(ServerSettings serverSettings)
        {
            this.ServerSettings = serverSettings;

            _server = new NetManager(_listener)
            {
                AutoRecycle = true,
                ChannelsCount = 2
            };

            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
            _listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            _listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            _listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        bool ValidateConnection(AuthData authData, [NotNullWhen(false)] out string? errorMessage)
        {
            if (_authorizedUsers.Count + _waitingForAuthUsers.Count >= MaxPlayersCount)
            {
                errorMessage = "Too many connections";
                return false;
            }
            if (authData.Version != Program.Version)
            {
                errorMessage = $"Versions unmatched. Server: {Program.Version}. Client: {authData.Version}";
                return false;
            }
            errorMessage = null;
            return true;
        }

        private void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            byte[] buffer = new byte[request.Data.AvailableBytes];
            request.Data.GetBytes(buffer, buffer.Length);
            AuthData authData = MessagePackSerializer.Deserialize<AuthData>(buffer);

            if (!ValidateConnection(authData, out var error))
            {
                Log.Debug($"Rejected connection from {request.RemoteEndPoint}, reason: {error}.");
                request.Reject();
                return;
            }

            Log.Debug($"Accepted connection from {request.RemoteEndPoint}.");
            request.Accept();
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            _waitingForAuthUsers.Add(peer);
            Log.Debug($"Connected peer with ID {peer.Id}.");
        }

        private void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _authorizedUsers.Remove(peer);
            _waitingForAuthUsers.Remove(peer);

            Log.Debug($"Disconnected peer with ID {peer.Id} {disconnectInfo.Reason}.");
            SendToAll(new LeaveMessage(peer.Id));
        }

        private void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte[] buffer = new byte[reader.AvailableBytes];
            reader.GetBytes(buffer, buffer.Length);
            NetworkReceiveEventProcess(peer, buffer, channel, deliveryMethod);
        }

        private void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Error($"A network error has occurred {socketError}.");
        }

        public void Start(int port)
        {
            _server.Start(port);
            _pollEventsTask = Task.Run(() =>
            {
                while (_server.IsRunning) // FIXME: Cancelation token or something
                {
                    _server.PollEvents();
                    Task.Delay(15).Wait(); // FIXME: SCARY! .Wait is a bad thing 
                }
            });
            Log.Information($"Server started at port {port}.");
        }

        public void Wait() =>
            _pollEventsTask?.Wait(); // FIXME: SCARY! Stop using wait!

        public void Stop()
        {
            _server.Stop();
            Log.Information("Server stopped.");
        }

        private void NetworkReceiveEventProcess(NetPeer peer, byte[] buffer, byte channel, DeliveryMethod deliveryMethod)
        {
            switch (channel)
            {
                case 0x00:
                    ReceiveGameDataMessage(peer, MessagePackSerializer.Deserialize<IGameDataMessage>(buffer));
                    break;
                case 0x01:
                    ReceiveServerDataMessage(peer, MessagePackSerializer.Deserialize<IServerDataMessage>(buffer));
                    break;
            }
        }

        /// <summary>
        /// Process Server Data Messages
        /// </summary>
        private void ReceiveServerDataMessage(NetPeer peer, IServerDataMessage serverDataMessage)
        {
            switch (serverDataMessage)
            {
                // Authorizing
                case RequestServerInfoMessage message:
                    _authorizedUsers[peer] = new() { GUID = message.UserGUID };
                    _waitingForAuthUsers.Remove(peer);

                    LocalWorldData localWorldData = World.GetLocalWorldData(message.UserGUID);

                    Send(new ServerInfoMessage(
                        MaxPlayersCount,
                        localWorldData,
                        ServerSettings.DayNightCycle
                    ), peer);

                    SendToAllExcluded(new JoinMessage(peer.Id), peer);

                    Log.Information($"Peer with ID {peer.Id} was authorized.");
                    break;
            }
        }
        /// <summary>
        /// Process Game Data Messages
        /// </summary>
        private void ReceiveGameDataMessage(NetPeer peer, IGameDataMessage gameDataMessage)
        {
            switch (gameDataMessage)
            {
                case PlayerPositionMessage playerPosition:
                    SendToAllExcluded(new PlayerPositionUpdateMessage(playerPosition.PlayerPos, _authorizedUsers[peer].ID), peer);
                    break;
            };
        }
        /// <summary>
        /// Sends message to everyon, except peer with id <paramref name="excludedPeerId"/>
        /// </summary>
        public void SendToAllExcluded(IDataMessage message, int excludedPeerId) =>
            SendToAllExcluded(message, _server.GetPeerById(excludedPeerId));
        /// <summary>
        /// Sends message to everyone, except <paramref name="excludedPeer"/>
        /// </summary>
        public void SendToAllExcluded(IDataMessage message, NetPeer excludedPeer) =>
            SendToAllExcluded(message, peer => peer != excludedPeer);
        /// <summary>
        /// Sends message to everyone, who satisfy <paramref name="filter"/>
        /// </summary>
        public void SendToAllExcluded(IDataMessage message, Func<NetPeer, bool> filter) =>
            _authorizedUsers.Keys.Where(filter).ForEach(peer => Send(message, peer));

        public void SendToAll(IDataMessage message) =>
            _authorizedUsers.Keys.ForEach(peer => Send(message, peer));

        // used _connectedUsers.Keys so data will be sended only to authorized users

        public void Send(IDataMessage message, int peerId) =>
            Send(message, _server.GetPeerById(peerId));
        public void Send(IDataMessage message, NetPeer peer)
        {
            byte[] data = IDataMessage.Serialize(message);

            peer.Send(data, message.Channel, message.Protocol switch
            {
                ProtocolType.Tcp => DeliveryMethod.ReliableOrdered,
                ProtocolType.Udp => DeliveryMethod.Sequenced,
                _ => throw new NotSupportedException()
            });
        }
        
    }
}