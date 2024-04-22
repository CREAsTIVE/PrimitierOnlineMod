using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;

class Settings {
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public string version { get; set; } = "0.0.0";
    public string ip { get; set; } = "";
    public int port { get; set; } = 54162;
    public int maxPlayer { get; set; } = 10;
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
        TcpClient client = (TcpClient)obj!;
        IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
        ServerConsole.WriteLine($"Connected from {remoteEndPoint.Address}:{remoteEndPoint.Port}");
        if (iPEndPoints!.Contains(remoteEndPoint))
        {
            ThreadPool.QueueUserWorkItem(UdpClient, client);
        }
    }

    void UdpClient(object? obj) {
        IPEndPoint iPEndPoint = (IPEndPoint)obj!;
        UdpClient udpClient = new UdpClient();
    }

    string[] SplitCommand(byte[] bytes) {
        return new string[] {"command", "args"};
    }
}

class ServerConsole
{
    internal static void WriteLine(string v)
    {
        Console.WriteLine($"[{DateTime.Now.ToString("hh:mm:ss")}] {v}");
    }
}