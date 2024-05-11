using System.Net;
using System.Net.Sockets;
using Serilog;
using YuchiGames.POM.Server.Data.Methods;
using YuchiGames.POM.Server.Data.Serialization;
using YuchiGames.POM.Server.Network.Utilities;

namespace YuchiGames.POM.Server.Network.Process
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
                        case ConnectMethod connect:
                            if (Utils.ContainAddress(remoteEndPoint))
                            {
                                Log.Error("Already connected to {0}.", remoteEndPoint);
                                Log.Debug("Closed Tcp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
                                return;
                            }

                            for (int i = 0; i < Listeners.Tcp.iPEndPoints!.Length; i++)
                            {
                                if (Listeners.Tcp.iPEndPoints[i] == default)
                                {
                                    Listeners.Tcp.iPEndPoints[i] = remoteEndPoint;
                                    Log.Information("Connected to {0}.", remoteEndPoint);
                                    break;
                                }
                            }
                            break;
                        case DisconnectMethod disconnect:
                            if (!Utils.ContainAddress(remoteEndPoint))
                            {
                                Log.Error("Not connected to {0}.", remoteEndPoint);
                                Log.Debug("Closed Tcp Client thread. ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
                                return;
                            }

                            for (int i = 0; i < Listeners.Tcp.iPEndPoints!.Length; i++)
                            {
                                if (Listeners.Tcp.iPEndPoints[i] == remoteEndPoint)
                                {
                                    Listeners.Tcp.iPEndPoints[i] = default!;
                                    Log.Information("Disconnected from {0}.", remoteEndPoint);
                                    break;
                                }
                            }
                            break;
                        case SuccessMethod success:
                            Log.Information("Received success: {0}.", remoteEndPoint);
                            break;
                        case FailureMethod error:
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
}