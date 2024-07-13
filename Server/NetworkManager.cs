using LiteNetLib;
using MessagePack;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Server
{
    public static class NetworkManager
    {
        private static EventBasedNetListener s_listener;
        private static NetManager s_server;
        private static Thread s_pollEventsThread;

        static NetworkManager()
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

            Version? version = Assembly.GetExecutingAssembly().GetName().Version
                ?? throw new Exception("Version not found.");
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
            ITcpMessage joinMessage = new JoinMessage(peer.Id);
            byte[] buffer = new byte[1024];
            buffer = MessagePackSerializer.Serialize(joinMessage);
            s_server.SendToAll(buffer, DeliveryMethod.ReliableOrdered, peer);
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug("PeerDisconnectedEvent occurred.");
            Log.Information($"Client disconnected: {peer.Address}:{peer.Port}, {peer.Id}, {disconnectInfo.Reason}");
            ITcpMessage leaveMessage = new LeaveMessage(peer.Id);
            byte[] buffer = new byte[1024];
            buffer = MessagePackSerializer.Serialize(leaveMessage);
            s_server.SendToAll(buffer, DeliveryMethod.ReliableOrdered, peer);
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
                s_server.PollEvents();
        }
    }
}