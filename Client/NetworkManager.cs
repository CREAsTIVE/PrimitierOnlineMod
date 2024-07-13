using LiteNetLib;
using LiteNetLib.Utils;
using MessagePack;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client
{
    public static class NetworkManager
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

        static NetworkManager()
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
            Log.Debug("PeerConnectedEvent occurred.");
            s_id = peer.Id;
            s_isConnected = true;
            Log.Information($"Client connected: {peer.Address}:{peer.Port}, {peer.Id}");
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug("PeerDisconnectedEvent occurred.");
            s_id = -1;
            s_ping = -1;
            s_isConnected = false;
            Log.Information($"Client disconnected: {peer.Address}:{peer.Port}, {peer.Id}, {disconnectInfo.Reason}");
        }

        private static void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            Log.Debug("NetworkReceiveEvent occurred.");
            if (deliveryMethod == DeliveryMethod.ReliableOrdered)
            {
                byte[] buffer = new byte[1024];
                reader.GetBytes(buffer, buffer.Length);
                switch (MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                {
                    case JoinMessage joinMessage:
                        Log.Debug($"Received JoinMessage. {joinMessage.ID}");
                        break;
                    case LeaveMessage leaveMessage:
                        Log.Debug($"Received LeaveMessage. {leaveMessage.ID}");
                        break;
                    case UpdateVRMMessage updateVRMMessage:
                        Log.Debug($"Received UpdateVRMMessage. {updateVRMMessage.ID}, {updateVRMMessage.Data.Length} bytes.");
                        break;
                    default:
                        Log.Debug("Unknown Message");
                        break;
                }
            }
            else if (deliveryMethod == DeliveryMethod.Unreliable)
            {
                byte[] buffer = new byte[1024];
                reader.GetBytes(buffer, buffer.Length);
                switch (MessagePackSerializer.Deserialize<IUdpMessage>(buffer))
                {
                    default:
                        Log.Debug("Unknown Message");
                        break;
                }
            }
            else
            {
                Log.Debug("Unknown DeliveryMethod Message");
            }
        }

        private static void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Debug("NetworkErrorEvent occurred.");
            Log.Error($"Error: {socketError}");
        }

        public static void Connect()
        {
            Version? version = Assembly.GetExecutingAssembly().GetName().Version
                ?? throw new Exception("Version not found.");
            s_client.Start();
            s_client.Connect(Program.Settings.IP, Program.Settings.Port, version.ToString());
        }

        public static void Disconnect()
        {
            s_client.Stop();
        }

        public static void OnUpdate()
        {
            if (!s_client.IsRunning)
                return;
            s_client.PollEvents();
            if (s_client.FirstPeer != null)
                s_ping = s_client.FirstPeer.Ping;
            s_isRunning = s_client.IsRunning;
        }

        public static void SendTcp(ITcpMessage message)
        {
            byte[] buffer = new byte[1024];
            buffer = MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            s_client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendUdp(IUdpMessage message)
        {
            byte[] buffer = new byte[1024];
            buffer = MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            s_client.FirstPeer.Send(writer, DeliveryMethod.Unreliable);
        }
    }
}