using System.Net;
using Serilog;
using YuchiGames.POM.Server.Network.Utilities;
using YuchiGames.POM.Server.Data;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Connect
    {
        public static ITcpMessage Client(ConnectMessage connectMessage, IPEndPoint remoteEndPoint)
        {
            try
            {
                if (Program.settings is null)
                    throw new Exception("Settings not found.");
                if (Program.userData is null)
                    throw new Exception("UserData not found.");

                if (Utils.ContainAddress(remoteEndPoint))
                {
                    throw new Exception($"Already connected to {remoteEndPoint}.");
                }

                int[] idList = new int[Program.settings.MaxPlayer];
                int yourID = 0;

                for (int i = 0; i < Program.userData.Length; i++)
                {
                    if (Program.userData[i] == default)
                    {
                        yourID = i + 1;
                        Program.userData[i] = new UserData(yourID, connectMessage.UserName, remoteEndPoint);
                        Log.Information("Connected to {0}.", remoteEndPoint);
                        break;
                    }
                }

                for (int i = 0; i < Program.userData.Length; i++)
                {
                    if (Program.userData[i] != default)
                    {
                        idList[i] = i + 1;
                    }
                }

                return new SuccessConnectionMessage(yourID, idList); ;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new FailureMessage(e);
            }
        }
    }
}