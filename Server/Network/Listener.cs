using LiteNetLib;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace YuchiGames.POM.Server.Network
{
    public static class Listener
    {
        private static EventBasedNetListener s_listener;
        private static NetManager s_server;
        private static Thread s_pollEventsThread;

        static Listener()
        {
            s_listener = new EventBasedNetListener();
            s_server = new NetManager(s_listener)
            {
                AutoRecycle = true
            };
            s_pollEventsThread = new Thread(PollEvents);

            s_listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            s_listener.PeerConnectedEvent += PeerConnectedEventHandler;
            s_listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            s_listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            s_listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private static void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            Log.Debug("ConnectionRequestEvent occurred.");

            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is null)
                throw new Exception("Version not found.");

            if (s_server.ConnectedPeersCount < Program.Settings.MaxPlayers)
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
            Log.Debug($"Received: {BitConverter.ToString(buffer)}");
            switch (deliveryMethod)
            {
                case DeliveryMethod.ReliableOrdered:
                    Log.Debug("ReliableOrdered");
                    break;
                case DeliveryMethod.Unreliable:
                    Log.Debug("Unreliable");
                    s_server.SendToAll(buffer, DeliveryMethod.Unreliable, peer);
                    break;
            }
        }

        private static void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Debug("NetworkErrorEvent occurred.");
            Log.Error($"Error: {socketError}");
        }

        public static void Start()
        {
            Log.Information("Server started.");
            s_server.Start(Program.Settings.Port);
            s_pollEventsThread.Start();
        }

        public static void Stop()
        {
            Log.Information("Server stopped.");
            s_server.Stop();
        }

        public static void PollEvents()
        {
            Log.Debug("PollEvent occurred.");
            while (s_server.IsRunning)
            {
                s_server.PollEvents();
            }
        }
    }
}