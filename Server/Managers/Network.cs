using LiteNetLib;
using MessagePack;
using Serilog;
using YuchiGames.POM.Shared.DataObjects;
using System.Net;
using System.Net.Sockets;
using System.Text;
using YuchiGames.POM.Shared;

namespace YuchiGames.POM.Server.Managers
{
    public class Network
    {
        private EventBasedNetListener _listener;
        private NetManager _server;
        private CancellationTokenSource _cancellationTokenSource;
        private string[] _userGUIDs;

        private const int s_serverId = -1;

        public Network()
        {
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener)
            {
                AutoRecycle = true,
                ChannelsCount = 2
            };
            _cancellationTokenSource = new CancellationTokenSource();
            _userGUIDs = new string[Program.Settings.MaxPlayers];

            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
            _listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            _listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            _listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            byte[] buffer = new byte[request.Data.AvailableBytes];
            request.Data.GetBytes(buffer, buffer.Length);
            AuthData authData = MessagePackSerializer.Deserialize<AuthData>(buffer);
            if (_server.ConnectedPeersCount < Program.Settings.MaxPlayers
                && authData.Version == Program.Version)
            {
                Log.Debug($"Accepted connection from {request.RemoteEndPoint}");
                request.Accept();
            }
            else
            {
                Log.Debug($"Rejected connection from {request.RemoteEndPoint}");
                request.Reject();
            }
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Debug($"Connected peer with ID{peer.Id}");
        }

        private void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug($"Disconnected peer with ID{peer.Id} {disconnectInfo.Reason}");
            _userGUIDs[peer.Id] = "";
            IMultiMessage leaveMessage = new LeaveMessage(peer.Id);
            Send(leaveMessage);
            Log.Information($"Disconnected from Server: {peer.Id}");
        }

        private void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte[] buffer = new byte[reader.AvailableBytes];
            reader.GetBytes(buffer, buffer.Length);
            NetworkReceiveEventProcess(peer, buffer, channel, deliveryMethod);
        }

        private void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Error($"A network error has occurred {socketError}");
        }

        public void Start(int port)
        {
            Thread pollEventsThread = new Thread(() => PollEventsThread(_cancellationTokenSource.Token));
            pollEventsThread.Start();
            _server.Start(port);
            Log.Information("Server started.");
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _server.Stop();
            Log.Information("Server stopped.");
        }

        private void PollEventsThread(CancellationToken token)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _server.LocalPort);
            listener.Start();
            while (!token.IsCancellationRequested)
            {
                _server.PollEvents();
                if (!listener.Pending())
                    continue;
                TcpClient tcpClient = listener.AcceptTcpClient();
                Task.Run(() => DataRequestHandler(tcpClient), token);
            }
            listener.Stop();
        }

        private void DataRequestHandler(TcpClient client)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] length = new byte[4];
                stream.Read(length, 0, length.Length);
                byte[] channel = new byte[1];
                stream.Read(channel, 0, channel.Length);
                byte[] guid = new byte[36];
                stream.Read(guid, 0, guid.Length);
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

                int id = Array.IndexOf(_userGUIDs, Encoding.UTF8.GetString(guid));
                if (id == -1)
                    throw new Exception("User not found.");
                NetPeer peer = _server.GetPeerById(id);
                NetworkReceiveEventProcess(peer, data, channel[0], DeliveryMethod.ReliableOrdered);
            }
        }

        private void NetworkReceiveEventProcess(NetPeer peer, byte[] buffer, byte channel, DeliveryMethod deliveryMethod)
        {
            switch (channel)
            {
                case 0x00:
                    IMultiMessage multiMessage = MessagePackSerializer.Deserialize<IMultiMessage>(buffer);
                    if (multiMessage.FromID != peer.Id)
                        return;
                    switch (multiMessage)
                    {
                        case PlayerPositionMessage:
                            Send(multiMessage, peer);
                            break;
                    };
                    break;
                case 0x01:
                    IUniMessage uniMessage = MessagePackSerializer.Deserialize<IUniMessage>(buffer);
                    if (uniMessage.FromID != peer.Id)
                        return;
                    switch (uniMessage)
                    {
                        case RequestServerInfoMessage message:
                            if (message.ToID != s_serverId)
                                return;
                            _userGUIDs[peer.Id] = message.UserGUID;
                            LocalWorldData localWorldData = World.GetLocalWorldData(message.UserGUID);

                            Send(new ServerInfoMessage(
                                peer.Id,
                                Program.Settings.MaxPlayers,
                                localWorldData,
                                Program.Settings.DayNightCycle
                            ));

                            Send(new JoinMessage(peer.Id), peer);

                            Log.Information($"Connected to Client with ID{peer.Id}");
                            break;
                    }
                    break;
            }
        }

        public void Send(IMultiMessage message)
        {
            byte[] data = MessagePackSerializer.Serialize(message);
            if (message.IsLarge)
            {
                byte[] length = BitConverter.GetBytes(data.Length);
                byte[] channel = new byte[1] { 0x00 };
                byte[] buffer = new byte[length.Length + channel.Length + data.Length];

                int offset = 0;
                Array.Copy(length, 0, buffer, offset, length.Length);
                offset += length.Length;
                Array.Copy(channel, 0, buffer, offset, channel.Length);
                offset += channel.Length;
                Array.Copy(data, 0, buffer, offset, data.Length);

                _server.ConnectedPeerList.ForEach(p =>
                {
                    using TcpClient client = new TcpClient();
                    client.Connect(p.Address, p.Port);
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                });
                return;
            }
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    _server.SendToAll(data, 0x00, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    _server.SendToAll(data, 0x00, DeliveryMethod.Sequenced);
                    break;
            }
        }

        public void Send(IMultiMessage message, NetPeer excludePeer)
        {
            byte[] data = MessagePackSerializer.Serialize(message);
            if (message.IsLarge)
            {
                byte[] length = BitConverter.GetBytes(data.Length);
                byte[] channel = new byte[1] { 0x00 };
                byte[] buffer = new byte[length.Length + channel.Length + data.Length];

                int offset = 0;
                Array.Copy(length, 0, buffer, offset, length.Length);
                offset += length.Length;
                Array.Copy(channel, 0, buffer, offset, channel.Length);
                offset += channel.Length;
                Array.Copy(data, 0, buffer, offset, data.Length);

                _server.ConnectedPeerList.ForEach(p =>
                {
                    if (p != excludePeer)
                    {
                        using TcpClient client = new TcpClient();
                        client.Connect(p.Address, p.Port);
                        using (NetworkStream stream = client.GetStream())
                        {
                            stream.Write(buffer, 0, buffer.Length);
                        }
                    }
                });
                return;
            }
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    _server.SendToAll(data, 0x00, DeliveryMethod.ReliableOrdered, excludePeer);
                    break;
                case ProtocolType.Udp:
                    _server.SendToAll(data, 0x00, DeliveryMethod.Sequenced, excludePeer);
                    break;
            }
        }

        public void Send(IUniMessage message)
        {
            byte[] data = MessagePackSerializer.Serialize(message);
            if (message.IsLarge)
            {
                byte[] length = BitConverter.GetBytes(data.Length);
                byte[] channel = new byte[1] { 0x01 };
                byte[] buffer = new byte[length.Length + channel.Length + data.Length];

                int offset = 0;
                Array.Copy(length, 0, buffer, offset, length.Length);
                offset += length.Length;
                Array.Copy(channel, 0, buffer, offset, channel.Length);
                offset += channel.Length;
                Array.Copy(data, 0, buffer, offset, data.Length);

                NetPeer peer = _server.GetPeerById(message.ToID);
                using TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(peer.Address, peer.Port));
                using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                return;
            }
            switch (message.Protocol)
            {
                case ProtocolType.Tcp:
                    _server.GetPeerById(message.ToID).Send(data, 0x01, DeliveryMethod.ReliableOrdered);
                    break;
                case ProtocolType.Udp:
                    _server.GetPeerById(message.ToID).Send(data, 0x01, DeliveryMethod.Sequenced);
                    break;
            }
        }
    }
}