using System.Net.Sockets;
using MelonLoader;
using MessagePack;
using YuchiGames.POM.Client.Data;

namespace YuchiGames.POM.Client.Network
{
    public class Listeners
    {
        public async void Udp()
        {
            try
            {
                using (UdpClient listener = new UdpClient(Program.Settings.Port))
                {
                    MelonLogger.Msg("Udp listener started on port {0}.", Program.Settings.Port);

                    while (true)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        if (result.RemoteEndPoint.Address.ToString() == Program.Settings.IP)
                            throw new Exception("Not a message sent by the server.");
                        _ = Task.Run(() =>
                        {
                            IUdpMessage message = MessagePackSerializer.Deserialize<IUdpMessage>(result.Buffer);
                            switch (message)
                            {
                                case SendPlayerPosMessage sendPlayerPosMessage:
                                    MelonLogger.Msg("Received player position message from {0}.", sendPlayerPosMessage.PlayerID);
                                    break;
                                default:
                                    throw new Exception("Unknown message type.");
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }
    }
}