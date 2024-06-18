using System.Net.Sockets;
using MessagePack;
using YuchiGames.POM.Data;

namespace YuchiGames.POM.Client.Network
{
    public class Senders
    {
        public static ITcpMessage Tcp(ITcpMessage message)
        {
            try
            {
                byte[] buffer = new byte[1024];
                buffer = MessagePackSerializer.Serialize(message);

                using (TcpClient client = new TcpClient(Program.Settings.IP, Program.Settings.Port))
                {
                    client.Client.Bind(Program.EndPoint);
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Read(buffer, 0, buffer.Length);

                        ITcpMessage receiveMessage = MessagePackSerializer.Deserialize<ITcpMessage>(buffer);
                        switch (receiveMessage)
                        {
                            case SuccessMessage:
                                return receiveMessage;
                            case FailureMessage failureMessage:
                                throw failureMessage.ExceptionMessage;
                            default:
                                throw new Exception("Unknown message type.");
                        }
                    }
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
                byte[] buffer = new byte[1024];
                buffer = MessagePackSerializer.Serialize(message);

                using (UdpClient client = new UdpClient(Program.Settings.IP, Program.Settings.Port))
                {
                    client.Client.Bind(Program.EndPoint);
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