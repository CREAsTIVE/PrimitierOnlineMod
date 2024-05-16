using System.Net;
using System.Net.Sockets;
using Serilog;
using YuchiGames.POM.Server.Data.Messages;
using YuchiGames.POM.Server.Data.Serialization;
using YuchiGames.POM.Server.Network.Utilities;

namespace YuchiGames.POM.Server.Network.Process
{
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
                            for (int i = 0; i < Listeners.Tcp.iPEndPoints.Length; i++)
                            {
                                if (Listeners.Tcp.iPEndPoints[i] == default)
                                {
                                    continue;
                                }
                                if (!Listeners.Tcp.iPEndPoints[i].Address.Equals(remoteEndPoint.Address))
                                {
                                    client.Send(receivedData, receivedData.Length, Listeners.Tcp.iPEndPoints[i]);
                                    Log.Information("Sent data to {0}.", Listeners.Tcp.iPEndPoints[i]);
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