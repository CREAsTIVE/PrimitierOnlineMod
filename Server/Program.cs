using System.Net;
using Microsoft.Extensions.Configuration;
using Serilog;
using YuchiGames.POM.Server.Data;
using YuchiGames.POM.Server.Network;

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
        private static ServerSettings? _settings;
        public static ServerSettings Settings
        {
            get
            {
                if (_settings is null)
                    throw new Exception("Settings not found.");
                return _settings;
            }
        }
        private static UserData[]? _userData;
        public static UserData[] UserData
        {
            get
            {
                if (_userData is null)
                    throw new Exception("User data not found.");
                return _userData;
            }
            set
            {
                _userData = value;
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build();
                _settings = config.Get<ServerSettings>();
                if (_settings is null)
                    throw new Exception("Settings not found.");

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();

                _userData = new UserData[_settings.MaxPlayer];

                Program program = new Program();
                program.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Start()
        {
            Listeners listeners = new Listeners();
            Thread tcpThread = new Thread(listeners.Tcp);
            Thread udpThread = new Thread(listeners.Udp);
            tcpThread.Start();
            udpThread.Start();
        }
    }
}