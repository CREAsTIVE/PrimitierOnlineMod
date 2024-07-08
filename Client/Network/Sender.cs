using LiteNetLib;
using System.Net;
using System.Reflection;

namespace YuchiGames.POM.Client.Network
{
    public class Sender
    {
        private IPEndPoint _localEndPoint;
        private EventBasedNetListener _listener;
        private NetManager _client;
        private Thread _pollEventsThread;

        public Sender()
        {
            _localEndPoint = new IPEndPoint(
                IPAddress.Parse(Program.Settings.IP),
                Program.Settings.Port
                );
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener)
            {
                AutoRecycle = true
            };
            _pollEventsThread = new Thread(PollEvents);

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
            _pollEventsThread.Start();
            _client.Connect(_localEndPoint, version.ToString());
        }

        public void PollEvents()
        {
            Log.Debug("PollEvents occurred.");
            while (_client.IsRunning)
            {
                _client.PollEvents();
            }
        }

        public void Disconnect()
        {
            _client.Stop();
        }
    }
}