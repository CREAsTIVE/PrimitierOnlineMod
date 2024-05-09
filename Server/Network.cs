using System.Net;
using System.Net.Sockets;
using Serilog;
using YuchiGames.POM.Server.Data.Serialization;
using YuchiGames.POM.Server.Data.Methods;

namespace YuchiGames.POM.Server.Network
{
    public static class Tcp
    {
        public static IPEndPoint[] iPEndPoints = new IPEndPoint[Program.settings!.MaxPlayer];

        public static void Listener()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Program.settings!.Port);

            try
            {
                listener.Start();
                Log.Information("Tcp server started on port {0}.", Program.settings!.Port);

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(Client!, client);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                listener.Stop();
            }
        }

        public static void Client(object state)
        {
            Log.Debug("Created new Tcp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);

            try
            {
                using (TcpClient client = (TcpClient)state!)
                using (NetworkStream stream = client.GetStream())
                {
                    IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
                    byte[] bytes = new byte[64];

                    stream.Read(bytes, 0, bytes.Length);

                    switch (CommandsSerializer.Deserialize(bytes))
                    {
                        case Connect connect:
                            if (Utils.ContainAddress(remoteEndPoint))
                            {
                                Log.Error("Already connected to {0}.", remoteEndPoint);
                                Log.Debug("Closed Tcp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
                                return;
                            }

                            for (int i = 0; i < iPEndPoints!.Length; i++)
                            {
                                if (iPEndPoints[i] == default)
                                {
                                    iPEndPoints[i] = remoteEndPoint;
                                    Log.Information("Connected to {0}.", remoteEndPoint);
                                    break;
                                }
                            }
                            break;
                        case Disconnect disconnect:
                            if (!Utils.ContainAddress(remoteEndPoint))
                            {
                                Log.Error("Not connected to {0}.", remoteEndPoint);
                                Log.Debug("Closed Tcp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
                                return;
                            }

                            for (int i = 0; i < iPEndPoints!.Length; i++)
                            {
                                if (iPEndPoints[i] == remoteEndPoint)
                                {
                                    iPEndPoints[i] = default!;
                                    Log.Information("Disconnected from {0}.", remoteEndPoint);
                                    break;
                                }
                            }
                            break;
                        case Error error:
                            Log.Error("Received error: {0} to {1}.", error.ExceptionMessage.Message, remoteEndPoint);
                            break;
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
        public static void Listener()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, Program.settings!.Port);

            try
            {
                using (UdpClient udpClient = new UdpClient(iPEndPoint))
                {
                    Log.Information("Udp server started on port {0}.", Program.settings!.Port);
                    
                    while (true)
                    {
                        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] receivedData = udpClient.Receive(ref remoteEndPoint);
                        ThreadPool.QueueUserWorkItem(Client!, new object[] { iPEndPoint, receivedData });
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static void Client(object? state)
        {
            object[] args = (object[])state!;
            IPEndPoint iPEndPoint = (IPEndPoint)args[0]!;
            byte[] receivedData = (byte[])args[1]!;

            Log.Debug("Created new Udp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);

            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    if (!Utils.ContainAddress(iPEndPoint))
                    {
                        Log.Error("Not connected to {0}.", iPEndPoint);
                        Log.Debug("Closed Udp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
                        return;
                    }
                    for (int i = 0; i < Tcp.iPEndPoints.Length; i++)
                    {
                        if (Tcp.iPEndPoints[i] == default)
                        {
                            continue;
                        }
                        if (!Tcp.iPEndPoints[i].Address.Equals(iPEndPoint.Address))
                        {
                            udpClient.Send(receivedData, receivedData.Length, Tcp.iPEndPoints[i]);
                            Log.Information("Sent data to {0}.", Tcp.iPEndPoints[i]);
                        }
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

    public static class Utils
    {
        public static bool ContainAddress(IPEndPoint iPEndPoint)
        {
            try
            {
                for (int i = 0; i < Tcp.iPEndPoints.Length; i++)
                {
                    if (Tcp.iPEndPoints[i] == default)
                    {
                        continue;
                    }
                    if (Tcp.iPEndPoints[i].Address.Equals(iPEndPoint.Address))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }
    }
}
