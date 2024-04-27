using Microsoft.Extensions.Configuration;
using Serilog;
using Server.Network;

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
        public static Settings? settings;
        
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

                Tcp.Listener(null!);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}