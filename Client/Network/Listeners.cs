using System.Net;
using System.Net.Sockets;
using MelonLoader;

namespace YuchiGames.POM.Client.Network.Listeners
{
    public static class Tcp
    {
        public static void Listener()
        {
            if (Program.settings == null)
                throw new Exception("Settings is null");
            TcpListener listener = new TcpListener(IPAddress.Parse(Program.settings.IP), Program.settings.Port);

            try
            {
                listener.Start();
                MelonLogger.Msg($"Tcp listener started on port {Program.settings.Port}.");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    // _ = Task.Run(() => Processes.Tcp.Client(client));
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
            finally
            {
                listener.Stop();
                MelonLogger.Msg($"Tcp listener stopped on port {Program.settings.Port}.");
            }
        }
    }

    public static class Udp
    {
        public static async Task ListenerAsync()
        {
            try
            {
                if (Program.settings == null)
                    throw new Exception("Settings is null");

                using (UdpClient listener = new UdpClient(Program.settings.Port))
                {
                    MelonLogger.Msg("Udp listener started on port {0}.", Program.settings!.Port);

                    while (true)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        if (result.RemoteEndPoint.Address.ToString() == Program.settings.IP)
                        {
                            // _ = Task.Run(() => Processes.Udp.Client(result));
                        }
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