using System.Net;
using System.Net.Sockets;
using Serilog;
using YuchiGames.POM.Server.Data;
using YuchiGames.POM.Server.MessageMethods;
using MessagePack;
using YuchiGames.POM.Server.Network.Utilities;

namespace YuchiGames.POM.Server.Network.Clients
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
                    if (client.Client.RemoteEndPoint is null)
                        throw new Exception("RemoteEndPoint not found.");
                    IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;

                    int bufferLength = 1024;
                    byte[] buffer = new byte[bufferLength];
                    stream.Read(buffer, 0, bufferLength);

                    ITcpMessage message;
                    switch (MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                    {
                        case ConnectMessage connect:
                            message = Connect.Client(connect, remoteEndPoint);
                            break;
                        case DisconnectMessage disconnect:
                            message = Disconnect.Client(remoteEndPoint);
                            break;
                        default:
                            throw new Exception($"Received unknown message from {remoteEndPoint}.");
                    }

                    buffer = MessagePackSerializer.Serialize(message);
                    stream.Write(buffer, 0, buffer.Length);
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
                if (Program.userData is null)
                    throw new Exception("UserData not found.");

                using (UdpClient client = new UdpClient())
                {
                    if (!Utils.ContainAddress(remoteEndPoint))
                    {
                        throw new Exception($"Not connected to {remoteEndPoint}.");
                    }

                    switch (MessagePackSerializer.Deserialize<IUdpMessage>(receivedData))
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