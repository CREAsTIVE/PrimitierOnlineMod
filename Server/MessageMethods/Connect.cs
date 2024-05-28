using System.Net;
using Serilog;
using YuchiGames.POM.Server.Network.Utilities;
using YuchiGames.POM.Server.Network.Listeners;
using YuchiGames.POM.Server.Data.Serialization;
using YuchiGames.POM.Server.Data.TcpMessages;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Connect
    {
        public static byte[] Client(IPEndPoint remoteEndPoint)
        {
            try
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

                return MethodsSerializer.Serialize(new SuccessConnectionMessage("yourID", new string[0]));
            } catch (Exception e)
            {
                Log.Error(e.Message);
                return MethodsSerializer.Serialize(new FailureMessage(e));
            }
        }
    }
}