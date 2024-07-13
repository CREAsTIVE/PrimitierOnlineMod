using MelonLoader;
using Microsoft.Extensions.Configuration;
using UnityEngine;
using YuchiGames.POM.Client.Assets;
using YuchiGames.POM.DataTypes;

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

            MelonEvents.OnUpdate.Subscribe(NetworkManager.OnUpdate);
            MelonEvents.OnGUI.Subscribe(InfoUI.Ping);
        }

        public override void OnApplicationQuit()
        {
            NetworkManager.Disconnect();
        }

        public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                InfoUI.Show = !InfoUI.Show;
            }
        }
    }
}
