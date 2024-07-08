using Microsoft.Extensions.Configuration;
using Serilog;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Server
{
    static class Program
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

        private static void Main()
        {
            try
            {
                if (!File.Exists("settings.json"))
                    throw new FileNotFoundException();
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build();
                s_settings = config.Get<ServerSettings>();
                if (s_settings is null)
                    throw new Exception("Settings not found.");

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}