using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using YuchiGames.POM.Client.Assets;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(AvatarVisibilityButton), nameof(AvatarVisibilityButton.SwitchState))]
    class AvatarVisibilityButton_SwitchState
    {
        private static bool Prefix(bool state)
        {
            Melon<Program>.Logger.Msg($"AvatarVisibilityButton.SwitchState({state}) Prefix called!");
            try
            {
                PlayerSync.IsVRM = state;
                var WorldField = typeof(VrmLoader).GetField("avatarVisibility", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                Melon<Program>.Logger.Msg($"AvatarVisibilityButton.SwitchState() WorldField: {WorldField}");
                return true;
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e);
                return false;
            }
        }

        private static void Postfix()
        {
            Melon<Program>.Logger.Msg("AvatarVisibilityButton.SwitchState() Postfix called!");
        }
    }
}
