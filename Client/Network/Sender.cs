using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
{
    public class Sender
    {
        private EventBasedNetListener _listener;
        private NetManager _client;

        public Sender()
        {
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener)
            {
                AutoRecycle = true
            };

            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
            _listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            _listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            _listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Debug("PeerConnectedEvent occurred.");
            Log.Information($"Client connected: {peer.Address}:{peer.Port}, {peer.Id}");
        }

        private void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug("PeerDisconnectedEvent occurred.");
            Log.Information($"Client disconnected: {peer.Address}:{peer.Port}, {peer.Id}, {disconnectInfo.Reason}");
        }

        private void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            Log.Debug("NetworkReceiveEvent occurred.");
            byte[] buffer = new byte[1024];
            reader.GetBytes(buffer, buffer.Length);
            Log.Debug($"Received data: {BitConverter.ToString(buffer)}");
        }

        private void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Debug("NetworkErrorEvent occurred.");
            Log.Error($"Error: {socketError}");
        }

        public void Connect()
        {
            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is null)
                throw new Exception("Version not found.");

            _client.Start();
            _client.Connect(Program.Settings.IP, Program.Settings.Port, version.ToString());
        }

        public void Disconnect()
        {
            _client.Stop();
        }

        public void PollEventsHandler()
        {
            _client.PollEvents();
        }

        public void SendTcp(ITcpMessage message)
        {
            byte[] buffer = new byte[1024];
            buffer = MessagePack.MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            _client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void SendUdp(IUdpMessage message)
        {
            byte[] buffer = new byte[1024];
            buffer = MessagePack.MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            _client.FirstPeer.Send(writer, DeliveryMethod.Unreliable);
        }
    }
}