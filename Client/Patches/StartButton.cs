using HarmonyLib;
using Il2Cpp;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(StartButton), nameof(StartButton.OnPress))]
    class StartButton_OnPress
    {
        private static bool Prefix()
        {
            NetworkManager.Connect();
            return true;
        }

        private static void Postfix() { }
    }
}