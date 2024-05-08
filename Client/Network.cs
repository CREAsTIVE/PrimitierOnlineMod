using System.Net.Sockets;
using System.Net;
using MelonLoader;

namespace YuchiGames.POM.Client.Network
{
    public static class Tcp
    {
        public static void Listener()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(Program.settings!.IPAddress), Program.settings.Port);
            TcpListener listener = new TcpListener(iPEndPoint);

            try
            {
                listener.Start();

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    using (client)
                    {

                    }
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
        }

        public static void Sender(byte[] bytes)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(Program.settings!.IPAddress), Program.settings.Port);
            try
            {
                using (TcpClient client = new TcpClient(iPEndPoint))
                using (NetworkStream stream = client.GetStream())
                {

                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }
    }

    public static class Udp
    {
        public static void Listener()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(Program.settings!.IPAddress), Program.settings.Port);

            try
            {
                using (UdpClient client = new UdpClient(iPEndPoint))
                {
                    while (true)
                    {
                        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] bytes = client.Receive(ref remoteEndPoint);
                    }
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }

        public static void Sender(byte[] bytes)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(Program.settings!.IPAddress), Program.settings.Port);
            try
            {
                using (UdpClient client = new UdpClient(iPEndPoint))
                {
                    client.Send(bytes, bytes.Length);
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }
    }
}
