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
                for (int i = 0; i < Program.UserData.Length; i++)
                {
                    if (Program.UserData[i] == default)
                    {
                        continue;
                    }
                    if (Program.UserData[i].EndPoint.Address.Equals(iPEndPoint.Address))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}