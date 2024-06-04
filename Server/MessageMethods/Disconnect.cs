using System.Net;
using Serilog;
using YuchiGames.POM.Server.Network;
using YuchiGames.POM.Server.Data;
using MessagePack;

namespace YuchiGames.POM.Server.MessageMethods
{
    public static class Disconnect
    {
        public static byte[] Client(IPEndPoint remoteEndPoint)
        {
            try
            {
                if (!Utils.ContainAddress(remoteEndPoint))
                {
                    throw new Exception($"Not connected to {remoteEndPoint}.");
                }

                for (int i = 0; i < Program.userData!.Length; i++)
                {
                    if (Program.userData[i].EndPoint == remoteEndPoint)
                    {
                        Program.userData[i] = default!;
                        Log.Information("Disconnected from {0}.", remoteEndPoint);
                        break;
                    }
                }

                ITcpMessage message = new SuccessMessage();
                return Utils.AddLength(MessagePackSerializer.Serialize(message));
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                ITcpMessage message = new FailureMessage(e);
                return Utils.AddLength(MessagePackSerializer.Serialize(message));
            }
        }
    }
}