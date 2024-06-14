using System.Net;
using System.Net.Sockets;
using Serilog;
using YuchiGames.POM.Data;
using YuchiGames.POM.Server.MessageMethods;
using MessagePack;

namespace YuchiGames.POM.Server.Network
{
    public class Clients
    {
        public void Tcp(TcpClient client)
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

                    int bufferLength = 1024;
                    byte[] buffer = new byte[bufferLength];
                    stream.Read(buffer, 0, bufferLength);

                    ITcpMessage message;
                    switch (MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                    {
                        case ConnectMessage connect:
                            message = new Connect().Process(connect, remoteEndPoint);
                            break;
                        case DisconnectMessage disconnect:
                            message = new Disconnect().Process(remoteEndPoint);
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

        public void Udp(UdpReceiveResult result)
        {
            IPEndPoint remoteEndPoint = result.RemoteEndPoint;
            byte[] receivedData = result.Buffer;

            Log.Debug("Created new Udp Client thread. ThreadID: {0}", Environment.CurrentManagedThreadId);

            try
            {
                using (UdpClient client = new UdpClient())
                {
                    if (!Utils.ContainAddress(remoteEndPoint))
                    {
                        throw new Exception($"Not connected to {remoteEndPoint}.");
                    }

                    switch (MessagePackSerializer.Deserialize<IUdpMessage>(receivedData))
                    {
                        case SendPlayerPosMessage sendPlayerPosMessage:
                            for (int i = 0; i < Program.UserData.Length; i++)
                            {
                                if (Program.UserData[i] == default)
                                {
                                    continue;
                                }
                                if (!Program.UserData[i].EndPoint.Address.Equals(remoteEndPoint.Address))
                                {
                                    client.Send(receivedData, receivedData.Length, Program.UserData[i].EndPoint);
                                    Log.Information("Sent data to {0}.", Program.UserData[i].EndPoint);
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