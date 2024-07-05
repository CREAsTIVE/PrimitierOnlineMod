using MessagePack;
using Serilog;
using System.Net;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Server
{
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
        private CancellationTokenSource _tcpCancelTokenSource;
        private CancellationTokenSource _udpCancelTokenSource;
        private bool _isRunning;

        public Listener(int port)
        {
            _localEndPoint = new IPEndPoint(IPAddress.Any, port);
            _tcpCancelTokenSource = new CancellationTokenSource();
            _udpCancelTokenSource = new CancellationTokenSource();
        }
        public Listener(string ip, int port)
        {
            _localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _tcpCancelTokenSource = new CancellationTokenSource();
            _udpCancelTokenSource = new CancellationTokenSource();
        }
        public Listener(IPEndPoint localEndPoint)
        {
            _localEndPoint = localEndPoint;
            _tcpCancelTokenSource = new CancellationTokenSource();
            _udpCancelTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            if (_isRunning)
                throw new InvalidOperationException("The server is already running.");

            Thread tcpThread = new Thread(() => Tcp(_tcpCancelTokenSource.Token));
            tcpThread.Start();
            Thread udpThread = new Thread(() => Udp(_udpCancelTokenSource.Token));
            udpThread.Start();

            _isRunning = true;
        }
        public void Stop()
        {
            if (!_isRunning)
                throw new InvalidOperationException("The server is not running.");

            _tcpCancelTokenSource.Cancel();
            _udpCancelTokenSource.Cancel();

            _isRunning = false;
        }

        private void Tcp(CancellationToken token)
        {
            TcpListener listener = new TcpListener(_localEndPoint);

            try
            {
                listener.Start();
                Log.Information($"Tcp server started on port {_localEndPoint.Port}.");

                while (!token.IsCancellationRequested)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();
                    _ = Task.Run(() => TcpProcess(client));
                }
            }
            catch (OperationCanceledException)
            {
                Log.Warning($"Tcp server stopped on port {_localEndPoint.Port}.");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                listener.Stop();
                Log.Information($"Tcp server stopped on port {_localEndPoint.Port}.");
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

                    ITcpMessage message;
                    switch (MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                    {
                        default:
                            throw new Exception($"Received unknown message from {remoteEndPoint}.");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        private async void Udp(CancellationToken token)
        {
            try
            {
                using (UdpClient listener = new UdpClient(_localEndPoint))
                {
                    Log.Information($"Udp server started on port {_localEndPoint.Port}.");

                    while (!token.IsCancellationRequested)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        _ = Task.Run(() => UdpProcess(result));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
        private void UdpProcess(UdpReceiveResult result)
        {
            IPEndPoint remoteEndPoint = result.RemoteEndPoint;
            byte[] receivedData = result.Buffer;

            try
            {
                using (UdpClient client = new UdpClient())
                {
                    IUdpMessage udpMessage = MessagePackSerializer.Deserialize<IUdpMessage>(receivedData);

                    for (int i = 0; i < Program.UserData.Length; i++)
                    {
                        if (Program.UserData[i] == default)
                            continue;
                        if (
                            Program.UserData[i].Address.Equals(remoteEndPoint.Address) &&
                            Program.UserData[i].ID == udpMessage.ID
                            )
                        {
                            break;
                        }
                        if (i == Program.UserData.Length - 1)
                        {
                            throw new Exception($"Not connected to {remoteEndPoint}.");
                        }
                    }

                    switch (udpMessage)
                    {
                        case SendPlayerPosMessage:
                            for (int i = 0; i < Program.UserData.Length; i++)
                            {
                                if (Program.UserData[i] == default)
                                    continue;
                                if (!Program.UserData[i].Address.Equals(remoteEndPoint.Address))
                                {
                                    client.SendAsync(receivedData, receivedData.Length, new IPEndPoint(Program.UserData[i].Address, Program.UserData[i].Port));
                                    Log.Information("Sent data to {0}.", Program.UserData[i].Address);
                                }
                            }
                            break;
                        default:
                            throw new Exception($"Received unknown message from {remoteEndPoint}.");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}