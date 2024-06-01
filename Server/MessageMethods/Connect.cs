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
                int yourID = 0;

                for (int i = 0; i < Program.userData!.Length; i++)
                {
                    if (Program.userData[i] == default)
                    {
                        yourID = i + 1;
                        Program.userData[i] = new UserData(yourID, connectMessage.UserName, remoteEndPoint);
                        Log.Information("Connected to {0}.", remoteEndPoint);
                        break;
                    }
                }

                for (int i = 0; i < Program.userData.Length; i++)
                {
                    if (Program.userData[i] != default)
                    {
                        idList[i] = i + 1;
                    }
                }

                IMessage message = new SuccessConnectionMessage(yourID, idList);
                byte[] data = MessagePackSerializer.Serialize(message);
                byte[] buffer = Utils.AddLength(data);
                Log.Debug($"Server send buffer size: {data.Length}");
                Log.Debug($"Server send buffer: {BitConverter.ToString(data)}");
                Log.Debug($"Server json: {MessagePackSerializer.ConvertToJson(data)}");

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