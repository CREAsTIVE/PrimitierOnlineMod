using MelonLoader;
using YuchiGames.POM.Data;

namespace YuchiGames.POM.Client.Network
{
    public class Clients
    {
        public void Udp(byte[] buffer)
        {
            try
            {
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