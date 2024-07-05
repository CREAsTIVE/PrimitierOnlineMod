using System.Net;

namespace YuchiGames.POM.Server.Network
{
    public class Sender
    {
        private IPEndPoint _remoteEndPoint;
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return _remoteEndPoint;
            }
        }

        public Sender(string ip, int port)
        {
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        public Sender(IPEndPoint remoteEndPoint)
        {
            _remoteEndPoint = remoteEndPoint;
        }
    }
}