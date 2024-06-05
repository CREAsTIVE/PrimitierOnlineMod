using System.Net;
using Serilog;
using YuchiGames.POM.Server.Network.Utilities;
using YuchiGames.POM.Server.Data;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Disconnect
    {
        public static ITcpMessage Client(IPEndPoint remoteEndPoint)
        {
            try
            {
                if (Program.userData is null)
                    throw new Exception("UserData not found.");

                if (!Utils.ContainAddress(remoteEndPoint))
                {
                    throw new Exception($"Not connected to {remoteEndPoint}.");
                }

                for (int i = 0; i < Program.userData.Length; i++)
                {
                    if (Program.userData[i].EndPoint == remoteEndPoint)
                    {
                        Program.userData[i] = default!;
                        Log.Information("Disconnected from {0}.", remoteEndPoint);
                        break;
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