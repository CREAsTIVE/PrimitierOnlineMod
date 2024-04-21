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
    
    static void Main() {
        try {
            config = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build();
            settings = new Settings();
            settings = config.Get<Settings>();
        } catch (Exception e) {
            ServerConsole.WriteLine(e.ToString());
            Environment.Exit(1);
        }
        new Program().Server();
    }

    void Server() {
        iPEndPoints = new IPEndPoint[settings!.maxPlayer];
        TcpListener listener = new TcpListener(IPAddress.Any, settings!.port);
        
        listener.Start();
        while (true) {
            TcpClient client = listener.AcceptTcpClient();
            IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
            if (!iPEndPoints.Contains(remoteEndPoint)) {
                ThreadPool.QueueUserWorkItem(TcpClient, client);
                ServerConsole.WriteLine($"Client connected: {remoteEndPoint}");
            }
        }
    }

    void TcpClient(object? obj) {
        TcpClient client = (TcpClient)obj!;
    }

    void UdpClient(object? obj) {
        UdpClient client = (UdpClient)obj!;
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