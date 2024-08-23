using LiteNetLib;
using MessagePack;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Server.Managers
{
    public static class Network
    {
        private static EventBasedNetListener s_listener;
        private static NetManager s_server;
        private static CancellationTokenSource s_cancelTokenSource;
        private static string[] s_userGUIDs;

        private const int s_serverId = -1;

        static Network()
        {
            s_listener = new EventBasedNetListener();
            s_server = new NetManager(s_listener)
            {
                AutoRecycle = true,
                ChannelsCount = 2
            };
            s_cancelTokenSource = new CancellationTokenSource();
            s_userGUIDs = new string[Program.Settings.MaxPlayers];

            s_listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            s_listener.PeerConnectedEvent += PeerConnectedEventHandler;
            s_listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            s_listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            s_listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private static void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            byte[] buffer = new byte[request.Data.AvailableBytes];
            request.Data.GetBytes(buffer, buffer.Length);
            AuthData authData = MessagePackSerializer.Deserialize<AuthData>(buffer);
            if (s_server.ConnectedPeersCount < Program.Settings.MaxPlayers
                && authData.Version == Program.Version)
            {
                Log.Debug($"Accepted connection from {request.RemoteEndPoint}");
                request.Accept();
            }
            else
            {
                Log.Debug($"Rejected connection from {request.RemoteEndPoint}");
                request.Reject();
            }
        }

        private static void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Debug($"Connected peer with ID{peer.Id}");
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug($"Disconnected peer with ID{peer.Id}");
            s_userGUIDs[peer.Id] = "";
            IMultiMessage leaveMessage = new LeaveMessage(s_serverId, peer.Id);
            Send(leaveMessage);
            Log.Information($"Disconnected from Server: {peer.Id}");
        }

        private static void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte[] buffer = new byte[reader.AvailableBytes];
            reader.GetBytes(buffer, buffer.Length);
            NetworkReceiveEventProcess(peer, buffer, channel, deliveryMethod);
        }

        private static void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Error($"A network error has occurred {socketError}");
        }

        public static void Start(int port)
        {
            Thread pollEventsThread = new Thread(() => PollEventsThread(s_cancelTokenSource.Token));
            pollEventsThread.Start();
            s_server.Start(port);
            Log.Information("Server started.");
        }

        public static void Stop()
        {
            s_cancelTokenSource.Cancel();
            s_server.Stop();
            Log.Information("Server stopped.");
        }

        private static void PollEventsThread(CancellationToken token)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, s_server.LocalPort);
            listener.Start();
            while (!token.IsCancellationRequested)
            {
                s_server.PollEvents();
                if (!listener.Pending())
                    continue;
                TcpClient tcpClient = listener.AcceptTcpClient();
                Task.Run(() => DataRequestHandler(tcpClient), token);
            }
            listener.Stop();
        }

        private static void DataRequestHandler(TcpClient client)
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

                int id = Array.IndexOf(s_userGUIDs, Encoding.UTF8.GetString(guid));
                if (id == -1)
                    throw new Exception("User not found.");
                NetPeer peer = s_server.GetPeerById(id);
                NetworkReceiveEventProcess(peer, data, channel[0], DeliveryMethod.ReliableOrdered);
            }
        }

        private static void NetworkReceiveEventProcess(NetPeer peer, byte[] buffer, byte channel, DeliveryMethod deliveryMethod)
        {
            switch (channel)
            {
                case 0x00:
                    IMultiMessage multiMessage = MessagePackSerializer.Deserialize<IMultiMessage>(buffer);
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
                    IUniMessage uniMessage = MessagePackSerializer.Deserialize<IUniMessage>(buffer);
                    if (uniMessage.FromID != peer.Id)
                        return;
                    switch (uniMessage)
                    {
                        case RequestServerInfoMessage message:
                            if (message.ToID != s_serverId)
                                return;
                            s_userGUIDs[peer.Id] = message.UserGUID;
                            LocalWorldData localWorldData = World.GetLocalWorldData(message.UserGUID);
                            IUniMessage infoMessage = new ServerInfoMessage(
                                s_serverId,
                                peer.Id,
                                Program.Settings.MaxPlayers,
                                localWorldData);
                            Send(infoMessage);
                            IMultiMessage joinMessage = new JoinMessage(
                                s_serverId,
                                peer.Id);
                            Send(joinMessage, peer);
                            Log.Information($"Connected to Client with ID{peer.Id}");
                            break;
                    }
                    break;
            }
        }

        public static void Send(IMultiMessage message)
        {
            byte[] data = MessagePackSerializer.Serialize(message);
            if (message.IsLarge)
            {
                byte[] length = BitConverter.GetBytes(data.Length);
                byte[] channel = new byte[1] { 0x00 };
                byte[] buffer = new byte[length.Length + channel.Length + data.Length];

                int offset = 0;
                Array.Copy(length, 0, buffer, offset, length.Length);
                offset += length.Length;
                Array.Copy(channel, 0, buffer, offset, channel.Length);
                offset += channel.Length;
                Array.Copy(data, 0, buffer, offset, data.Length);

                s_server.ConnectedPeerList.ForEach(p =>
                {
                    using TcpClient client = new TcpClient();
                    client.Connect(p.Address, p.Port);
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                });
                return;
            }
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    s_server.SendToAll(data, 0x00, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    s_server.SendToAll(data, 0x00, DeliveryMethod.Sequenced);
                    break;
            }
        }

        public static void Send(IMultiMessage message, NetPeer excludePeer)
        {
            byte[] data = MessagePackSerializer.Serialize(message);
            if (message.IsLarge)
            {
                byte[] length = BitConverter.GetBytes(data.Length);
                byte[] channel = new byte[1] { 0x00 };
                byte[] buffer = new byte[length.Length + channel.Length + data.Length];

                int offset = 0;
                Array.Copy(length, 0, buffer, offset, length.Length);
                offset += length.Length;
                Array.Copy(channel, 0, buffer, offset, channel.Length);
                offset += channel.Length;
                Array.Copy(data, 0, buffer, offset, data.Length);

                s_server.ConnectedPeerList.ForEach(p =>
                {
                    if (p != excludePeer)
                    {
                        using TcpClient client = new TcpClient();
                        client.Connect(p.Address, p.Port);
                        using (NetworkStream stream = client.GetStream())
                        {
                            stream.Write(buffer, 0, buffer.Length);
                        }
                    }
                });
                return;
            }
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    s_server.SendToAll(data, 0x00, DeliveryMethod.ReliableOrdered, excludePeer);
                    break;
                case ProtocolType.Udp:
                    s_server.SendToAll(data, 0x00, DeliveryMethod.Sequenced, excludePeer);
                    break;
            }
        }

        public static void Send(IUniMessage message)
        {
            byte[] data = MessagePackSerializer.Serialize(message);
            if (message.IsLarge)
            {
                byte[] length = BitConverter.GetBytes(data.Length);
                byte[] channel = new byte[1] { 0x01 };
                byte[] buffer = new byte[length.Length + channel.Length + data.Length];

                int offset = 0;
                Array.Copy(length, 0, buffer, offset, length.Length);
                offset += length.Length;
                Array.Copy(channel, 0, buffer, offset, channel.Length);
                offset += channel.Length;
                Array.Copy(data, 0, buffer, offset, data.Length);

                NetPeer peer = s_server.GetPeerById(message.ToID);
                using TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(peer.Address, peer.Port));
                using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                return;
            }
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    s_server.GetPeerById(message.ToID).Send(data, 0x01, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    s_server.GetPeerById(message.ToID).Send(data, 0x01, DeliveryMethod.Sequenced);
                    break;
            }
        }
    }
}