using MelonLoader;
using System.Net;
using System.Net.Sockets;

namespace YuchiGames.POM.Client.Network
{
    public static class Listeners
    {
        private static IPEndPoint? s_tcpIPEndPoint;
        public static IPEndPoint TcpIPEndPoint
        {
            get
            {
                if (s_tcpIPEndPoint is null)
                    throw new Exception("TCP IPEndPoint not found.");
                return s_tcpIPEndPoint;
            }
        }
        private static IPEndPoint? s_udpIPEndPoint;
        public static IPEndPoint UdpIPEndPoint
        {
            get
            {
                if (s_udpIPEndPoint is null)
                    throw new Exception("UDP IPEndPoint not found.");
                return s_udpIPEndPoint;
            }
        }

        public static void Tcp()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(Program.Settings.IP), Program.Settings.TcpPort);

            try
            {
                listener.Start();
                s_tcpIPEndPoint = (IPEndPoint)listener.LocalEndpoint;

                Melon<Program>.Logger.Msg($"Tcp server started on port {Program.Settings.TcpPort}.");

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
                Melon<Program>.Logger.Msg($"Tcp server stopped on port {Program.Settings.TcpPort}.");
            }
        }

        public static async void Udp()
        {
            IPEndPoint remoteEndPont = new IPEndPoint(IPAddress.Parse(Program.Settings.IP), Program.Settings.UdpPort);

            try
            {
                using (UdpClient listener = new UdpClient(remoteEndPont))
                {
                    if (listener.Client.LocalEndPoint is null)
                        throw new Exception("LocalEndPoint not found.");
                    s_udpIPEndPoint = (IPEndPoint)listener.Client.LocalEndPoint;

                    Melon<Program>.Logger.Msg($"Udp server started on port {Program.Settings.UdpPort}.");

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

            Melon<Program>.Logger.Msg($"Udp server stopped on port {Program.Settings.UdpPort}.");
        }
    }
}