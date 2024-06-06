using System.Net.Sockets;
using MelonLoader;
using MessagePack;
using YuchiGames.POM.Client.Data;

namespace YuchiGames.POM.Client.Network.Senders
{
    public static class Tcp
    {
        public static ITcpMessage Sender(ITcpMessage message)
        {
            try
            {
                if (Program.settings == null)
                    throw new Exception("Settings is null");

                byte[] buffer = new byte[1024];
                buffer = MessagePackSerializer.Serialize(message);

                using (TcpClient client = new TcpClient(Program.settings.IP, Program.settings.Port))
                using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Read(buffer, 0, buffer.Length);

                    message = MessagePackSerializer.Deserialize<ITcpMessage>(buffer);
                    if (message is FailureMessage)
                        throw new Exception("Failure message received.");

                    return message;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static class Udp
    {
        public static void Sender(IUdpMessage message)
        {
            try
            {
                if (Program.settings == null)
                    throw new Exception("Settings is null");

                byte[] buffer = new byte[1024];
                buffer = MessagePackSerializer.Serialize(message);

                using (UdpClient client = new UdpClient(Program.settings.IP, Program.settings.Port))
                {
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