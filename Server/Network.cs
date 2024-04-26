using System.Net;
using System.Net.Sockets;
using Serilog;

namespace Server
{
    public static class Tcp
    {
        public static void Client(object state)
        {
            object?[]? obj = (object[])state;
            IPEndPoint[] iPEndPoints = (IPEndPoint[])obj[1]!;

            try
            {
                using (TcpClient client = (TcpClient)obj[0]!)
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
                            for (int i = 0; i < iPEndPoints!.Length; i++)
                            {
                                if (iPEndPoints[i] == default)
                                {
                                    iPEndPoints[i] = remoteEndPoint;
                                    break;
                                }
                            }
                            Log.Warning("Server is full.");
                            break;
                        case Disconnect disconnect:
                            Log.Information("Disconnect: {0}", disconnect);
                            for (int i = 0; i < iPEndPoints!.Length; i++)
                            {
                                if (iPEndPoints[i].Address == remoteEndPoint.Address)
                                {
                                    iPEndPoints[i] = default!;
                                    break;
                                }
                            }
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
        public static void Listener(object? state) {
            object[] obj = (object[])state!;
            IPEndPoint iPEndPoint = (IPEndPoint)obj[0]!;
            IPEndPoint[] iPEndPoints = (IPEndPoint[])obj[1]!;
            UdpClient udpClient = new UdpClient();
            byte[] receivedData = udpClient.Receive(ref iPEndPoint);
            
            try
            {
                foreach (IPEndPoint endPoint in iPEndPoints!)
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

        public static void UdpClient(object? state)
        {
            object[] obj = (object[])state!;
            IPEndPoint iPEndPoint = (IPEndPoint)obj[0]!;
            IPEndPoint[] iPEndPoints = (IPEndPoint[])obj[1]!;
            UdpClient udpClient = new UdpClient();
            byte[] receivedData = udpClient.Receive(ref iPEndPoint);
            
            try
            {
                foreach (IPEndPoint endPoint in iPEndPoints!)
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
