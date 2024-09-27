using Serilog;
using System.Text;
using System.Text.Json;
using System.Reflection;
using YuchiGames.POM.Shared;
using YuchiGames.POM.Server.Managers;
using Microsoft.Extensions.Configuration;

namespace YuchiGames.POM.Server
{
    static class Program
    {
        public static ServerSettings Settings { get; private set; } = new();
        public static string Version { get; private set; } = "";

        private static bool s_isCancelled = false;

        // public static Network Network { get; private set; } = new();

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
                        Settings,
                        options);
                    stream.Write(Encoding.UTF8.GetBytes(json));
                }
            }
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();
            Settings = config.Get<ServerSettings>()
                ?? throw new Exception("Settings not found.");
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Version = (Assembly.GetExecutingAssembly().GetName().Version
                ?? throw new Exception("Version not found."))
                .ToString();

            s_isCancelled = false;

            Network.Start(Settings.Port);
        }
    }
}