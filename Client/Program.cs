using MelonLoader;
using UnityEngine;
using Microsoft.Win32;
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
        private static ClientSettings? s_settings;
        public static ClientSettings Settings
        {
            get
            {
                if (s_settings is null)
                    throw new Exception("Settings not found.");
                return s_settings;
            }
        }
        private static string? s_version;
        public static string Version
        {
            get
            {
                if (s_version is null)
                    throw new Exception("Version not found.");
                return s_version;
            }
        }
        private static string? s_userGUID;
        public static string UserGUID
        {
            get
            {
                if (s_userGUID is null)
                    throw new Exception("MachineId not found.");
                return s_userGUID;
            }
        }

        [SupportedOSPlatform("windows")]
        public override void OnInitializeMelon()
        {
            string path = $"{Directory.GetCurrentDirectory()}/Mods/settings.json";
            if (!File.Exists(path))
                throw new FileNotFoundException();
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();
            s_settings = config.Get<ClientSettings>();
            if (s_settings is null)
                throw new Exception("Settings not found.");

            s_version = (Assembly.GetExecutingAssembly().GetName().Version
                ?? throw new Exception("Version not found."))
                .ToString();

            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\SQMClient")
                ?? throw new Exception("MachineId not found");
            string? machineID = (key.GetValue("MachineId")?.ToString())
                ?? throw new Exception("MachineId not found");
            s_userGUID = machineID.Trim('{', '}');

            MelonEvents.OnUpdate.Subscribe(Network.OnUpdate);
            MelonEvents.OnUpdate.Subscribe(PingUI.SetPing);
            MelonEvents.OnUpdate.Subscribe(Avatar.SendAvatarPosition);
            MelonEvents.OnGUI.Subscribe(InfoGUI.ShowGUI);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                StartButton.Initialize();
                PingUI.Initialize();
                GameObject settingsTabObject = GameObject.Find("/Player/XR Origin/Camera Offset/LeftHand Controller/RealLeftHand/MenuWindowL/Windows/MainCanvas/SettingsTab");
                settingsTabObject.transform.Find("DayNightCycleButton").gameObject.SetActive(false);
                settingsTabObject.transform.Find("DistanceSettings").gameObject.SetActive(false);
            }
        }

        public override void OnApplicationQuit()
        {
            Network.Disconnect();
        }

        public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                InfoGUI.IsShow = !InfoGUI.IsShow;
            }
        }
    }
}
