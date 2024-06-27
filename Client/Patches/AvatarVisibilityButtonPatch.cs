using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(AvatarVisibilityButton), nameof(AvatarVisibilityButton.SwitchState))]
    class AvatarVisibilityButton_SwitchState
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

        private static bool Prefix(bool state)
        {
            Melon<Program>.Logger.Msg($"AvatarVisibilityButton.SwitchState({state}) Prefix called!");
            try
            {
                s_isVRM = state;
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
