using System.Net;
using YuchiGames.POM.Server.Network.Utilities;
using YuchiGames.POM.Server.Network.Listeners;
using Serilog;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Connect
    {
        public static void Client(IPEndPoint remoteEndPoint)
        {
            if (Utils.ContainAddress(remoteEndPoint))
            {
                throw new Exception($"Already connected to {remoteEndPoint}.");
            }

            for (int i = 0; i < Tcp.iPEndPoints!.Length; i++)
            {
                if (Tcp.iPEndPoints[i] == default)
                {
                    Tcp.iPEndPoints[i] = remoteEndPoint;
                    Log.Information("Connected to {0}.", remoteEndPoint);
                    break;
                }
            }
        }
    }
}