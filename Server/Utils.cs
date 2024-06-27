using System.Net;

namespace YuchiGames.POM.Server
{
    public static class Utils
    {
        public static bool IsConnected(IPEndPoint iPEndPoint)
        {
            for (int i = 0; i < Program.UserData.Length; i++)
            {
                if (Program.UserData[i] == default)
                    continue;
                if (Program.UserData[i].Address.Equals(iPEndPoint.Address))
                {
                    return true;
                }
            }
            return false;
        }
    }
}