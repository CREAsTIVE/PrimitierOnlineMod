using MelonLoader;
using System.Net;
using System.Net.Sockets;

namespace YuchiGames.POM.Client.Network
{
    public static class Utils
    {
        public static int GetFreePort()
        {
            int port = -1;
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                    if (socket.LocalEndPoint is null)
                        throw new Exception("LocalEndPoint is null");
                    port = ((IPEndPoint)socket.LocalEndPoint).Port;
                }
                if (port == -1)
                    throw new Exception("Port is -1");
                return port;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}