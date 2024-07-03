using Serilog;
using System.Net;
using System.Net.Sockets;

namespace YuchiGames.POM.Server.Network
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
        private TcpListener _tcpListener;
        private UdpClient _udpListener;
        private CancellationTokenSource _tcpCancelTokenSource;
        private CancellationTokenSource _udpCancelTokenSource;
        private bool _isRunning;

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
                Log.Error(e.Message);
            }
        }
        public void Stop()
        {
            if (!_isRunning)
                throw new Exception("Listener is not running.");

            _tcpCancelTokenSource.Cancel();
            _udpCancelTokenSource.Cancel();

            _isRunning = false;
        }

        private void Tcp(CancellationToken token)
        {

        }

        private void Udp(CancellationToken token)
        {

        }
    }
}