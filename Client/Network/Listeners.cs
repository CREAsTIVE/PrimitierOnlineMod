using System.Net;
using System.Net.Sockets;
using MelonLoader;

namespace YuchiGames.POM.Client.Network
{
    public class Listeners
    {
        public static void Udp()
        {
            try
            {
                using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Udp))
                {
                    socket.Bind(new IPEndPoint(IPAddress.Any, Program.EndPoint.Port));
                    socket.Listen();

                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        Socket handler = socket.Accept();
                        handler.Receive(buffer);
                        _ = Task.Run(() => Clients.Udp(buffer));
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