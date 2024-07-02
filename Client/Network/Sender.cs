using System.Net;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
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

        public ITcpMessage Tcp(ITcpMessage message)
        {
            try
            {
                using (TcpClient client = new TcpClient(_remoteEndPoint))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    buffer = MessagePack.MessagePackSerializer.Serialize(message);

                    stream.Write(buffer, 0, buffer.Length);
                    stream.Read(buffer, 0, buffer.Length);

                    return MessagePack.MessagePackSerializer.Deserialize<ITcpMessage>(buffer);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Udp(IUdpMessage message)
        {
            try
            {
                using (UdpClient client = new UdpClient())
                {
                    client.Connect(_remoteEndPoint);
                    byte[] buffer = new byte[1024];
                    buffer = MessagePack.MessagePackSerializer.Serialize(message);
                    client.Send(buffer, buffer.Length);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}