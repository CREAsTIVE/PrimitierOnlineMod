using MelonLoader;
using UnityEngine;
using YuchiGames.POM.Client.Network;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Assets
{
    class PlayerSync : MelonMod
    {
        private static bool s_isVRM = false;
        public static bool IsVRM
        {
            get
            {
                return s_isVRM;
            }
            set
            {
                s_isVRM = value;
            }
        }

        private static Transform[] s_handTransforms = new Transform[2];
        private static Transform[] s_vrmTransforms = new Transform[15];

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                if (ToggleOnline.IsOnline)
                    return;
                if (sceneName == "Main")
                {
                    s_handTransforms[0] = GameObject.Find("Player/XR Origin/Camera Offset/LeftHand Controller").transform;
                    s_handTransforms[1] = GameObject.Find("Player/XR Origin/Camera Offset/RightHand Controller").transform;

                    string vrmObjectPath = "Player/AvatarParent/VRM1/Root/Global/Position/J_Bip_C_Hips";
                    s_vrmTransforms[0] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_C_Neck/J_Bip_C_Head").transform;
                    s_vrmTransforms[1] = GameObject.Find($"{vrmObjectPath}").transform;
                    s_vrmTransforms[2] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine").transform;
                    s_vrmTransforms[3] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm").transform;
                    s_vrmTransforms[4] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm").transform;
                    s_vrmTransforms[5] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm/J_Bip_L_LowerArm").transform;
                    s_vrmTransforms[6] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm").transform;
                    s_vrmTransforms[7] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm/J_Bip_L_LowerArm/J_Bip_L_Hand").transform;
                    s_vrmTransforms[8] = GameObject.Find($"{vrmObjectPath}/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm/J_Bip_R_Hand").transform;
                    s_vrmTransforms[9] = GameObject.Find($"{vrmObjectPath}/J_Bip_L_UpperLeg").transform;
                    s_vrmTransforms[10] = GameObject.Find($"{vrmObjectPath}/J_Bip_R_UpperLeg").transform;
                    s_vrmTransforms[11] = GameObject.Find($"{vrmObjectPath}/J_Bip_L_UpperLeg/J_Bip_L_LowerLeg").transform;
                    s_vrmTransforms[12] = GameObject.Find($"{vrmObjectPath}/J_Bip_R_UpperLeg/J_Bip_R_LowerLeg").transform;
                    s_vrmTransforms[13] = GameObject.Find($"{vrmObjectPath}/J_Bip_L_UpperLeg/J_Bip_L_LowerLeg/J_Bip_L_Foot").transform;
                    s_vrmTransforms[14] = GameObject.Find($"{vrmObjectPath}/J_Bip_R_UpperLeg/J_Bip_R_LowerLeg/J_Bip_R_Foot").transform;
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }

        public override void OnFixedUpdate()
        {
            try
            {
                if (ToggleOnline.IsOnline)
                    return;
                SendPlayerPosMessage sendPlayerPosMessage = new SendPlayerPosMessage(
                    Program.MyID,
                    s_isVRM,
                    new BaseBody(
                        Utils.ConvertToPosRot(s_handTransforms[0]),
                        Utils.ConvertToPosRot(s_handTransforms[1]),
                        Utils.ConvertToPosRot(s_handTransforms[2])
                        ),
                    new VRMBody(
                        Utils.ConvertToPosRot(s_vrmTransforms[0]),
                        Utils.ConvertToPosRot(s_vrmTransforms[1]),
                        Utils.ConvertToPosRot(s_vrmTransforms[2]),
                        Utils.ConvertToPosRot(s_vrmTransforms[3]),
                        Utils.ConvertToPosRot(s_vrmTransforms[4]),
                        Utils.ConvertToPosRot(s_vrmTransforms[5]),
                        Utils.ConvertToPosRot(s_vrmTransforms[6]),
                        Utils.ConvertToPosRot(s_vrmTransforms[7]),
                        Utils.ConvertToPosRot(s_vrmTransforms[8]),
                        Utils.ConvertToPosRot(s_vrmTransforms[9]),
                        Utils.ConvertToPosRot(s_vrmTransforms[10]),
                        Utils.ConvertToPosRot(s_vrmTransforms[11]),
                        Utils.ConvertToPosRot(s_vrmTransforms[12]),
                        Utils.ConvertToPosRot(s_vrmTransforms[13]),
                        Utils.ConvertToPosRot(s_vrmTransforms[14])
                        )
                    );
                Senders.Udp(sendPlayerPosMessage);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }
    }
}