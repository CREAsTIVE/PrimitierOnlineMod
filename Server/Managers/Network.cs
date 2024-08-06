using Serilog;
using LiteNetLib;
using System.Net;
using MessagePack;
using LiteNetLib.Utils;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Server.Managers
{
    public static class Network
    {
        private static EventBasedNetListener s_listener;
        private static NetManager s_server;
        private static Thread s_pollEventsThread;

        static Network()
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
            byte[] buffer = new byte[request.Data.AvailableBytes];
            request.Data.GetBytes(buffer, buffer.Length);
            AuthData authData = MessagePackSerializer.Deserialize<AuthData>(buffer);
            if (s_server.ConnectedPeersCount < Program.Settings.MaxPlayers
                && authData.Version == Program.Version)
            {
                request.Accept();
                Log.Information($"Request accepted: {request.RemoteEndPoint}");
            }
            else
            {
                Log.Information($"Request rejected: {request.RemoteEndPoint}");
                request.Reject();
            }
        }

        private static void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Information($"Client connected: {peer.Id}, {peer.Address}:{peer.Port}");
            IMultiMessage joinMessage = new JoinMessage(0, peer.Id);
            Send(joinMessage, peer);
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Information($"Client disconnected: {peer.Id}, {peer.Address}:{peer.Port}, {disconnectInfo.Reason}");
            IMultiMessage leaveMessage = new LeaveMessage(0, peer.Id);
            Send(leaveMessage, peer);
        }

        private static void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            if (reader.AvailableBytes > Program.Settings.MaxDataSize)
            {
                Log.Error("Data size is too large.");
                return;
            }
            byte[] buffer = new byte[reader.AvailableBytes];
            reader.GetBytes(buffer, buffer.Length);

            switch (channel)
            {
                case 0x00:
                    try
                    {
                        IMultiMessage multiMessage = MessagePackSerializer.Deserialize<IMultiMessage>(buffer);
                        if (multiMessage.FromID != peer.Id)
                            return;
                        switch (multiMessage)
                        {
                            case UploadVRMMessage:
                            case AvatarPositionMessage:
                                Send(multiMessage);
                                break;
                        };
                    }
                    catch (MessagePackSerializationException)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Error: {e.Message}");
                        return;
                    }
                    break;
                case 0x01:
                    try
                    {
                        IUniMessage uniMessage = MessagePackSerializer.Deserialize<IUniMessage>(buffer);
                        if (uniMessage.FromID != peer.Id)
                            return;
                        switch (uniMessage)
                        {
                            case RequestServerInfoMessage requestServerInfoMessage:
                                byte[][] vrmData = Avatar.GetAllVRMFiles();
                                LocalWorldData localWorldData = World.GetLocalWorldData(requestServerInfoMessage.UserGUID);
                                IUniMessage infoMessage = new ServerInfoMessage(0, peer.Id, Program.Settings.MaxPlayers, vrmData, localWorldData);
                                Send(infoMessage);
                                break;
                        }
                    }
                    catch (MessagePackSerializationException)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Error: {e.Message}");
                        return;
                    }
                    break;
            }
        }

        private static void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Error($"NetworkError: {socketError}");
        }

        public static void Start(int port)
        {
            Log.Information("Server started.");
            s_server.Start(port);
            s_pollEventsThread.Start();
        }

        public static void Stop()
        {
            Log.Information("Server stopped.");
            s_server.Stop();
        }

        public static void PollEvents()
        {
            while (s_server.IsRunning)
                s_server.PollEvents();
        }

        public static void Send(IUniMessage message)
        {
            byte[] buffer = MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    s_server.GetPeerById(message.ToID).Send(buffer, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    s_server.GetPeerById(message.ToID).Send(buffer, DeliveryMethod.Sequenced);
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
                    s_server.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    s_server.SendToAll(buffer, DeliveryMethod.Sequenced);
                    break;
            }
        }

        public static void Send(IMultiMessage message, NetPeer excludePeer)
        {
            byte[] buffer = MessagePackSerializer.Serialize(message);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(buffer);
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    s_server.SendToAll(buffer, DeliveryMethod.ReliableOrdered, excludePeer);
                    break;
                case ProtocolType.Udp:
                    s_server.SendToAll(buffer, DeliveryMethod.Sequenced, excludePeer);
                    break;
            }
        }
    }
}