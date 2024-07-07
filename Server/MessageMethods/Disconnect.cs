using System.Net;
using Serilog;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Disconnect
    {
        public static ITcpMessage Process(IPEndPoint remoteEndPoint)
        {
            try
            {
                if (!Utils.IsConnected(remoteEndPoint))
                {
                    throw new Exception($"Not connected to {remoteEndPoint}.");
                }

                lock (Program.LockUserData)
                {
                    for (int i = 0; i < Program.UserData.Length; i++)
                    {
                        if (Program.UserData[i].Address.Equals(remoteEndPoint.Address))
                        {
                            Program.UserData[i] = default!;
                            Log.Information("Disconnected from {0}.", remoteEndPoint);
                            break;
                        }
                    }
                }

                return new SuccessMessage();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new FailureMessage(e);
            }
        }
    }
}