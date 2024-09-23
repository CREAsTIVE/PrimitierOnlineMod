using MelonLoader;
using UnityEngine;
using System.Text;
using Microsoft.Win32;
using System.Text.Json;
using System.Reflection;
using YuchiGames.POM.Shared;
using System.Runtime.Versioning;
using YuchiGames.POM.Client.Assets;
using YuchiGames.POM.Client.Managers;
using Microsoft.Extensions.Configuration;

namespace YuchiGames.POM.Client
{
    public class Program : MelonMod
    {
        public static ClientSettings Settings { get; private set; } = new();
        public static string Version { get; private set; } = "";
        public static string UserGUID { get; private set; } = "";


        [SupportedOSPlatform("windows")]
        public override void OnInitializeMelon()
        {
            string settingsPath = $"{Directory.GetCurrentDirectory()}/Mods/settings.json";

            if (!File.Exists(settingsPath))
            {
                FileStream stream = File.Create(settingsPath);
                JsonSerializerOptions jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(
                    Settings,
                    jsonOptions);
                stream.Write(Encoding.UTF8.GetBytes(json));
            }

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(settingsPath)
                .Build();

            Settings = config.Get<ClientSettings>()
                ?? throw new Exception("Settings not found.");

            Version = (Assembly.GetExecutingAssembly().GetName().Version
                ?? throw new Exception("Version not found."))
                .ToString();

            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\SQMClient")
                ?? throw new Exception("SQMClient not found");

            string machineID = (key.GetValue("MachineId")?.ToString())
                ?? throw new Exception("MachineId not found");

            UserGUID = machineID.Trim('{', '}');

            MelonEvents.OnSceneWasInitialized.Subscribe(PingUI.OnSceneWasInitialized);
            MelonEvents.OnUpdate.Subscribe(Network.OnUpdate);
            MelonEvents.OnUpdate.Subscribe(PingUI.OnUpdate);
            MelonEvents.OnGUI.Subscribe(InfoGUI.OnGUI);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                InfoGUI.IsShow = !InfoGUI.IsShow;
        }

        public override void OnApplicationQuit() =>
            Network.Disconnect();
    }
}
