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

        public static byte[] AddLength(byte[] data)
        {
            byte[] lengthBytes = new byte[4];
            lengthBytes = BitConverter.GetBytes(data.Length);
            byte[] buffer = new byte[lengthBytes.Length + data.Length];
            Buffer.BlockCopy(lengthBytes, 0, buffer, 0, lengthBytes.Length);
            Buffer.BlockCopy(data, 0, buffer, lengthBytes.Length, data.Length);

            return buffer;
        }
    }
}