using System.Net;
using System.Net.Sockets;
using Serilog;
using YuchiGames.POM.DataTypes;
using YuchiGames.POM.Server.MessageMethods;
using MessagePack;

namespace YuchiGames.POM.Server.Network
{
    public static class Clients
    {
        public static void Tcp(TcpClient client)
        {
            Log.Debug("Created new Tcp Client thread. ThreadID: {0}", Environment.CurrentManagedThreadId);

            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    if (client.Client.RemoteEndPoint is null)
                        throw new Exception("RemoteEndPoint not found.");
                    IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;

                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);

                    ITcpMessage message;
                    switch (MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                    {
                        case ConnectMessage connect:
                            message = Connect.Process(connect, remoteEndPoint);
                            break;
                        case DisconnectMessage disconnect:
                            message = Disconnect.Process(remoteEndPoint);
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

            Log.Debug("Closed Tcp Client thread. ThreadID: {0}", Environment.CurrentManagedThreadId);
        }

        public static void Udp(UdpReceiveResult result)
        {
            IPEndPoint remoteEndPoint = result.RemoteEndPoint;
            byte[] receivedData = result.Buffer;

            Log.Debug("Created new Udp Client thread. ThreadID: {0}", Environment.CurrentManagedThreadId);

            try
            {
                using (UdpClient client = new UdpClient())
                {
                    IUdpMessage udpMessage = MessagePackSerializer.Deserialize<IUdpMessage>(receivedData);

                    for (int i = 0; i < Program.UserData.Length; i++)
                    {
                        if (Program.UserData[i] == default)
                            continue;
                        if (
                            Program.UserData[i].Address.Equals(remoteEndPoint.Address) &&
                            Program.UserData[i].ID == udpMessage.ID
                            )
                        {
                            break;
                        }
                        if (i == Program.UserData.Length - 1)
                        {
                            throw new Exception($"Not connected to {remoteEndPoint}.");
                        }
                    }

                    switch (udpMessage)
                    {
                        case SendPlayerPosMessage:
                            for (int i = 0; i < Program.UserData.Length; i++)
                            {
                                if (Program.UserData[i] == default)
                                    continue;
                                if (!Program.UserData[i].Address.Equals(remoteEndPoint.Address))
                                {
                                    client.SendAsync(receivedData, receivedData.Length, new IPEndPoint(Program.UserData[i].Address, Program.UserData[i].Port));
                                    Log.Information("Sent data to {0}.", Program.UserData[i].Address);
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

            Log.Debug("Closed Udp Client thread. ThreadID: {0}", Environment.CurrentManagedThreadId);
        }
    }
}