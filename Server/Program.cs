using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Server
{
    class Settings
    {
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public string version { get; set; } = "0.0.0";
        public int port { get; set; } = 54162;
        public int maxPlayer { get; set; } = 10;
    }

    class Program
    {
        static Settings? settings;
        IPEndPoint[]? iPEndPoints;
        
        static void Main(string[] args)
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build();
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

            try
            {
                listener.Start();
                Log.Information("Server started on {0}", settings.port);
                while (true)
                {
                    WaitCallback callback = new WaitCallback(Tcp.Client!);
                    object? obj = new object[] { listener.AcceptTcpClient(), iPEndPoints };
                    ThreadPool.QueueUserWorkItem(callback, obj);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}