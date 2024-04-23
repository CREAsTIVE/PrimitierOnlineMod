using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Configuration;

class Settings {
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public string version { get; set; } = "0.0.0";
    public string ip { get; set; } = "";
    public int port { get; set; } = 54162;
    public int maxPlayer { get; set; } = 10;
}

[Serializable]
class Command {
    public string Name { get; set; } = "";
    public object[] Args { get; set; } = new object[] {};

    public Command(string name, params object[] args) {
        Name = name;
        Args = args;
    }
}

class Program {
    static IConfigurationRoot? config;
    static Settings? settings;
    IPEndPoint[]? iPEndPoints;
    
    static void Main(string[] args) {
        try
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build();
            settings = new Settings();
            settings = config.Get<Settings>();
            new Program().MainServer();
        }
        catch (Exception e)
        {
            ServerConsole.WriteLine(e.ToString());
        }
    }

    void MainServer()
    {
        iPEndPoints = new IPEndPoint[settings!.maxPlayer];
        TcpListener listener = new TcpListener(IPAddress.Any, settings.port);

        listener.Start();
        while (true)
        {
            ThreadPool.QueueUserWorkItem(TcpClient, listener.AcceptTcpClient());
        }
    }

    void TcpClient(object? obj) {
        try {
            using (TcpClient client = (TcpClient)obj!) {
                IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
                using (NetworkStream stream = client.GetStream()) {
                    
                }
            }
        } catch (Exception e) {
            ServerConsole.WriteLine(e.ToString());
        }
    }

    void UdpClient(object? obj) {
        IPEndPoint iPEndPoint = (IPEndPoint)obj!;
        UdpClient udpClient = new UdpClient();
        byte[] receivedData = udpClient.Receive(ref iPEndPoint);
        
        foreach (IPEndPoint endPoint in iPEndPoints!) {
            if (endPoint != null) {
                udpClient.Send(receivedData, receivedData.Length, endPoint);
            }
        }
    }
}

class ServerConsole
{
    internal static void WriteLine(string v)
    {
        Console.WriteLine($"[{DateTime.Now.ToString("hh:mm:ss")}] {v}");
    }
}