using MelonLoader;
using Microsoft.Extensions.Configuration;
using UnityEngine;
using YuchiGames.POM.DataTypes;
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

        private static Transform[] s_handObjects = new Transform[2];
        private static Transform[] s_vrmObjects = new Transform[15];

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
                LoggerInstance.Msg($"Scene loaded: {sceneName}, SceneIndex: {buildIndex}");
                if (sceneName == "Main")
                {
                    s_handObjects[0] = GameObject.Find("Player/XR Origin/Camera Offset/LeftHand Controller").transform;
                    s_handObjects[1] = GameObject.Find("Player/XR Origin/Camera Offset/RightHand Controller").transform;

                    string vrmObjectPath = "Player/AvatarParent/VRM1/Root/Global/Position/J_Bip_C_Hips";
                    s_vrmObjects[0] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_C_Neck/J_Bip_C_Head").transform;
                    s_vrmObjects[1] = GameObject.Find($"{vrmObjectPath}").transform;
                    s_vrmObjects[2] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine").transform;
                    s_vrmObjects[3] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm").transform;
                    s_vrmObjects[4] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm").transform;
                    s_vrmObjects[5] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm/J_Bip_L_LowerArm").transform;
                    s_vrmObjects[6] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm").transform;
                    s_vrmObjects[7] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm/J_Bip_L_LowerArm/J_Bip_L_Hand").transform;
                    s_vrmObjects[8] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm/J_Bip_R_Hand").transform;
                    s_vrmObjects[9] = GameObject.Find($"{vrmObjectPath}/J_Bip_L_UpperLeg").transform;
                    s_vrmObjects[10] = GameObject.Find($"{vrmObjectPath}/J_Bip_R_UpperLeg").transform;
                    s_vrmObjects[11] = GameObject.Find($"{vrmObjectPath}/J_Bip_L_UpperLeg/J_Bip_L_LowerLeg").transform;
                    s_vrmObjects[12] = GameObject.Find($"{vrmObjectPath}/J_Bip_R_UpperLeg/J_Bip_R_LowerLeg").transform;
                    s_vrmObjects[13] = GameObject.Find($"{vrmObjectPath}/J_Bip_L_UpperLeg/J_Bip_L_LowerLeg/J_Bip_L_Foot").transform;
                    s_vrmObjects[14] = GameObject.Find($"{vrmObjectPath}/J_Bip_R_UpperLeg/J_Bip_R_LowerLeg/J_Bip_R_Foot").transform;
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
