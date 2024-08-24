using MelonLoader;
using UnityEngine;
using System.Text;
using Microsoft.Win32;
using System.Text.Json;
using System.Reflection;
using YuchiGames.POM.DataTypes;
using System.Runtime.Versioning;
using YuchiGames.POM.Client.Assets;
using YuchiGames.POM.Client.Managers;
using Microsoft.Extensions.Configuration;

namespace YuchiGames.POM.Client
{
    public class Program : MelonMod
    {
        private static ClientSettings s_settings;
        public static ClientSettings Settings
        {
            get => s_settings;
        }
        private static string s_version;
        public static string Version
        {
            get => s_version;
        }
        private static string s_userGUID;
        public static string UserGUID
        {
            get => s_userGUID;
        }

        static Program()
        {
            s_settings = new ClientSettings();
            s_version = "";
            s_userGUID = "";
        }

        [SupportedOSPlatform("windows")]
        public override void OnInitializeMelon()
        {
            string path = $"{Directory.GetCurrentDirectory()}/Mods/settings.json";
            if (!File.Exists(path))
            {
                using (FileStream stream = File.Create(path))
                {
                    JsonSerializerOptions jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    string json = JsonSerializer.Serialize(
                        s_settings,
                        jsonOptions);
                    stream.Write(Encoding.UTF8.GetBytes(json));
                }
            }
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();
            s_settings = config.Get<ClientSettings>()
                ?? throw new Exception("Settings not found.");

            s_version = (Assembly.GetExecutingAssembly().GetName().Version
                ?? throw new Exception("Version not found."))
                .ToString();

            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\SQMClient")
                ?? throw new Exception("SQMClient not found");
            string? machineID = (key.GetValue("MachineId")?.ToString())
                ?? throw new Exception("MachineId not found");
            s_userGUID = machineID.Trim('{', '}');

            MelonEvents.OnSceneWasInitialized.Subscribe(PingUI.OnSceneWasInitialized);
            MelonEvents.OnUpdate.Subscribe(Network.OnUpdate);
            MelonEvents.OnUpdate.Subscribe(PingUI.OnUpdate);
            MelonEvents.OnGUI.Subscribe(InfoGUI.OnGUI);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                InfoGUI.IsShow = !InfoGUI.IsShow;
            }
        }

        public override void OnApplicationQuit()
        {
            Network.Disconnect();
        }
    }
}
