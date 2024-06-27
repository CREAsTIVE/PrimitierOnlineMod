using MelonLoader;
using System.Net.Sockets;
using YuchiGames.POM.Client.MessageMethods;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
{
    public class Clients
    {
        public static void Tcp(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);

                    // ITcpMessage message;
                    switch (MessagePack.MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                    {
                        default:
                            throw new Exception("Received unknown message.");
                    }
                    // buffer = MessagePack.MessagePackSerializer.Serialize(message);
                    // stream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }
        }

        public static void Udp(UdpReceiveResult result)
        {
            try
            {
                byte[] buffer = new byte[1024];
                buffer = result.Buffer;
                IUdpMessage message = MessagePack.MessagePackSerializer.Deserialize<IUdpMessage>(buffer);

                switch (message)
                {
                    case SendPlayerPosMessage:
                        SendPlayerPos.Process((SendPlayerPosMessage)message);
                        break;
                    default:
                        throw new Exception("Unknown message type.");
                }
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }
        }
    }
}