using HarmonyLib;
using Il2Cpp;
using YuchiGames.POM.Client.Managers;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(StartButton), nameof(StartButton.OnPress))]
    class StartButton_OnPress
    {
        private static bool Prefix()
        {
            Network.Connect();
            return true;
        }

        private static void Postfix() { }
    }
}