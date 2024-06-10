﻿using MelonLoader;
using Microsoft.Extensions.Configuration;
using UnityEngine;
using YuchiGames.POM.Client.Data;
using YuchiGames.POM.Server.Data;
using YuchiGames.POM.Client.Network;

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
        private Transform[] PlayerTransforms = new Transform[2];

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

                Listeners listeners = new Listeners();
                Thread udpThread = new Thread(listeners.Udp);
                udpThread.Start();

                Senders senders = new Senders();
                senders.Tcp(new ConnectMessage(settings.Version, settings.Name));
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
                PlayerTransforms[0] = GameObject.Find("/Player/LeftHand").transform;
                PlayerTransforms[1] = GameObject.Find("/Player/RightHand").transform;
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
                LoggerInstance.Msg("Player 1 position: {0}.", PlayerTransforms[0].position);
                LoggerInstance.Msg("Player 2 position: {0}.", PlayerTransforms[1].position);
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
