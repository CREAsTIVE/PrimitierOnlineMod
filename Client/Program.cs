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
        private static int s_myID;
        public static int MyID
        {
            get
            {
                return s_myID;
            }
            set
            {
                s_myID = value;
            }
        }
        private static int[]? s_idList;
        public static int[] IDList
        {
            get
            {
                if (s_idList is null)
                    throw new Exception("ID list not found.");
                return s_idList;
            }
            set
            {
                s_idList = value;
            }
        }

        private static GameObject[] s_playerObjects = new GameObject[2];

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

                Thread udpThread = new Thread(Listeners.Udp);
                udpThread.Start();

                ITcpMessage connectMessage = new ConnectMessage(s_settings.Version);
                ITcpMessage receiveMessage = Senders.Tcp(connectMessage);
                switch (receiveMessage)
                {
                    case SuccessConnectionMessage successConnectionMessage:
                        s_myID = successConnectionMessage.YourID;
                        s_idList = successConnectionMessage.IDList;
                        LoggerInstance.Msg($"Connected to server. My ID: {s_myID}, ID list: {string.Join(", ", s_idList)}");
                        break;
                    case FailureMessage failureMessage:
                        throw failureMessage.ExceptionMessage;
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
                if (sceneName == "Main")
                {
                    LoggerInstance.Msg($"Scene loaded: {sceneName}, SceneIndex: {buildIndex}");
                    s_playerObjects[0] = GameObject.Find("Player/XR Origin/Camera Offset/LeftHand Controller");
                    s_playerObjects[1] = GameObject.Find("Player/XR Origin/Camera Offset/RightHand Controller");
                    LoggerInstance.Msg($"Player objects found. {s_playerObjects[0].name}, {s_playerObjects[1].name}");
                }
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
                LoggerInstance.Msg($"LeftHand: {s_playerObjects[0].transform.position}, RightHand: {s_playerObjects[1].transform.position}");
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
