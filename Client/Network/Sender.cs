using LiteNetLib;
using LiteNetLib.Utils;
using MelonLoader;
using System.Reflection;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
{
    public class Sender : MelonMod
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
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Debug("PeerConnectedEvent occurred.");
            Log.Information($"Connected to server: {peer.Address}:{peer.Port}, {peer.Id}");
        }

        private void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug("PeerDisconnectedEvent occurred.");
            Log.Information($"Disconnected from server: {peer.Address}:{peer.Port}, {peer.Id}, {disconnectInfo.Reason}");
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

        public override void OnUpdate()
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