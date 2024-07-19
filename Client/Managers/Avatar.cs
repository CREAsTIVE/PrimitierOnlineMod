using Il2Cpp;
using Il2CppUniGLTF;
using UnityEngine;

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

        public static void Initialize(int maxPlayers)
        {
            s_gltfInstances = new RuntimeGltfInstance[maxPlayers];
            s_isInitialized = true;
        }

        public static void GetAllAvatarData(byte[][] data)
        {
            if (!s_isInitialized)
                throw new Exception("AvatarManager not initialized.");
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                    continue;
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
            Log.Debug($"Avatar loaded: {id}: {s_gltfInstances[id].name}");
        }

        public static void DestroyAvatar(int id)
        {
            if (!s_isInitialized)
                throw new Exception("AvatarManager not initialized.");
            if (s_gltfInstances == null)
                throw new Exception("Avatar not loaded.");
            GameObject.Destroy(s_gltfInstances[id]);
        }
    }
}