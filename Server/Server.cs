﻿using LiteNetLib;
using MessagePack;
using YuchiGames.POM.Shared.DataObjects;
using System.Net;
using System.Net.Sockets;
using YuchiGames.POM.Shared;
using System.Diagnostics.CodeAnalysis;
using YuchiGames.POM.Shared.Utils;

namespace YuchiGames.POM.Server
{
    public class Server
    {
        // Connected users: 
        public class ConnectedUser
        {
            public ConnectedUser() { }
            public string GUID = "";
            public int UID = NextID();

            public SVector3 LastPlayerPosition = new();


            static int s_idCounter;
            static int NextID() => s_idCounter++;
        }
        Dictionary<NetPeer, ConnectedUser> _authorizedUsers = new();
        List<NetPeer> _waitingForAuthUsers = new();

        bool IsAuthorized(NetPeer peer) => _authorizedUsers.ContainsKey(peer);


        // Internal:
        EventBasedNetListener _listener = new();
        NetManager _netServer;


        // Settings:
        public ServerSettings Settings;
        public ILogger Log = new EmptyLogger();

        public WorldData WorldData = new();

        public Server(ServerConfig serverSettings)
        {
            Settings = serverSettings;

            _netServer = new NetManager(_listener)
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
            if (_authorizedUsers.Count + _waitingForAuthUsers.Count >= Settings.MaxPlayers)
            {
                errorMessage = "Too many connections";
                return false;
            }
            if (authData.Version != Settings.Version)
            {
                errorMessage = $"Versions unmatched. Server: {Settings.Version}. Client: {authData.Version}";
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

        private void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError) =>
            Log.Error($"A network error has occurred:\n{socketError}");

        public void StartSynced(int port)
        {
            _netServer.Start(port);
            Log.Info($"Server started at port {port}.");
            while (_netServer.IsRunning) // FIXME: Cancelation token or something
            {
                _netServer.PollEvents();
                Task.Delay(15).Wait(); // FIXME: SCARY! .Wait is a bad thing 
            }
        }

        public bool Stopped { get; private set; } = false;

        public void Stop()
        {
            if (Stopped) return;

            Stopped = true;
            _netServer.Stop();
            Log.Info("Server stopped.");
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

        // TODO: Maybe??? just send for everyone that chunk is loaded, even if it still loading by another player
        Dictionary<SVector2Int, (NetPeer loader, HashSet<NetPeer> waiters)> _awaitsForChunkLoad = new();
        Dictionary<SVector2Int, HashSet<NetPeer>> _loadedChunks = new();

        /// <summary>
        /// Process Server Data Messages
        /// </summary>
        private void ReceiveServerDataMessage(NetPeer peer, IServerDataMessage serverDataMessage)
        {
            switch (serverDataMessage)
            {
                // Authorizing
                case RequestServerInfoMessage message:
                    ConnectedUser newUser = new() { GUID = message.UserGUID };
                    _authorizedUsers[peer] = newUser;
                    _waitingForAuthUsers.Remove(peer);

                    var localWorldData = LocalWorldData.From(WorldData, message.UserGUID);

                    Send(new ServerInfoMessage()
                    {
                        UID = newUser.UID,
                        IsDayNightCycle = Settings.DayNightCycle,
                        WorldData = localWorldData,
                        MaxPlayers = Settings.MaxPlayers,
                        WorldUpdateDistance = Settings.WorldUpdateDistance,
                        WorldQuickUpdateDistance = Settings.WorldQuickUpdateDistance
                    }, peer);

                    SendToAllExcluded(new JoinMessage(peer.Id), peer);

                    Log.Info($"Peer with ID {peer.Id} was authorized.");
                    break;

                case RequestNewChunkDataMessage message:
                    // FIXME: EDGE CASE! Server request to load a chunk, but player leaves.

                    // If chunk already loaded by another player
                    if (_loadedChunks.TryGetValue(message.ChunkPos, out var loaders))
                    {
                        Send(new ChunkUnloadMessage() { Pos = message.ChunkPos }, peer);
                        loaders.Add(peer);
                        break;
                    }

                    // If someone at this moment loading that chunk
                    if (_awaitsForChunkLoad.TryGetValue(message.ChunkPos, out var loaderWaiter))
                    {
                        loaderWaiter.waiters.Add(peer);
                        break;
                    }

                    // If chunk is saved
                    if (WorldData.ChunksData.TryGetValue(message.ChunkPos, out var chunk))
                    {
                        Send(new SavedChunkDataMessage()
                        {
                            Chunk = chunk,
                            Pos = message.ChunkPos,
                        }, peer);

                        break;
                    }

                    // If chunk need to be generated
                    _awaitsForChunkLoad[message.ChunkPos] = (peer, new());
                    Send(new RequestNewChunkDataMessage() { ChunkPos = message.ChunkPos }, peer);
                    break;

                case RequestedChunkDataMessage message:
                    // When player finnaly generates a chunk after request
                    _loadedChunks[message.Pos] = new();
                    _awaitsForChunkLoad[message.Pos].waiters.ForEach(waiter =>
                    {
                        Send(new ChunkUnloadMessage() { Pos = message.Pos }, waiter);
                        _loadedChunks[message.Pos].Add(waiter);
                    });
                    break;

                case ChunkUnloadMessage message:

                    // If player unload any loaded chunk:
                    if (_loadedChunks.TryGetValue(message.Pos, out var loadedBy))
                    {
                        loadedBy.Remove(peer);
                        if (loadedBy.Count <= 0)
                            _loadedChunks.Remove(message.Pos);
                    }

                    // If player unload still loading chunk:
                    if (_awaitsForChunkLoad.TryGetValue(message.Pos, out var loader))
                        loader.waiters.Remove(peer);

                    break;

                // Merge with MarkChunkLoadState? Send only by request?
                case SavedChunkDataMessage message:
                    if (!_loadedChunks.ContainsKey(message.Pos))
                        WorldData.ChunksData[message.Pos] = message.Chunk;
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
                    _authorizedUsers[peer].LastPlayerPosition = playerPosition.PlayerPos.Head.Position;
                    SendToAllExcluded(new PlayerPositionUpdateMessage(playerPosition.PlayerPos, _authorizedUsers[peer].UID), peer);
                    break;

                case GroupUpdateMessage message:
                    SendToAllExcluded(message, otherPeer =>
                    {
                        var sourceUserData = _authorizedUsers[peer];
                        var userData = _authorizedUsers[otherPeer];

                        var distBetween = SVector3.Distance(sourceUserData.LastPlayerPosition, userData.LastPlayerPosition) < Settings.WorldUpdateDistance && peer != otherPeer;

                        return distBetween;
                    });
                    break;

                case GroupDestroyedMessage message:
                    SendToAllExcluded(message, peer);
                    break;

                case GroupSetHostMessage message:
                    Log.Debug($"Transfering ownership of {message.GroupID} to {message.NewHostID}");
                    SendToAll(message);
                    break;

                case GroupQuickUpdateMessage message:
                    foreach (var user in _authorizedUsers)
                    {
                        if (user.Key == peer || SVector3.Distance(message.Position, user.Value.LastPlayerPosition) > Settings.WorldQuickUpdateDistance)
                            continue;

                        Send(message, user.Key);
                    }
                    break;
            };
        }

        /// <summary>
        /// Sends message to everyon, except peer with id <paramref name="excludedPeerId"/>
        /// </summary>
        public void SendToAllExcluded(IDataMessage message, int excludedPeerId) =>
            SendToAllExcluded(message, _netServer.GetPeerById(excludedPeerId));
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
            Send(message, _netServer.GetPeerById(peerId));
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