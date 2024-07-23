using MelonLoader;
using Microsoft.Extensions.Configuration;
using UnityEngine;
using YuchiGames.POM.Client.Assets;
using YuchiGames.POM.DataTypes;
using YuchiGames.POM.Client.Managers;

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

            MelonEvents.OnUpdate.Subscribe(Network.OnUpdate);
            MelonEvents.OnGUI.Subscribe(InfoGUI.ShowGUI);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                Assets.StartButton.Initialize();
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
