using System.Net;
using System.Net.Sockets;
using Serilog;

namespace YuchiGames.POM.Server.Network.Listeners
{
    public class Tcp
    {
        public void Listener()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Program.Settings.Port);

            try
            {
                listener.Start();
                Log.Information("Tcp server started on port {0}.", Program.Settings.Port);

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    _ = Task.Run(() => Clients.Tcp.Client(client));
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                listener.Stop();
                Log.Information("Tcp server stopped on port {0}.", Program.Settings.Port);
            }
        }
    }

    public class Udp
    {
        public async void Listener()
        {
            if (Program.Settings is null)
                throw new Exception("Settings not found.");

            try
            {
                using (UdpClient listener = new UdpClient(Program.Settings.Port))
                {
                    Log.Information("Udp server started on port {0}.", Program.Settings.Port);

                    while (true)
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        _ = Task.Run(() => Clients.Udp.Client(result));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Log.Information("Udp server stopped on port {0}.", Program.Settings.Port);
        }
    }
}