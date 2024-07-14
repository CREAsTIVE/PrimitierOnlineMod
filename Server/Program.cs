﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System.Text;
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
            string path = "./settings.json";
            if (!File.Exists(path))
            {
                using (FileStream stream = File.Create(path))
                {
                    string jsonSource = @"{ ""port"": 54162, ""maxPlayers"": 15, ""maxDataSize"": 67108864, ""Serilog"": { ""Using"": [ ""Serilog.Settings.Configuration"", ""Serilog.Sinks.Console"", ""Serilog.Sinks.File"" ], ""MinimumLevel"": ""Debug"", ""WriteTo"": [ { ""Name"": ""Console"" }, { ""Name"": ""File"", ""Args"": { ""path"": ""Logs/.log"", ""rollingInterval"": ""Day"" } } ] } } ";
                    string json = JsonConvert.SerializeObject(
                        JsonConvert.DeserializeObject(jsonSource),
                        Formatting.Indented);
                    stream.Write(Encoding.UTF8.GetBytes(json));
                }
            }
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();
            s_settings = config.Get<ServerSettings>();
            if (s_settings is null)
                throw new Exception("Settings not found.");
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            NetworkManager.Start();
            while (!Console.KeyAvailable) { }
            NetworkManager.Stop();
        }
    }
}