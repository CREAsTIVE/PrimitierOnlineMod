using MelonLoader;
using System.Net;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
{
    public static class Senders
    {
        public static ITcpMessage Tcp(ITcpMessage message)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(Program.Settings.IP), Program.Settings.Port);
                using (TcpClient client = new TcpClient(remoteEndPoint))
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

        public static void Udp(IUdpMessage message)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(Program.Settings.IP), Program.Settings.Port);
                using (UdpClient client = new UdpClient(remoteEndPoint))
                {
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

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