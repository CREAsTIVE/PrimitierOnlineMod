using System.Net;
using System.Net.Sockets;
using MelonLoader;

namespace YuchiGames.POM.Client.Network
{
    public class Listeners
    {
        private static IPEndPoint? s_udpEndPoint;
        public static IPEndPoint UdpEndPoint
        {
            get
            {
                if (s_udpEndPoint is null)
                    throw new Exception("End point not found.");
                return s_udpEndPoint;
            }
        }

        public static async void Udp()
        {
            Melon<Program>.Logger.Msg("Udp listener started on port {0}.", Program.Settings.Port);

            try
            {
                using (UdpClient listener = new UdpClient(Program.Settings.Port))
                {
                    if (listener.Client.LocalEndPoint is null)
                        throw new Exception("Local end point not found.");
                    s_udpEndPoint = (IPEndPoint)listener.Client.LocalEndPoint;

                    while (true)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        _ = Task.Run(() => Clients.Udp(result));
                    }
                }
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }

            Melon<Program>.Logger.Msg("Udp listener stopped on port {0}.", Program.Settings.Port);
        }
    }
}