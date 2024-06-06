using System.Net;
using Serilog;
using YuchiGames.POM.Server.Data;
using YuchiGames.POM.Server.Network;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Connect
    {
        public static ITcpMessage Client(ConnectMessage connectMessage, IPEndPoint remoteEndPoint)
        {
            try
            {
                if (Utils.ContainAddress(remoteEndPoint))
                {
                    throw new Exception($"Already connected to {remoteEndPoint}.");
                }

                int[] idList = new int[Program.Settings.MaxPlayer];
                int yourID = 0;

                for (int i = 0; i < Program.UserData.Length; i++)
                {
                    if (Program.UserData[i] == default)
                    {
                        yourID = i + 1;
                        Program.UserData[i] = new UserData(yourID, connectMessage.UserName, remoteEndPoint);
                        Log.Information("Connected to {0}.", remoteEndPoint);
                        break;
                    }
                }

                for (int i = 0; i < Program.UserData.Length; i++)
                {
                    if (Program.UserData[i] != default)
                    {
                        idList[i] = i + 1;
                    }
                }

                return new SuccessConnectionMessage(yourID, idList);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new FailureMessage(e);
            }
        }
    }
}