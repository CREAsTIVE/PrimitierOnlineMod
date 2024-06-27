using System.Net;
using System.Net.Sockets;
using Serilog;

namespace YuchiGames.POM.Server.Network
{
    public static class Listeners
    {
        public static void Tcp()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Program.Settings.TcpPort);

            try
            {
                listener.Start();
                Log.Information("Tcp server started on port {0}.", Program.Settings.TcpPort);

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    _ = Task.Run(() => Clients.Tcp(client));
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                listener.Stop();
                Log.Information("Tcp server stopped on port {0}.", Program.Settings.TcpPort);
            }
        }

        public static async void Udp()
        {
            try
            {
                using (UdpClient listener = new UdpClient(Program.Settings.UdpPort))
                {
                    Log.Information("Udp server started on port {0}.", Program.Settings.UdpPort);

                    while (true)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        _ = Task.Run(() => Clients.Udp(result));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Log.Information("Udp server stopped on port {0}.", Program.Settings.UdpPort);
        }
    }
}