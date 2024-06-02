using System.Net;
using System.Net.Sockets;
using Serilog;

namespace YuchiGames.POM.Server.Network.Listeners
{
    public static class Tcp
    {
        public static void Listener()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Program.settings!.Port);

            try
            {
                listener.Start();
                Log.Information("Tcp server started on port {0}.", Program.settings!.Port);

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    _ = Task.Run(() => Processes.Tcp.Client(client));
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                listener.Stop();
            }

            Log.Information("Tcp server stopped on port {0}.", Program.settings!.Port);
        }
    }

    public static class Udp
    {
        public static async void Listener()
        {
            try
            {
                using (UdpClient listener = new UdpClient(Program.settings!.Port))
                {
                    Log.Information("Udp server started on port {0}.", Program.settings!.Port);

                    while (true)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        _ = Task.Run(() => Processes.Udp.Client(result));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Log.Information("Udp server stopped on port {0}.", Program.settings!.Port);
        }
    }
}