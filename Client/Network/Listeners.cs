using MelonLoader;
using System.Net;
using System.Net.Sockets;

namespace YuchiGames.POM.Client.Network
{
    public static class Listeners
    {
        public static void Tcp()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(Program.Settings.IP), Program.Settings.ListenPort);
            TcpListener listener = new TcpListener(localEndPoint);

            try
            {
                listener.Start();

                Melon<Program>.Logger.Msg($"Tcp server started on port {localEndPoint.Port}.");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    _ = Task.Run(() => Clients.Tcp(client));
                }
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }
            finally
            {
                listener.Stop();
                Melon<Program>.Logger.Msg($"Tcp server stopped on port {localEndPoint.Port}.");
            }
        }

        public static async void Udp()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(Program.Settings.IP), Program.Settings.ListenPort);

            try
            {
                using (UdpClient listener = new UdpClient(localEndPoint))
                {
                    Melon<Program>.Logger.Msg($"Udp server started on port {localEndPoint.Port}.");

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

            Melon<Program>.Logger.Msg($"Udp server stopped on port {localEndPoint.Port}.");
        }
    }
}