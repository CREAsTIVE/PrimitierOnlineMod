using System.Net.Sockets;
using MelonLoader;

namespace YuchiGames.POM.Client.Network.Listeners
{
    public static class Udp
    {
        public static async void Listener()
        {
            if (Program.settings is null)
                throw new Exception("Settings is null");

            try
            {
                using (UdpClient listener = new UdpClient(Program.settings.Port))
                {
                    MelonLogger.Msg("Udp listener started on port {0}.", Program.settings.Port);

                    while (true)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        if (result.RemoteEndPoint.Address.ToString() == Program.settings.IP)
                            throw new Exception("Not a message sent by the server.");
                        _ = Task.Run(() =>
                        {
                            result = default;
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