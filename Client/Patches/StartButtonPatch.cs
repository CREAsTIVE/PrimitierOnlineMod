using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(StartButton), nameof(StartButton.OnPress))]
    class StartButton_OnPress
    {
        private static bool Prefix()
        {
            Melon<Program>.Logger.Msg("StartButton.OnPress() Prefix called!");
            try
            {
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
            Melon<Program>.Logger.Msg("StartButton.OnPress() Postfix called!");
        }
    }
}
