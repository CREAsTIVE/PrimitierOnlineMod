using LiteNetLib;
using System.Net;
using MessagePack;
using UnityEngine;
using LiteNetLib.Utils;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;
using System.Text;

namespace YuchiGames.POM.Client.Managers
{
    public static class Network
    {
        private static int s_id;
        public static int ID
        {
            get => s_id;
        }
        private static int s_ping;
        public static int Ping
        {
            get => s_ping;
        }
        public static bool IsRunning
        {
            get => s_client.IsRunning;
        }
        private static bool s_isConnected;
        public static bool IsConnected
        {
            get => s_isConnected;
        }
        private static ServerInfoMessage s_serverInfo;
        public static ServerInfoMessage ServerInfo
        {
            get => s_serverInfo;
        }

        private static EventBasedNetListener s_listener;
        private static NetManager s_client;
        private static CancellationTokenSource s_cancelTokenSource;

        private const int s_serverId = -1;

        static Network()
        {
            s_id = -1;
            s_ping = -1;
            s_isConnected = false;
            s_serverInfo = new ServerInfoMessage(
                s_id,
                s_serverId,
                0,
                new byte[0][],
                new LocalWorldData());

            s_listener = new EventBasedNetListener();
            s_client = new NetManager(s_listener)
            {
                AutoRecycle = true,
                ChannelsCount = 2
            };
            s_cancelTokenSource = new CancellationTokenSource();

            s_listener.PeerConnectedEvent += PeerConnectedEventHandler;
            s_listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            s_listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            s_listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private static void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Debug($"Connected peer with ID{peer.RemoteId}");
            s_id = peer.RemoteId;
            IUniMessage message = new RequestServerInfoMessage(
                s_id,
                s_serverId,
                Program.UserGUID);
            Send(message);
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug("Disconnected peer");
            s_isConnected = false;
            s_id = -1;
            s_ping = -1;
            Log.Information($"Disconnected from Server");
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

        public static void Connect(string ipAddress, int port)
        {
            Log.Information($"Connecting to Server...");
            s_client.Start();
            AuthData authData = new AuthData(Program.Version);
            byte[] buffer = MessagePackSerializer.Serialize(authData);
            NetDataWriter data = new NetDataWriter();
            data.Put(buffer);
            s_client.Connect(ipAddress, port, data);
            Thread tcpThread = new Thread(() => ReceiveDataRequestsThread(s_cancelTokenSource.Token));
            tcpThread.Start();
        }

        public static void Disconnect()
        {
            Log.Information($"Disconnecting from Server...");
            s_cancelTokenSource.Cancel();
            s_client.Stop();
            Log.Information($"Disconnected from Server");
        }

        public static void OnUpdate()
        {
            s_client.PollEvents();
            if (s_client.FirstPeer != null)
                s_ping = s_client.FirstPeer.Ping;
        }

        private static void ReceiveDataRequestsThread(CancellationToken token)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(Program.Settings.IP), s_client.LocalPort);
            listener.Start();
            while (!token.IsCancellationRequested)
            {
                if (!listener.Pending())
                    continue;
                TcpClient tcpClient = listener.AcceptTcpClient();
                // Task.Run(() => DataRequestHandler(tcpClient));
                DataRequestHandler(tcpClient);
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

                NetworkReceiveEventProcess(s_client.FirstPeer, data, channel[0], DeliveryMethod.ReliableOrdered);
            }
        }

        private static void NetworkReceiveEventProcess(NetPeer peer, byte[] buffer, byte channel, DeliveryMethod deliveryMethod)
        {
            switch (channel)
            {
                case 0x00:
                    switch (MessagePackSerializer.Deserialize<IMultiMessage>(buffer))
                    {
                        case JoinMessage message:
                            Log.Information($"Joined player with ID{message.JoinID}");
                            break;
                        case LeaveMessage message:
                            Log.Information($"Player left with ID{message.LeaveID}");
                            Avatar.DestroyAvatar(message.LeaveID);
                            break;
                        case UploadVRMMessage message:
                            Avatar.LoadAvatar(message.FromID, message.VRMData);
                            break;
                    }
                    break;
                case 0x01:
                    switch (MessagePackSerializer.Deserialize<IUniMessage>(buffer))
                    {
                        case ServerInfoMessage message:
                            s_serverInfo = message;
                            World.Load(message.WorldData);
                            Avatar.LoadAvatars(message.AvatarData);
                            s_isConnected = true;
                            Log.Information($"Connected to Server with ID{s_id}");
                            IMultiMessage multiMessage = new UploadVRMMessage(
                                s_id,
                                Avatar.GetAvatar());
                            Send(multiMessage);
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
                byte[] guid = Encoding.UTF8.GetBytes(Program.UserGUID);
                byte[] buffer = new byte[length.Length + channel.Length + guid.Length + data.Length];

                int offset = 0;
                Array.Copy(length, 0, buffer, offset, length.Length);
                offset += length.Length;
                Array.Copy(channel, 0, buffer, offset, channel.Length);
                offset += channel.Length;
                Array.Copy(guid, 0, buffer, offset, guid.Length);
                offset += guid.Length;
                Array.Copy(data, 0, buffer, offset, data.Length);

                NetPeer peer = s_client.FirstPeer;
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
                    s_client.FirstPeer.Send(data, 0x00, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    s_client.FirstPeer.Send(data, 0x00, DeliveryMethod.Sequenced);
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
                byte[] guid = Encoding.UTF8.GetBytes(Program.UserGUID);
                byte[] buffer = new byte[length.Length + channel.Length + guid.Length + data.Length];

                int offset = 0;
                Array.Copy(length, 0, buffer, offset, length.Length);
                offset += length.Length;
                Array.Copy(channel, 0, buffer, offset, channel.Length);
                offset += channel.Length;
                Array.Copy(guid, 0, buffer, offset, guid.Length);
                offset += guid.Length;
                Array.Copy(data, 0, buffer, offset, data.Length);

                NetPeer peer = s_client.FirstPeer;
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
                    s_client.FirstPeer.Send(data, 0x01, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    s_client.FirstPeer.Send(data, 0x01, DeliveryMethod.Sequenced);
                    break;
            }
        }
    }
}