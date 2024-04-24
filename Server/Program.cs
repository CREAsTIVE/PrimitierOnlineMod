using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using Serilog;

class Settings {
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public string version { get; set; } = "0.0.0";
    public string ip { get; set; } = "";
    public int port { get; set; } = 54162;
    public int maxPlayer { get; set; } = 10;
}

[DataContract]
class Command {
    [DataMember]
    public string Name { get; set; } = "";
    [DataMember]
    public object[] Args { get; set; } = new object[] {};

    public Command(string name, params object[] args) {
        Name = name;
        Args = args;
    }
}

class Program {
    static Settings? settings;
    IPEndPoint[]? iPEndPoints;
    
    static void Main(string[] args) {
        try
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build();
            settings = new Settings();
            settings = config.Get<Settings>();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            new Program().MainServer();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }

    void MainServer()
    {
        iPEndPoints = new IPEndPoint[settings!.maxPlayer];
        TcpListener listener = new TcpListener(IPAddress.Any, settings.port);

        Log.Information("Server started on {0}:{1}", settings.ip, settings.port);
        try
        {
            listener.Start();
            while (true)
            {
                ThreadPool.QueueUserWorkItem(TcpClient, listener.AcceptTcpClient());
            }
        } catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }

    void TcpClient(object? obj) {
        try {
            using (TcpClient client = (TcpClient)obj!)
            using (NetworkStream stream = client.GetStream())
            {
                IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
                byte[] buffer = new byte[4];
                byte[] bytes;

                stream.Read(buffer, 0, buffer.Length);
                bytes = new byte[BitConverter.ToInt32(buffer, 0)];
                stream.Read(bytes, 0, bytes.Length);

                DataContractSerializer serializer = new DataContractSerializer(typeof(Command));
                Command command = (Command)serializer.ReadObject(new MemoryStream(bytes))!;

                if (command.Name == "connect")
                {
                    if (iPEndPoints!.Contains(remoteEndPoint)) {
                        
                    }

                    foreach (IPEndPoint endPoint in iPEndPoints!)
                    {
                        if (endPoint == null)
                        {

                        }
                    }
                } else if (command.Name == "disconnect")
                {

                }
            }
        } catch (Exception e) {
            Log.Error(e.ToString());
        }
    }

    void UdpClient(object? obj) {
        IPEndPoint iPEndPoint = (IPEndPoint)obj!;
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
        } catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
}