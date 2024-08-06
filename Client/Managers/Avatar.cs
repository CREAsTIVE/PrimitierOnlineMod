using Il2Cpp;
using UnityEngine;
using Il2CppUniGLTF;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Managers
{
    public static class Avatar
    {
        private static bool s_isInitialized = false;
        public static bool IsInitialized
        {
            get => s_isInitialized;
        }

        private static RuntimeGltfInstance[]? s_gltfInstances;
        private static Transform[] s_avatarTransforms = new Transform[15];

        public static void Initialize(int maxPlayers)
        {
            s_gltfInstances = new RuntimeGltfInstance[maxPlayers];
            Transform root = GameObject.Find("/Player/AvatarParent/VRM1/Root/Global/Position/J_Bip_C_Hips").transform;
            s_avatarTransforms[0] = root.Find("J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_C_Neck").transform;
            s_avatarTransforms[1] = root;
            s_avatarTransforms[2] = root.Find("J_Bip_C_Spine").transform;
            s_avatarTransforms[3] = root.Find("J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm").transform;
            s_avatarTransforms[4] = root.Find("J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm").transform;
            s_avatarTransforms[5] = root.Find("J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm/J_Bip_L_LowerArm").transform;
            s_avatarTransforms[6] = root.Find("J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm").transform;
            s_avatarTransforms[7] = root.Find("J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm/J_Bip_L_LowerArm/J_Bip_L_Hand").transform;
            s_avatarTransforms[8] = root.Find("J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm/J_Bip_R_Hand").transform;
            s_avatarTransforms[9] = root.Find("J_Bip_L_UpperLeg").transform;
            s_avatarTransforms[10] = root.Find("J_Bip_R_UpperLeg").transform;
            s_avatarTransforms[11] = root.Find("J_Bip_L_UpperLeg/J_Bip_L_LowerLeg").transform;
            s_avatarTransforms[12] = root.Find("J_Bip_R_UpperLeg/J_Bip_R_LowerLeg").transform;
            s_avatarTransforms[13] = root.Find("J_Bip_L_UpperLeg/J_Bip_L_LowerLeg/J_Bip_L_Foot").transform;
            s_avatarTransforms[14] = root.Find("J_Bip_R_UpperLeg/J_Bip_R_LowerLeg/J_Bip_R_Foot").transform;
            s_isInitialized = true;
        }

        public static void GetAllAvatarData(byte[][] data)
        {
            if (!s_isInitialized)
                throw new Exception("AvatarManager not initialized.");
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != null)
                    LoadAvatar(i, data[i]);
            }
        }

        public static byte[] GetAvatarData()
        {
            string path = VrmLoader.GetAvatarDirectory();
            string[] files = Directory.GetFiles(path, "*.vrm");
            return File.ReadAllBytes(files[0]);
        }

        public static void LoadAvatar(int id, byte[] vrmData)
        {
            if (!s_isInitialized)
                throw new Exception("AvatarManager not initialized.");
            GltfData gltfData;
            try
            {
                gltfData = new GlbBinaryParser(vrmData, "").Parse();
            }
            catch
            {
                Log.Error("Failed to parse VRM data.");
                return;
            }
            ImporterContext loader = new ImporterContext(gltfData);
            RuntimeGltfInstance instance = loader.Load();
            instance.name = $"Player_{id}";
            instance.EnableUpdateWhenOffscreen();
            instance.ShowMeshes();
            if (s_gltfInstances == null)
                throw new Exception("Avatar already loaded.");
            s_gltfInstances[id] = instance;
            Log.Debug($"Avatar loaded: {id}, {s_gltfInstances[id].name}");
        }

        public static void DestroyAvatar(int id)
        {
            if (!s_isInitialized)
                throw new Exception("AvatarManager not initialized.");
            if (s_gltfInstances == null)
                throw new Exception("Avatar not loaded.");
            GameObject.Destroy(s_gltfInstances[id]);
            Log.Debug($"Avatar destroyed: {id}");
        }

        public static void SendAvatarPosition()
        {
            if (!Network.IsConnected)
                return;
            DataTypes.PosRot[] posRots = new DataTypes.PosRot[15];
            for (int i = 0; i < posRots.Length; i++)
            {
                posRots[i] = DataConverter.ToPosRot(s_avatarTransforms[i]);
            }
            VRMPosData vrmPosData = new VRMPosData(posRots);
            IMultiMessage message = new AvatarPositionMessage(Network.ID, vrmPosData);
            Network.Send(message);
        }

        public static void UpdatePosition(VRMPosData posData)
        {
            if (!s_isInitialized)
                return;
        }
    }
}