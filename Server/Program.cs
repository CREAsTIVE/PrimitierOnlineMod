using System.Net;
using Microsoft.Extensions.Configuration;
using Serilog;
using YuchiGames.POM.DataTypes;
using YuchiGames.POM.Server.Network;

namespace YuchiGames.POM.Server
{
    class UserData
    {
        public int ID { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public UserData(int id, IPEndPoint endPoint)
        {
            ID = id;
            EndPoint = endPoint;
        }
    }

    class Program
    {
        private static ServerSettings? s_settings;
        public static ServerSettings Settings
        {
            get
            {
                if (s_settings is null)
                    throw new Exception("Settings not found.");
                return s_settings;
            }
        }
        private static UserData[]? s_userData;
        public static UserData[] UserData
        {
            get
            {
                if (s_userData is null)
                    throw new Exception("User data not found.");
                return s_userData;
            }
            set
            {
                s_userData = value;
            }
        }
        private static object s_lockUserData = new object();
        public static object LockUserData
        {
            get
            {
                return s_lockUserData;
            }
            set
            {
                s_lockUserData = value;
            }
        }

        private static void Main()
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build();
                s_settings = config.Get<ServerSettings>();
                if (s_settings is null)
                    throw new Exception("Settings not found.");

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();

                s_userData = new UserData[s_settings.MaxPlayer];

                Thread tcpThread = new Thread(Listeners.Tcp);
                Thread udpThread = new Thread(Listeners.Udp);
                tcpThread.Start();
                udpThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}