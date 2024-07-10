using Microsoft.Extensions.Configuration;
using Serilog;
using YuchiGames.POM.DataTypes;
using YuchiGames.POM.Server.Network;

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
            string path = "settings.json";
            if (!File.Exists(path))
                throw new FileNotFoundException();
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();
            s_settings = config.Get<ServerSettings>();
            if (s_settings is null)
                throw new Exception("Settings not found.");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Listener.Start();
        }
    }
}