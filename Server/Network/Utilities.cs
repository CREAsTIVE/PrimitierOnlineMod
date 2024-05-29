using System.Net;
using Serilog;

namespace YuchiGames.POM.Server.Network.Utilities
{
    public static class Utils
    {
        public static bool ContainAddress(IPEndPoint iPEndPoint)
        {
            try
            {
                for (int i = 0; i < Program.userData!.Length; i++)
                {
                    if (Program.userData[i] == default)
                    {
                        continue;
                    }
                    if (Program.userData[i].EndPoint.Address.Equals(iPEndPoint.Address))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }
    }
}