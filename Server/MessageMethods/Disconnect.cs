using System.Net;
using YuchiGames.POM.Server.Network.Utilities;
using YuchiGames.POM.Server.Network.Listeners;
using Serilog;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Disconnect
    {
        public static void Client(IPEndPoint remoteEndPoint)
        {
            if (!Utils.ContainAddress(remoteEndPoint))
            {
                throw new Exception($"Not connected to {remoteEndPoint}.");
            }

            for (int i = 0; i < Tcp.iPEndPoints!.Length; i++)
            {
                if (Tcp.iPEndPoints[i] == remoteEndPoint)
                {
                    Tcp.iPEndPoints[i] = default!;
                    Log.Information("Disconnected from {0}.", remoteEndPoint);
                    break;
                }
            }
        }
    }
}