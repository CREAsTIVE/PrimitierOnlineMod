using System.Net;
using System.Net.Sockets;
using Serilog;

namespace Server
{
    public static class Network
    {
        public static IPEndPoint[] iPEndPoints = new IPEndPoint[Program.settings!.maxPlayer];
    }

    public static class Tcp
    {
        public static void Client(object state)
        {
            try
            {
                using (TcpClient client = (TcpClient)state!)
                using (NetworkStream stream = client.GetStream())
                {
                    IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
                    Log.Information("Client connected from {0}:{1}", remoteEndPoint.Address, remoteEndPoint.Port);
                    byte[] bytes = new byte[256];

                    stream.Read(bytes, 0, bytes.Length);
                    Log.Information("Received {0} bytes", bytes.Length);

                    switch (Commands.Deserialize(bytes))
                    {
                        case Connect connect:
                            Log.Information("Connect : {0}", connect.UserName);
                            for (int i = 0; i < Network.iPEndPoints!.Length; i++)
                            {
                                if (Network.iPEndPoints[i] == default)
                                {
                                    Network.iPEndPoints[i] = remoteEndPoint;
                                    break;
                                }
                            }
                            Log.Warning("Server is full.");
                            break;
                        case Disconnect disconnect:
                            Log.Information("Disconnect: {0}", disconnect);
                            for (int i = 0; i < Network.iPEndPoints!.Length; i++)
                            {
                                if (Network.iPEndPoints[i].Address == remoteEndPoint.Address)
                                {
                                    Network.iPEndPoints[i] = default!;
                                    break;
                                }
                            }
                            break;
                        case Error error:
                            Log.Error("Error: {0}", error.ExceptionMessage.Message);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }

    public static class Udp
    {
        public static void Listener(object? state)
        {
            IPEndPoint iPEndPoint = (IPEndPoint)state!;
            UdpClient udpClient = new UdpClient();
            byte[] receivedData = udpClient.Receive(ref iPEndPoint);
            
            try
            {
                foreach (IPEndPoint endPoint in Network.iPEndPoints!)
                {
                    if (endPoint != null)
                    {
                        udpClient.Send(receivedData, receivedData.Length, endPoint);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public static void Client(object? state)
        {
            IPEndPoint iPEndPoint = (IPEndPoint)state!;
            UdpClient udpClient = new UdpClient();
            byte[] receivedData = udpClient.Receive(ref iPEndPoint);
            
            try
            {
                foreach (IPEndPoint endPoint in Network.iPEndPoints!)
                {
                    if (endPoint != null)
                    {
                        udpClient.Send(receivedData, receivedData.Length, endPoint);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
