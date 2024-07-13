using HarmonyLib;
using Il2Cpp;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(StartButton), nameof(StartButton.OnPress))]
    class StartButton_OnPress
    {
        private static bool Prefix()
        {
            NetworkManager.Connect();
            string path = VrmLoader.GetAvatarDirectory();
            string[] files = Directory.GetFiles(path, "*.vrm");
            byte[] data = File.ReadAllBytes(files[0]);
            return true;
        }

        private static void Postfix() { }
    }
}