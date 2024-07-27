using LiteNetLib;
using System.Net;
using MessagePack;
using LiteNetLib.Utils;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Managers
{
    public static class Network
    {
        private static int s_id = -1;
        public static int ID
        {
            get => s_id;
        }
        private static int s_ping = -1;
        public static int Ping
        {
            get => s_ping;
        }
        private static bool s_isRunning = false;
        public static bool IsRunning
        {
            get => s_isRunning;
        }
        private static bool s_isConnected = false;
        public static bool IsConnected
        {
            get => s_isConnected;
        }

        private static EventBasedNetListener s_listener;
        private static NetManager s_client;

        static Network()
        {
            s_listener = new EventBasedNetListener();
            s_client = new NetManager(s_listener)
            {
                AutoRecycle = true
            };

            s_listener.PeerConnectedEvent += PeerConnectedEventHandler;
            s_listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            s_listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            s_listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private static void PeerConnectedEventHandler(NetPeer peer)
        {
            s_isConnected = true;
            s_id = peer.RemoteId;
            Log.Information($"Connected to Server: {s_id}, {peer.Address}:{peer.Port}");
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            s_isConnected = false;
            s_id = -1;
            s_ping = -1;
            Log.Information($"Disconnected to server: {peer.Address}:{peer.Port}, {disconnectInfo.Reason}");
        }

        private static void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte[] buffer = new byte[reader.AvailableBytes];
            reader.GetBytes(buffer, buffer.Length);

            switch (channel)
            {
                case 0x00:
                    switch (MessagePackSerializer.Deserialize<IMultiMessage>(buffer))
                    {
                        case JoinMessage message:
                            Log.Information($"Player joined: {message.JoinID}");
                            break;
                        case LeaveMessage message:
                            Log.Information($"Player left: {message.LeaveID}");
                            break;
                        case UploadVRMMessage message:
                            Avatar.LoadAvatar(message.FromID, message.Data);
                            break;
                    }
                    break;
                case 0x01:
                    switch (MessagePackSerializer.Deserialize<IUniMessage>(buffer))
                    {
                        case ServerInfoMessage message:
                            Avatar.Initialize(message.MaxPlayers);
                            byte[] data = Avatar.GetAvatarData();
                            IMultiMessage vrmMessage = new UploadVRMMessage(s_id, data);
                            Send(vrmMessage);
                            for (int i = 0; i < message.AvatarData.Length; i++)
                            {
                                if (message.AvatarData[i] != null && i != s_id)
                                    Avatar.LoadAvatar(i, message.AvatarData[i]);
                            }
                            break;
                    }
                    break;
            }
        }

        private static void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Error($"NetworkError: {socketError}");
        }

        public static void Connect(string ipAddress, int port, string version)
        {
            s_client.Start();
            s_isRunning = s_client.IsRunning;
            s_client.Connect(ipAddress, port, version);
        }

        public static void Disconnect()
        {
            s_client.Stop();
            s_isRunning = s_client.IsRunning;
        }

        public static void OnUpdate()
        {
            if (!s_client.IsRunning)
                return;
            s_client.PollEvents();
            if (s_client.FirstPeer == null)
                return;
            s_ping = s_client.FirstPeer.Ping;
        }

        public static void Send(IUniMessage message)
        {
            byte[] buffer = MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    s_client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    s_client.FirstPeer.Send(writer, DeliveryMethod.Sequenced);
                    break;
            }
        }

        public static void Send(IMultiMessage message)
        {
            byte[] buffer = MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    s_client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    s_client.FirstPeer.Send(writer, DeliveryMethod.Sequenced);
                    break;
            }
        }
    }
}