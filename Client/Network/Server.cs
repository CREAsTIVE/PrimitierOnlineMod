using System.Net;

namespace YuchiGames.POM.Client.Network
{
    public class Listener
    {
        private IPEndPoint _ipEndPoint;
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return _ipEndPoint;
            }
        }

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;

        public Listener(int port)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Any, port);
        }
        public Listener(string ip, int port)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
    }
}