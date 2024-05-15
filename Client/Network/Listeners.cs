using System.Net;
using System.Net.Sockets;
using MelonLoader;
using YuchiGames.POM.Client;

namespace YuchiGames.POM.Server.Network.Listeners
{
    public static class Tcp
    {
        public static void Listener()
        {
            if (Program.settings == null)
            {
                MelonLogger.Error("Settings is null.");
                return;
            }
            
            TcpListener listener = new TcpListener(IPAddress.Parse(Program.settings.IP), Program.settings.Port);

            try
            {
                listener.Start();
                MelonLogger.Msg($"Tcp listener started on port {Program.settings.Port}.");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    // _ = Task.Run(() => Process.Tcp.Client(client));
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
            finally
            {
                listener.Stop();
            }

            MelonLogger.Msg($"Tcp listener stopped on port {Program.settings.Port}.");
        }
    }

    public static class Udp
    {
        public static async void Listener()
        {
            if (Program.settings == null)
            {
                MelonLogger.Error("Settings is null.");
                return;
            }

            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(Program.settings.IP), Program.settings.Port);
                using (UdpClient listener = new UdpClient(iPEndPoint))
                {
                    MelonLogger.Msg($"Udp listener started on port {Program.settings.Port}.");

                    while (true)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        // _ = Task.Run(() => Process.Udp.Client(result));
                    }
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }

            MelonLogger.Msg($"Udp listener stopped on port {Program.settings.Port}.");
        }
    }
}