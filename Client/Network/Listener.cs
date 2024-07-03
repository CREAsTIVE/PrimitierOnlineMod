using MelonLoader;
using MessagePack;
using System.Net;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
{
    public class JoinedEventArgs : EventArgs
    {
        public int ID { get; set; }

        public JoinedEventArgs(int id)
        {
            ID = id;
        }
    }
    public class ReceivePlayerPosEventArgs : EventArgs
    {
        public int ID { get; set; }
        public bool IsVRMBody { get; set; }
        public BaseBody BaseBody { get; set; }
        public VRMBody VRMBody { get; set; }

        public ReceivePlayerPosEventArgs(int id, bool isVRMBody, BaseBody baseBody, VRMBody vrmBody)
        {
            ID = id;
            IsVRMBody = isVRMBody;
            BaseBody = baseBody;
            VRMBody = vrmBody;
        }
    }

    public class Listener
    {
        private IPEndPoint _localEndPoint;
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return _localEndPoint;
            }
        }
        private TcpListener _tcpListener;
        private UdpClient _udpListener;
        private CancellationTokenSource _tcpCancelTokenSource;
        private CancellationTokenSource _udpCancelTokenSource;
        private bool _isRunning;

        public event EventHandler<JoinedEventArgs>? JoinedEvent;
        public event EventHandler<ReceivePlayerPosEventArgs>? ReceivePlayerPosEvent;

        public Listener(int port)
        {
            _localEndPoint = new IPEndPoint(IPAddress.Any, port);
            _tcpListener = new TcpListener(_localEndPoint);
            _udpListener = new UdpClient(_localEndPoint);
            _tcpCancelTokenSource = new CancellationTokenSource();
            _udpCancelTokenSource = new CancellationTokenSource();
        }
        public Listener(string ip, int port)
        {
            _localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _tcpListener = new TcpListener(_localEndPoint);
            _udpListener = new UdpClient(_localEndPoint);
            _tcpCancelTokenSource = new CancellationTokenSource();
            _udpCancelTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            try
            {
                if (_isRunning)
                    throw new Exception("Listener is already running.");

                Thread tcpThread = new Thread(() => Tcp(_tcpCancelTokenSource.Token));
                tcpThread.Start();
                Thread udpThread = new Thread(() => Udp(_udpCancelTokenSource.Token));
                udpThread.Start();

                _isRunning = true;
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }
        }
        public void Stop()
        {
            try
            {
                if (!_isRunning)
                    throw new Exception("Listener is not running.");

                _tcpCancelTokenSource.Cancel();
                _udpCancelTokenSource.Cancel();

                _isRunning = false;
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }
        }

        private void Tcp(CancellationToken token)
        {
            try
            {
                Melon<Program>.Logger.Msg($"Tcp server started on port {LocalEndPoint.Port}.");

                while (!token.IsCancellationRequested)
                {
                    TcpClient client = _tcpListener.AcceptTcpClient();
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();
                    _ = Task.Run(() => TcpProcess(client));
                }
            }
            catch (OperationCanceledException)
            {
                Melon<Program>.Logger.Msg($"Tcp server canceled on port {LocalEndPoint.Port}.");
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }
            finally
            {
                _tcpListener.Stop();
                Melon<Program>.Logger.Msg($"Tcp server stopped on port {_localEndPoint.Port}.");
            }
        }
        private void TcpProcess(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    if (client.Client.RemoteEndPoint is null)
                        throw new Exception("RemoteEndPoint not found.");
                    IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;

                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);

                    switch (MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                    {
                        case JoinedMessage connect:
                            OnJoinedEvent(new JoinedEventArgs(connect.ID));
                            break;
                        default:
                            throw new Exception($"Received unknown message from {remoteEndPoint}.");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void Udp(CancellationToken token)
        {
            try
            {
                Melon<Program>.Logger.Msg($"Udp server started on port {LocalEndPoint.Port}.");

                while (!token.IsCancellationRequested)
                {
                    UdpReceiveResult result = await _udpListener.ReceiveAsync();
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();
                    _ = Task.Run(() => UdpProcess(result));
                }
            }
            catch (OperationCanceledException)
            {
                Melon<Program>.Logger.Msg($"Udp server canceled on port {LocalEndPoint.Port}.");
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }

            Melon<Program>.Logger.Msg($"Udp server stopped on port {LocalEndPoint.Port}.");
        }
        private void UdpProcess(UdpReceiveResult result)
        {
            try
            {
                IPEndPoint remoteEndPoint = result.RemoteEndPoint;
                byte[] receivedData = result.Buffer;

                switch (MessagePackSerializer.Deserialize<IUdpMessage>(receivedData))
                {
                    case SendPlayerPosMessage playerPos:
                        OnReceivePlayerPosEvent(new ReceivePlayerPosEventArgs(playerPos.ID, playerPos.IsVRMBody, playerPos.BaseBody, playerPos.VrmBody));
                        break;
                    default:
                        throw new Exception($"Received unknown message from {remoteEndPoint}.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected virtual void OnReceivePlayerPosEvent(ReceivePlayerPosEventArgs e)
        {
            EventHandler<ReceivePlayerPosEventArgs>? receivePlayerPosHandler = ReceivePlayerPosEvent;

            if (receivePlayerPosHandler != null)
                receivePlayerPosHandler(this, e);
        }
        protected virtual void OnJoinedEvent(JoinedEventArgs e)
        {
            EventHandler<JoinedEventArgs>? joinedHandler = JoinedEvent;

            if (joinedHandler != null)
                joinedHandler(this, e);
        }
    }
}