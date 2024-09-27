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
        EventBasedNetListener _listener = new();
        CancellationTokenSource _cancellationTokenSource = new();

        List<string> _userGUIDs;
        public int MaxPlayersCount => Program.Settings.MaxPlayers; // Can be chaged to variable later 

        NetManager _server;

        private const int s_serverId = -1;

        public Network()
        {
            _userGUIDs = new(MaxPlayersCount);

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
            if (_server.ConnectedPeersCount >= MaxPlayersCount)
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

            Log.Debug($"Accepted connection from {request.RemoteEndPoint}");
            request.Accept();
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Debug($"Connected peer with ID {peer.Id}");
        }

        private void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug($"Disconnected peer with ID {peer.Id} {disconnectInfo.Reason}");
            _userGUIDs[peer.Id] = "";
            IGameDataMessage leaveMessage = new LeaveMessage(peer.Id);
            Send(leaveMessage);
            Log.Information($"Disconnected from Server: {peer.Id}");
        }

        private void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte[] buffer = new byte[reader.AvailableBytes];
            reader.GetBytes(buffer, buffer.Length);
            NetworkReceiveEventProcess(peer, buffer, channel, deliveryMethod);
        }

        private void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Error($"A network error has occurred {socketError}");
        }

        public void Start(int port)
        {
            Thread pollEventsThread = new Thread(() => PollEventsThread(_cancellationTokenSource.Token));
            pollEventsThread.Start();
            _server.Start(port);
            Log.Information("Server started.");
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _server.Stop();
            Log.Information("Server stopped.");
        }

        private void PollEventsThread(CancellationToken token)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _server.LocalPort);
            listener.Start();
            while (!token.IsCancellationRequested)
            {
                _server.PollEvents();
                if (!listener.Pending())
                    continue;
                TcpClient tcpClient = listener.AcceptTcpClient();
                Task.Run(() => DataRequestHandler(tcpClient), token);
            }
            listener.Stop();
        }

        private void DataRequestHandler(TcpClient client)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] length = new byte[4];
                stream.Read(length, 0, length.Length);
                byte[] channel = new byte[1];
                stream.Read(channel, 0, channel.Length);
                byte[] guid = new byte[36];
                stream.Read(guid, 0, guid.Length);
                byte[] data = new byte[BitConverter.ToInt32(length)];
                byte[] buffer = new byte[1024];
                int readDataLength = 0;
                int i;
                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    Array.Copy(buffer, 0, data, readDataLength, i);
                    readDataLength += i;
                }

                if (readDataLength != data.Length)
                    throw new Exception("Data length mismatch.");

                int id = _userGUIDs.IndexOf(Encoding.UTF8.GetString(guid));
                if (id == -1)
                    throw new Exception("User not found.");
                NetPeer peer = _server.GetPeerById(id);
                NetworkReceiveEventProcess(peer, data, channel[0], DeliveryMethod.ReliableOrdered);
            }
        }

        private void NetworkReceiveEventProcess(NetPeer peer, byte[] buffer, byte channel, DeliveryMethod deliveryMethod)
        {
            switch (channel)
            {
                case 0x00:
                    IGameDataMessage multiMessage = MessagePackSerializer.Deserialize<IGameDataMessage>(buffer);
                    if (multiMessage.FromID != peer.Id)
                        return;
                    switch (multiMessage)
                    {
                        case PlayerPositionMessage:
                            Send(multiMessage, peer);
                            break;
                    };
                    break;
                case 0x01:
                    IServerDataMessage uniMessage = MessagePackSerializer.Deserialize<IServerDataMessage>(buffer);
                    if (uniMessage.FromID != peer.Id)
                        return;
                    switch (uniMessage)
                    {
                        case RequestServerInfoMessage message:
                            if (message.ToID != s_serverId)
                                return;
                            _userGUIDs[peer.Id] = message.UserGUID;
                            LocalWorldData localWorldData = World.GetLocalWorldData(message.UserGUID);

                            Send(new ServerInfoMessage(
                                peer.Id,
                                MaxPlayersCount,
                                localWorldData,
                                Program.Settings.DayNightCycle // TODO: As local variable?
                            ));

                            Send(new JoinMessage(peer.Id), peer);

                            Log.Information($"Connected to Client with ID{peer.Id}");
                            break;
                    }
                    break;
            }
        }


        public void Send(IGameDataMessage message)
        {
            byte[] data = MessagePackSerializer.Serialize(message);

            _server.SendToAll(data, 0x00, message.Protocol switch
            {
                ProtocolType.Tcp => DeliveryMethod.ReliableOrdered,
                ProtocolType.Udp => DeliveryMethod.Sequenced,
                _ => throw new NotSupportedException()
            });
        }

        public void Send(IGameDataMessage message, NetPeer excludePeer)
        {
            byte[] data = MessagePackSerializer.Serialize(message);

            _server.SendToAll(data, 0x01, message.Protocol switch
            {
                ProtocolType.Tcp => DeliveryMethod.ReliableOrdered,
                ProtocolType.Udp => DeliveryMethod.Sequenced,
                _ => throw new NotSupportedException()
            });
        }


        public void Send(IServerDataMessage message)
        {
            byte[] data = MessagePackSerializer.Serialize(message);

            _server.GetPeerById(message.ToID).Send(data, 0x01, message.Protocol switch
            {
                ProtocolType.Tcp => DeliveryMethod.ReliableOrdered,
                ProtocolType.Udp => DeliveryMethod.Sequenced,
                _ => throw new NotSupportedException()
            });
        }
    }
}