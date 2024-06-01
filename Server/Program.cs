using System.Net;
using Microsoft.Extensions.Configuration;
using Serilog;
using YuchiGames.POM.Server.Data.Settings;
using YuchiGames.POM.Server.Network.Listeners;

namespace YuchiGames.POM.Server
{
    class UserData
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public UserData(int id, string userName, IPEndPoint endPoint)
        {
            ID = id;
            UserName = userName;
            EndPoint = endPoint;
        }
    }

    class Program
    {
        public static ServerSettings? settings;
        public static UserData[]? userData;

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

                userData = new UserData[settings!.MaxPlayer];

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