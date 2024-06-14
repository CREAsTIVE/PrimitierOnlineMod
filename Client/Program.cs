using MelonLoader;
using Microsoft.Extensions.Configuration;
using UnityEngine;
using YuchiGames.POM.Data;
using YuchiGames.POM.Client.Network;
using System.Net;

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
        private static IPEndPoint? s_endPoint;
        public static IPEndPoint EndPoint
        {
            get
            {
                if (s_endPoint is null)
                    throw new Exception("End point not found.");
                return s_endPoint;
            }
            set
            {
                s_endPoint = value;
            }
        }

        public override void OnInitializeMelon()
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile($"{Directory.GetCurrentDirectory()}/Mods/settings.json")
                    .Build();
                s_settings = config.Get<ClientSettings>();

                if (s_settings is null)
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
