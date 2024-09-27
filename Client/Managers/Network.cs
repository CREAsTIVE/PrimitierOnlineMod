using LiteNetLib;
using System.Net;
using MessagePack;
using LiteNetLib.Utils;
using System.Net.Sockets;
using YuchiGames.POM.Shared;
using System.Text;
using YuchiGames.POM.Client.Assets;
using UnityEngine;
using Il2CppPinwheel.Jupiter;
using Il2Cpp;
using YuchiGames.POM.Shared.DataObjects;
using Il2CppMToon;
using Client;

namespace YuchiGames.POM.Client.Managers
{
    public static class Network
    {
        public static int ID { get; private set; }
        public static int Ping { get; private set; }
        public static bool IsRunning { get; private set; }
        public static bool IsConnected { get; private set; }
        public static ServerInfoMessage ServerInfo { get; private set; }

        private static EventBasedNetListener s_listener;
        private static NetManager s_client;
        private static CancellationTokenSource s_cancelTokenSource;

        private const int s_serverId = -1;

        static Network()
        {
            ID = -1;
            Ping = -1;
            IsConnected = false;
            ServerInfo = new ServerInfoMessage(
                ID,
                0,
                new LocalWorldData(),
                true);

            s_listener = new EventBasedNetListener();
            s_client = new NetManager(s_listener)
            {
                AutoRecycle = true,
                ChannelsCount = 2
            };
            s_cancelTokenSource = new CancellationTokenSource();

            s_listener.PeerConnectedEvent += PeerConnectedEventHandler;
            s_listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            s_listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            s_listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private static void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Debug($"Connected peer with ID{peer.RemoteId}");
            ID = peer.RemoteId;
            IServerDataMessage message = new RequestServerInfoMessage(
                ID,
                Program.UserGUID);
            Send(message);
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug($"Disconnected peer {disconnectInfo.Reason}");
            IsConnected = false;
            ID = -1;
            Ping = -1;
            s_cancelTokenSource.Cancel();
            if (GameObject.Find("/TitleSpace/TitleMenu/MainCanvas/StartButton") is not null)
                Assets.StartButton.IsInteractable = true;
            Log.Information($"Disconnected from Server");
        }

        private static void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte[] buffer = new byte[reader.AvailableBytes];
            reader.GetBytes(buffer, buffer.Length);
            NetworkReceiveEventProcess(peer, buffer, channel, deliveryMethod);
        }

        private static void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Error($"A network error has occurred {socketError}");
        }

        public static void Connect(string ipAddress, int port)
        {
            Log.Information($"Connecting to Server...");
            s_cancelTokenSource = new CancellationTokenSource();
            s_client.Start();
            AuthData authData = new AuthData(Program.Version);
            byte[] buffer = MessagePackSerializer.Serialize(authData);
            NetDataWriter data = new NetDataWriter();
            data.Put(buffer);
            s_client.Connect(ipAddress, port, data);
            Thread tcpThread = new Thread(() => ReceiveDataRequestsThread(s_cancelTokenSource.Token));
            tcpThread.Start();
        }

        public static void Disconnect()
        {
            Log.Information($"Disconnecting from Server...");
            s_cancelTokenSource.Cancel();
            s_client.Stop();
            Log.Information($"Disconnected from Server");
        }

        public static void OnUpdate()
        {
            s_client.PollEvents();
            if (s_client.FirstPeer != null)
                Ping = s_client.FirstPeer.Ping;
            if (IsConnected)
            {
                IGameDataMessage message = new PlayerPositionMessage(
                    ID,
                    Player.GetPlayerPosition());
                Send(message);
            }
        }

        private static void ReceiveDataRequestsThread(CancellationToken token)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, s_client.LocalPort);
            listener.Start();
            while (!token.IsCancellationRequested)
            {
                if (!listener.Pending())
                    continue;
                TcpClient tcpClient = listener.AcceptTcpClient();
                Task.Run(() => DataRequestHandler(tcpClient), token);
            }
            listener.Stop();
        }

        private static void DataRequestHandler(TcpClient client)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] length = new byte[4];
                stream.Read(length, 0, length.Length);
                byte[] channel = new byte[1];
                stream.Read(channel, 0, channel.Length);
                byte[] data = new byte[BitConverter.ToInt32(length)];
                byte[] buffer = new byte[1024];
                int readDataLength = 0;
                int i;
                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    Array.Copy(buffer, 0, data, readDataLength, i);
                    readDataLength += i;
                }

                if (readDataLength != data.Length)
                    throw new Exception("Data length mismatch.");

                NetworkReceiveEventProcess(s_client.FirstPeer, data, channel[0], DeliveryMethod.ReliableOrdered);
            }
        }

        private static void NetworkReceiveEventProcess(NetPeer peer, byte[] buffer, byte channel, DeliveryMethod deliveryMethod)
        {
            switch (channel)
            {
                case 0x00:
                    switch (MessagePackSerializer.Deserialize<IGameDataMessage>(buffer))
                    {
                        case JoinMessage message:
                            Log.Information($"Joined player with ID{message.JoinID}");
                            Player.SpawnPlayer(message.JoinID);
                            break;
                        case LeaveMessage message:
                            Log.Information($"Player left with ID{message.LeaveID}");
                            Player.DespawnPlayer(message.LeaveID);
                            break;
                        case PlayerPositionMessage message:
                            if (IsConnected)
                                Player.SetPlayerPosition(message.FromID, message.PlayerPos);
                            break;
                    }
                    break;
                case 0x01:
                    switch (MessagePackSerializer.Deserialize<IServerDataMessage>(buffer))
                    {
                        case ServerInfoMessage message:
                            var dayNightCycleButton = UnityUtils.FindGameObjectOfType<DayNightCycleButton>();
                            dayNightCycleButton.SwitchState(message.IsDayNightCycle);
                            ServerInfo = message;
                            World.LoadWorldData(message.WorldData);
                            IsConnected = true;
                            Log.Information($"Connected to Server with ID: {ID}");
                            Assets.StartButton.JoinGame();
                            foreach (NetPeer i in s_client.ConnectedPeerList)
                            {
                                if (i.Id == ID)
                                    continue;
                                Player.SpawnPlayer(i.Id);
                            }
                            break;
                    }
                    break;
            }
        }

        public static void Send(IGameDataMessage message)
        {
            byte[] data = MessagePackSerializer.Serialize(message);

            s_client.FirstPeer.Send(data, 0x00, message.Protocol switch
            {
                ProtocolType.Tcp => DeliveryMethod.ReliableOrdered,
                ProtocolType.Udp => DeliveryMethod.Sequenced,
                _ => throw new NotSupportedException()
            });
        }

        public static void Send(IServerDataMessage message)
        {
            byte[] data = MessagePackSerializer.Serialize(message);

            s_client.FirstPeer.Send(data, 0x01, message.Protocol switch
            {
                ProtocolType.Tcp => DeliveryMethod.ReliableOrdered,
                ProtocolType.Udp => DeliveryMethod.Sequenced,
                _ => throw new NotSupportedException()
            });
        }
    }
}