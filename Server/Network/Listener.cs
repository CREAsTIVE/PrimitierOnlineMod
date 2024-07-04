using System.Net;
using System.Net.Sockets;

namespace YuchiGames.POM.Server
{
    public class Listeners
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

        public Listeners(int port)
        {
            _localEndPoint = new IPEndPoint(IPAddress.Any, port);
            _tcpListener = new TcpListener(_localEndPoint);
            _udpListener = new UdpClient(_localEndPoint);
            _tcpCancelTokenSource = new CancellationTokenSource();
            _udpCancelTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            if (_isRunning)
                throw new InvalidOperationException("The server is already running.");

            Thread tcpThread = new Thread(() => Tcp(_tcpCancelTokenSource.Token));
        }
        public void Stop()
        {

        }

        private void Tcp(CancellationToken token)
        {

        }
    }
}