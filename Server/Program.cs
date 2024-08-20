using Serilog;
using System.Text;
using System.Text.Json;
using System.Reflection;
using YuchiGames.POM.DataTypes;
using YuchiGames.POM.Server.Managers;
using Microsoft.Extensions.Configuration;

namespace YuchiGames.POM.Server
{
    static class Program
    {
        private static ServerSettings s_settings;
        public static ServerSettings Settings
        {
            get => s_settings;
        }
        private static string s_version;
        public static string Version
        {
            get => s_version;
        }

        private static bool s_isCancelled;

        static Program()
        {
            s_settings = new ServerSettings();
            s_version = "";
        }

        private static void Main()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                if (!s_isCancelled)
                {
                    s_isCancelled = true;
                    Network.Stop();
                }
                e.Cancel = true;
            };
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                if (!s_isCancelled)
                {
                    Network.Stop();
                }
            };

            string path = "./settings.json";
            if (!File.Exists(path))
            {
                using (FileStream stream = File.Create(path))
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    string json = JsonSerializer.Serialize(
                        s_settings,
                        options);
                    stream.Write(Encoding.UTF8.GetBytes(json));
                }
            }
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();
            s_settings = config.Get<ServerSettings>()
                ?? throw new Exception("Settings not found.");
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            s_version = (Assembly.GetExecutingAssembly().GetName().Version
                ?? throw new Exception("Version not found."))
                .ToString();

            s_isCancelled = false;

            Network.Start(s_settings.Port);
        }
    }
}