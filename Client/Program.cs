using MelonLoader;
using Microsoft.Extensions.Configuration;
using UnityEngine;
using YuchiGames.POM.Client.Data;

namespace YuchiGames.POM.Client
{
    public class Program : MelonMod
    {
        private static ClientSettings? settings;
        public static ClientSettings Settings
        {
            get
            {
                if (settings is null)
                    throw new Exception("Settings not found.");
                return settings;
            }
        }
        private GameObject[] PlayerObject = new GameObject[2];

        public override void OnInitializeMelon()
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile($"{Directory.GetCurrentDirectory()}/Mods/settings.json")
                    .Build();
                settings = config.Get<ClientSettings>();

                if (settings is null)
                    throw new Exception("Settings not found.");
            }
            catch (Exception e)
            {
                LoggerInstance.Error(e.Message);
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                PlayerObject[0] = GameObject.Find("/Player/LeftHand");
                PlayerObject[1] = GameObject.Find("/Player/RightHand");
            }
            catch (Exception e)
            {
                LoggerInstance.Error(e.Message);
            }
        }

        public override void OnUpdate()
        {
            try
            {

            }
            catch (Exception e)
            {
                LoggerInstance.Error(e.Message);
            }
        }

        public override void OnFixedUpdate()
        {
            try
            {
                LoggerInstance.Msg("Player 1 position: {0}.", PlayerObject[0].transform.position);
                LoggerInstance.Msg("Player 2 position: {0}.", PlayerObject[1].transform.position);
            }
            catch (Exception e)
            {
                LoggerInstance.Error(e.Message);
            }
        }

        public override void OnApplicationQuit()
        {
            try
            {

            }
            catch (Exception e)
            {
                LoggerInstance.Error(e.Message);
            }
        }
    }
}
