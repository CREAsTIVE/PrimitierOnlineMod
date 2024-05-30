using System.Net;
using Serilog;
using YuchiGames.POM.Server.Network.Utilities;
using YuchiGames.POM.Server.Data.TcpMessages;
using MessagePack;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Connect
    {
        public static byte[] Client(ConnectMessage connectMessage, IPEndPoint remoteEndPoint)
        {
            try
            {
                if (Utils.ContainAddress(remoteEndPoint))
                {
                    throw new Exception($"Already connected to {remoteEndPoint}.");
                }

                int[] idList = new int[Program.settings!.MaxPlayer];
                int idListIndex = 0;
                int yourID = 0;

                for (int i = 0; i < Program.userData!.Length; i++)
                {
                    if (Program.userData[i] == default)
                    {
                        Program.userData[i] = new UserData(connectMessage.UserName, remoteEndPoint);
                        yourID = i;
                        Log.Information("Connected to {0}.", remoteEndPoint);
                        break;
                    }
                }

                for (int i = 0; i < Program.userData.Length; i++)
                {
                    if (Program.userData[i] != default)
                    {
                        idList[idListIndex] = i;
                        idListIndex++;
                    }
                }

                byte[] buffer = Utils.AddLength(MessagePackSerializer.Serialize(new SuccessConnectionMessage(yourID, idList)));
                Log.Debug($"buffer.Length: {buffer.Length}");

                return buffer;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return MessagePackSerializer.Serialize(new FailureMessage(e));
            }
        }
    }
}