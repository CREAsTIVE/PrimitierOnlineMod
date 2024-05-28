using System.Net;
using YuchiGames.POM.Server.Network.Utilities;
using YuchiGames.POM.Server.Network.Listeners;
using Serilog;
using YuchiGames.POM.Server.Data.Serialization;
using YuchiGames.POM.Server.Data.TcpMessages;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Disconnect
    {
        public static byte[] Client(IPEndPoint remoteEndPoint)
        {
            try
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

                return MethodsSerializer.Serialize(new SuccessMessage());
            } catch (Exception e)
            {
                Log.Error(e.Message);
                return MethodsSerializer.Serialize(new FailureMessage(e));
            }
        }
    }
}