using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(StartButton), nameof(StartButton.OnPress))]
    class StartButtonPatch
    {
        private static bool Prefix()
        {
            Melon<Program>.Logger.Msg("StartButton.OnPress() called!");
            return false;
        }

        private static void Postfix()
        {
            Melon<Program>.Logger.Msg("StartButton.OnPress() finished!");
        }
    }
}
