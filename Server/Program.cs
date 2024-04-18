using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

class Settings {
    public string? Name { get; set; }
    public string? description { get; set; }
    public string? version { get; set; }
    public string? ip { get; set; }
    public int? port { get; set; }
}

class Program {
    static Settings? settings;
    IPEndPoint[]? iPEndPoints;
    
    static void Main() {
        try {
            settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
            if (settings == null) {
                throw new Exception("Settings is null");
            }
            ServerConsole.WriteLine($"Settings file loaded: {settings.Name}, {settings.description}, {settings.version}, {settings.ip}, {settings.port}");
        } catch (Exception e) {
            ServerConsole.WriteLine(e.ToString());
            Environment.Exit(1);
        }
        new Program().TcpListen();
    }

    void TcpListen() {
        TcpListener listener = new TcpListener(IPAddress.Any, settings?.port ?? 54162);
        listener.Start();

        while (true) {
            TcpClient client = listener.AcceptTcpClient();
            ServerConsole.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
            ThreadPool.QueueUserWorkItem(Client, client);
        }
    }

    void Client(object? obj) {
        TcpClient client = (TcpClient)obj!;
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