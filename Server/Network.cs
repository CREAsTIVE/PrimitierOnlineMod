using System.Net;
using System.Net.Sockets;
using Serilog;
using YuchiGames.POM.Server.Data.Methods;
using YuchiGames.POM.Server.Serialization;

namespace YuchiGames.POM.Server.Network
{
    public static class Tcp
    {
        public static IPEndPoint[] iPEndPoints = new IPEndPoint[Program.settings!.MaxPlayer];

        public static async void Listener()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Program.settings!.Port);

            try
            {
                listener.Start();
                Log.Information("Tcp server started on port {0}.", Program.settings!.Port);

                while (true)
                {
                    await ClientAsync(await listener.AcceptTcpClientAsync());
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task ClientAsync(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
                    byte[] bytes = new byte[256];

                    await stream.ReadAsync(bytes, 0, bytes.Length);

                    switch (CommandsSerializer.Deserialize(bytes))
                    {
                        case Connect connect:
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
                            Log.Error(error.ExceptionMessage.Message);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }

    public static class Udp
    {
        public static async void Listener()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, Program.settings!.Port);

            try
            {
                using (UdpClient udpClient = new UdpClient(iPEndPoint))
                {
                    Log.Information("Udp server started on port {0}.", Program.settings!.Port);
                    
                    while (true)
                    {
                        await ClientAsync(await udpClient.ReceiveAsync());
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task ClientAsync(UdpReceiveResult result)
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    bool hasAddressContained = false;

                    for (int i = 0; i < Tcp.iPEndPoints.Length; i++)
                    {
                        if (Tcp.iPEndPoints[i].Address == result.RemoteEndPoint.Address)
                        {
                            hasAddressContained = true;
                            break;
                        }
                    }
                    if (!hasAddressContained)
                    {
                        return;
                    }

                    for (int i = 0; i < Tcp.iPEndPoints.Length; i++)
                    {
                        if (Tcp.iPEndPoints[i].Address != result.RemoteEndPoint.Address && hasAddressContained)
                        {
                            udpClient.Send(result.Buffer, result.Buffer.Length, Tcp.iPEndPoints[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

    }
}
