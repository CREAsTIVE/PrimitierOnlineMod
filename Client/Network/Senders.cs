using System.Net;
using System.Net.Sockets;
using MessagePack;
using YuchiGames.POM.Data;

namespace YuchiGames.POM.Client.Network
{
    public class Senders
    {
        public ITcpMessage Tcp(ITcpMessage message)
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
                        case SuccessMessage or SuccessConnectionMessage or FailureMessage:
                            return receiveMessage;
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

        public void Udp(IUdpMessage message)
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

        public IPEndPoint Connect()
        {
            try
            {
                using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect(Program.Settings.IP, Program.Settings.Port);
                    byte[] buffer = new byte[1024];
                    buffer = MessagePackSerializer.Serialize(new ConnectMessage(Program.Settings.Version, Program.Settings.Name));
                    socket.Send(buffer, buffer.Length, SocketFlags.None);

                    if (socket.RemoteEndPoint is null)
                        throw new Exception("Failed to connect to server.");
                    return (IPEndPoint)socket.RemoteEndPoint;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}