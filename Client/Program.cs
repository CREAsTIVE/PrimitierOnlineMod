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
        private Transform[] _playerTransforms = new Transform[2];

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

                Listeners listeners = new Listeners();
                Thread udpThread = new Thread(listeners.Udp);
                udpThread.Start();

                Senders senders = new Senders();
                ITcpMessage receiveMessage = senders.Tcp(new ConnectMessage(s_settings.Version, s_settings.Name));

                switch (receiveMessage)
                {
                    case SuccessConnectionMessage successConnectionMessage:
                        LoggerInstance.Msg($"Success Connection: {successConnectionMessage.YourID}, {successConnectionMessage.IDList}");
                        break;
                    case FailureMessage failureMessage:
                        LoggerInstance.Error(failureMessage.ToString());
                        break;
                    default:
                        throw new Exception("Unknown message type.");
                }
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
                _playerTransforms[0] = GameObject.Find("/Player/LeftHand").transform;
                _playerTransforms[1] = GameObject.Find("/Player/RightHand").transform;
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
                //LoggerInstance.Msg("Player 1 position: {0}.", PlayerTransforms[0].position);
                //LoggerInstance.Msg("Player 2 position: {0}.", PlayerTransforms[1].position);
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
                Senders senders = new Senders();
                ITcpMessage receiveMessage = senders.Tcp(new DisconnectMessage());

                switch (receiveMessage)
                {
                    case SuccessMessage successMessage:
                        LoggerInstance.Msg("Success Disconnection.");
                        break;
                    case FailureMessage failureMessage:
                        LoggerInstance.Error(failureMessage.ToString());
                        break;
                    default:
                        throw new Exception("Unknown message type.");
                }
            }
            catch (Exception e)
            {
                LoggerInstance.Error(e.Message);
            }
        }
    }
}
