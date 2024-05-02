using System.Net;
using System.Net.Sockets;
using Serilog;
using Server.Commands;

namespace Server.Network
{
    public static class Tcp
    {
        public static IPEndPoint[] iPEndPoints = new IPEndPoint[Program.settings!.maxPlayer];

        public static void Listener(object state)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Program.settings!.port);

            try
            {
                listener.Start();
                Log.Information("Server started on {0}", Program.settings.port);
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Log.Information("Client connected from {0}", client.Client.RemoteEndPoint);
                    ThreadPool.QueueUserWorkItem(Client!, client);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

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

                    switch (Serializer.Deserialize(bytes))
                    {
                        case Connect connect:
                            Log.Information("Connect : {0}", connect.UserName);
                            for (int i = 0; i < iPEndPoints!.Length; i++)
                            {
                                if (iPEndPoints[i] == default)
                                {
                                    iPEndPoints[i] = remoteEndPoint;
                                    ThreadPool.QueueUserWorkItem(Udp.Client!, remoteEndPoint);
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
        public static void Listener(object state)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, Program.settings!.port);

            try
            {
                using (UdpClient udpClient = new UdpClient(iPEndPoint))
                {
                    while (true)
                    {
                        iPEndPoint = new IPEndPoint(IPAddress.Any, Program.settings!.port);

                        byte[] receivedData = udpClient.Receive(ref iPEndPoint);
                        ThreadPool.QueueUserWorkItem(Client!, new object[] { iPEndPoint, receivedData, });
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
            object[] args = (object[])state!;
            IPEndPoint iPEndPoint = (IPEndPoint)args[0]!;
            byte[] receivedData = (byte[])args[1]!;

            try
            {

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

    }
}
