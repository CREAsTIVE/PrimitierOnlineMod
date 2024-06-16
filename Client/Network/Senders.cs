using System.Net;
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
                    client.Send(buffer, buffer.Length);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void FirstConnect()
        {
            try
            {
                using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect(Program.Settings.IP, Program.Settings.Port);
                    byte[] buffer = new byte[1024];
                    buffer = MessagePackSerializer.Serialize(new ConnectMessage(Program.Settings.Version, Program.Settings.Name));
                    socket.Send(buffer, buffer.Length, SocketFlags.None);
                    socket.Receive(buffer, buffer.Length, SocketFlags.None);

                    switch (MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                    {
                        case SuccessConnectionMessage successConnectionMessage:
                            if (socket.LocalEndPoint is null)
                                throw new Exception("Local end point not found.");
                            Program.EndPoint = (IPEndPoint)socket.LocalEndPoint;
                            Program.MyID = successConnectionMessage.YourID;
                            break;
                        case FailureMessage failureMessage:
                            throw failureMessage.ExceptionMessage;
                        default:
                            throw new Exception("Unknown message type.");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}