using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
{
    public static class Sender
    {
        private static EventBasedNetListener s_listener;
        private static NetManager s_client;

        static Sender()
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
            Log.Information($"Client connected: {peer.Address}:{peer.Port}, {peer.Id}");
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug("PeerDisconnectedEvent occurred.");
            Log.Information($"Client disconnected: {peer.Address}:{peer.Port}, {peer.Id}, {disconnectInfo.Reason}");
        }

        private static void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            Log.Debug("NetworkReceiveEvent occurred.");
            byte[] buffer = new byte[1024];
            reader.GetBytes(buffer, buffer.Length);
            Log.Debug($"Received data: {BitConverter.ToString(buffer)}");
        }

        private static void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Debug("NetworkErrorEvent occurred.");
            Log.Error($"Error: {socketError}");
        }

        public static void Connect()
        {
            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is null)
                throw new Exception("Version not found.");

            s_client.Start();
            s_client.Connect(Program.Settings.IP, Program.Settings.Port, version.ToString());
        }

        public static void Disconnect()
        {
            s_client.Stop();
        }

        public static void PollEventsHandler()
        {
            s_client.PollEvents();
        }

        public static void SendTcp(ITcpMessage message)
        {
            byte[] buffer = new byte[1024];
            buffer = MessagePack.MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            s_client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendUdp(IUdpMessage message)
        {
            byte[] buffer = new byte[1024];
            buffer = MessagePack.MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            s_client.FirstPeer.Send(writer, DeliveryMethod.Unreliable);
        }
    }
}