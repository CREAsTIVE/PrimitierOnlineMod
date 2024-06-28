using MelonLoader;
using MessagePack;
using System.Net;
using System.Net.Sockets;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Network
{
    public static class Clients
    {
        public static void Tcp(TcpClient client)
        {
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

                    switch (MessagePackSerializer.Deserialize<ITcpMessage>(buffer))
                    {
                        case JoinedMessage connect:
                            // Connect.Process(connect, remoteEndPoint);
                            break;
                        default:
                            throw new Exception($"Received unknown message from {remoteEndPoint}.");
                    }
                }
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }
        }

        public static void Udp(UdpReceiveResult result)
        {
            IPEndPoint remoteEndPoint = result.RemoteEndPoint;
            byte[] receivedData = result.Buffer;

            try
            {
                switch (MessagePackSerializer.Deserialize<IUdpMessage>(receivedData))
                {
                    case SendPlayerPosMessage playerPos:
                        // SendPlayerPos.Process(playerPos, remoteEndPoint);
                        break;
                    default:
                        throw new Exception($"Received unknown message from {remoteEndPoint}.");
                }
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e.Message);
            }
        }
    }
}