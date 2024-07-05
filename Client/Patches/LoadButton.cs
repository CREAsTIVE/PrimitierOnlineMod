using HarmonyLib;
using Il2Cpp;
using YuchiGames.POM.Client.Network;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(LoadButton), nameof(LoadButton.OnPress))]
    class LoadButton_OnPress
    {
        static bool Prefix()
        {
            Sender sender = new Sender(Program.Settings.IP, Program.Settings.SendPort);

            return true;
        }

        static void Postfix()
        {

        }
    }
}