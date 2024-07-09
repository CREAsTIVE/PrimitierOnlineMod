using LiteNetLib;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace YuchiGames.POM.Server.Network
{
    public class Listener
    {
        private EventBasedNetListener _listener;
        private NetManager _server;
        private Thread _pollEventsThread;

        public Listener()
        {
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener)
            {
                AutoRecycle = true
            };
            _pollEventsThread = new Thread(PollEvents);

            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
            _listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            _listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            _listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            Log.Debug("ConnectionRequestEvent occurred.");

            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is null)
                throw new Exception("Version not found.");

            if (_server.ConnectedPeersCount < Program.Settings.MaxPlayers)
            {
                Log.Information($"Request accepted: {request.RemoteEndPoint}");
                request.AcceptIfKey(version.ToString());
            }
            else
            {
                Log.Information($"Request rejected: {request.RemoteEndPoint}");
                request.Reject();
            }
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
            Log.Debug($"Received: {BitConverter.ToString(buffer)}");
            switch (deliveryMethod)
            {
                case DeliveryMethod.ReliableOrdered:
                    Log.Debug("ReliableOrdered");
                    break;
                case DeliveryMethod.Unreliable:
                    Log.Debug("Unreliable");
                    _server.SendToAll(buffer, DeliveryMethod.Unreliable, peer);
                    break;
            }
        }

        private void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Debug("NetworkErrorEvent occurred.");
            Log.Error($"Error: {socketError}");
        }

        public void Start()
        {
            Log.Information("Server started.");
            _server.Start(Program.Settings.Port);
            _pollEventsThread.Start();
        }

        public void Stop()
        {
            Log.Information("Server stopped.");
            _server.Stop();
        }

        public void PollEvents()
        {
            Log.Debug("PollEvent occurred.");
            while (_server.IsRunning)
            {
                _server.PollEvents();
            }
        }
    }
}