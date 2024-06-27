using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(VrmLoader), nameof(VrmLoader.avatarVisibility))]
    class VrmLoader_avatarVisibility
    {
        private static bool Prefix(bool __result)
        {
            Melon<Program>.Logger.Msg($"VrmLoader.avatarVisibility({__result}) Prefix called!");
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
            Melon<Program>.Logger.Msg("VrmLoader.avatarVisibility() Postfix called!");
        }
    }
}