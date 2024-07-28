using MelonLoader;
using UnityEngine;
using System.Reflection;
using YuchiGames.POM.DataTypes;
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

            MelonEvents.OnUpdate.Subscribe(Network.OnUpdate);
            MelonEvents.OnUpdate.Subscribe(PingUI.SetPing);
            MelonEvents.OnGUI.Subscribe(InfoGUI.ShowGUI);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                Assets.StartButton.Initialize();
                Assets.PingUI.Initialize();
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
