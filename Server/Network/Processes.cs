using System.Net;
using System.Net.Sockets;
using Serilog;
using YuchiGames.POM.Server.Data.TcpMessages;
using YuchiGames.POM.Server.Data.UdpMessages;
using YuchiGames.POM.Server.Data.Serialization;
using YuchiGames.POM.Server.Network.Utilities;
using YuchiGames.POM.Server.MessageMethods;

namespace YuchiGames.POM.Server.Network.Processes
{
    public static class Tcp
    {
        public static void Client(TcpClient client)
        {
            Log.Debug("Created new Tcp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);

            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
                    byte[] bytes = new byte[64];

                    stream.Read(bytes, 0, bytes.Length);

                    switch (MethodsSerializer.Deserialize(bytes))
                    {
                        case ConnectMessage connect:
                            stream.Write(Connect.Client(remoteEndPoint));
                            break;
                        case DisconnectMessage disconnect:
                            stream.Write(Disconnect.Client(remoteEndPoint));
                            break;
                        default:
                            throw new Exception($"Received unknown message from {remoteEndPoint}.");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Log.Debug("Closed Tcp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }

    public static class Udp
    {
        public static void Client(UdpReceiveResult result)
        {
            IPEndPoint remoteEndPoint = result.RemoteEndPoint;
            byte[] receivedData = result.Buffer;

            Log.Debug("Created new Udp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);

            try
            {
                using (UdpClient client = new UdpClient())
                {
                    if (!Utils.ContainAddress(remoteEndPoint))
                    {
                        throw new Exception($"Not connected to {remoteEndPoint}.");
                    }

                    switch (MethodsSerializer.Deserialize(receivedData))
                    {
                        case SendPlayerPosMessage sendPlayerPosMessage:
                            for (int i = 0; i < Program.userData.Length; i++)
                            {
                                if (Program.userData[i] == default)
                                {
                                    continue;
                                }
                                if (!Program.userData[i].EndPoint.Address.Equals(remoteEndPoint.Address))
                                {
                                    client.Send(receivedData, receivedData.Length, Program.userData[i].EndPoint);
                                    Log.Information("Sent data to {0}.", Program.userData[i].EndPoint);
                                }
                            }
                            break;
                        default:
                            throw new Exception($"Received unknown message from {remoteEndPoint}.");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Log.Debug("Closed Udp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}