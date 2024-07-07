using System.Net;
using System.Reflection;
using Serilog;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Server.MessageMethods
{
    public class Connect
    {
        public static ITcpMessage Process(ConnectMessage connectMessage, IPEndPoint remoteEndPoint)
        {
            try
            {
                if (Utils.IsConnected(remoteEndPoint))
                {
                    throw new Exception($"Already connected to {remoteEndPoint}.");
                }
                AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
                if (assemblyName.Version is null)
                    throw new Exception("Assembly version is null");
                Version version = assemblyName.Version;
                if (connectMessage.Version != version.ToString())
                    throw new Exception($"Version mismatch. Server version: {version.ToString()}, Client version: {connectMessage.Version}.");

                int yourID = 0;
                lock (Program.LockUserData)
                {
                    for (int i = 0; i < Program.UserData.Length; i++)
                    {
                        if (Program.UserData[i] == default)
                        {
                            yourID = i + 1;
                            Program.UserData[i] = new UserData(yourID, remoteEndPoint.Address, connectMessage.Port);
                            Log.Information("Connected to {0}.", remoteEndPoint);
                            break;
                        }
                    }
                }

                int[] idList = new int[Program.Settings.MaxPlayer];
                int j = 0;
                for (int i = 0; i < Program.UserData.Length; i++)
                {
                    if (Program.UserData[i] != default)
                    {
                        idList[j] = Program.UserData[i].ID;
                        j++;
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