using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(AvatarVisibilityButton), nameof(AvatarVisibilityButton.SwitchState))]
    class AvatarVisibilityButtonPatch
    {
        private static void Prefix()
        {
            Melon<Program>.Logger.Msg("AvatarVisibilityButton.SwitchState() called!");
        }

        private static void Postfix()
        {
            Melon<Program>.Logger.Msg("AvatarVisibilityButton.SwitchState() finished!");
        }
    }
}
