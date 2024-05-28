using System.Net;
using Microsoft.Extensions.Configuration;
using Serilog;
using YuchiGames.POM.Server.Data.Settings;
using YuchiGames.POM.Server.Network.Listeners;

namespace YuchiGames.POM.Server
{
    class UserData
    {
        public int ID { get; set; } = 0;
        public string UserName { get; set; } = "";
        public IPEndPoint EndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 0);
    }

    class Program
    {
        public static ServerSettings? settings;
        public static UserData[] userData = new UserData[settings!.MaxPlayer];
        
        static void Main(string[] args)
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build();
                settings = config.Get<ServerSettings>();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();
                
                Program program = new Program();
                program.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        void Start()
        {
            Thread tcpThread = new Thread(Tcp.Listener);
            Thread udpThread = new Thread(Udp.Listener);
            tcpThread.Start();
            udpThread.Start();
        }
    }
}