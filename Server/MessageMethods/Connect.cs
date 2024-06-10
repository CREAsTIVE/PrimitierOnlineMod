using System.Net;
using Serilog;
using YuchiGames.POM.Server.Data;
using YuchiGames.POM.Server.Network;

namespace YuchiGames.POM.Server.MessageMethods
{
    public class Connect
    {
        public ITcpMessage Process(ConnectMessage connectMessage, IPEndPoint remoteEndPoint)
        {
            try
            {
                if (Utils.ContainAddress(remoteEndPoint))
                {
                    throw new Exception($"Already connected to {remoteEndPoint}.");
                }

                if (connectMessage.Version != Program.Settings.Version)
                {
                    throw new Exception($"Version mismatch. Server version: {Program.Settings.Version}, Client version: {connectMessage.Version}.");
                }

                int yourID = 0;
                lock (Program.LockUserData)
                {
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
                }

                int[] idList = new int[Program.Settings.MaxPlayer];
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