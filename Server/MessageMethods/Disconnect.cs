using System.Net;
using Serilog;
using YuchiGames.POM.Server.Network;
using YuchiGames.POM.Server.Data;

namespace YuchiGames.POM.Server.MessageMethods
{
    public class Disconnect
    {
        public ITcpMessage Process(IPEndPoint remoteEndPoint)
        {
            try
            {
                if (!Utils.ContainAddress(remoteEndPoint))
                {
                    throw new Exception($"Not connected to {remoteEndPoint}.");
                }

                for (int i = 0; i < Program.UserData.Length; i++)
                {
                    if (Program.UserData[i].EndPoint == remoteEndPoint)
                    {
                        Program.UserData[i] = default!;
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