using MelonLoader;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
{
    public class Clients
    {
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
                        Melon<Program>.Logger.Msg("Received SendPlayerPosMessage.");
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