using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using YuchiGames.POM.Client.Network;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(StartButton), nameof(StartButton.OnPress))]
    class StartButtonPatch
    {
        private static bool Prefix()
        {
            Melon<Program>.Logger.Msg("StartButton.OnPress() called!");
            try
            {
                ITcpMessage connectMessage = new ConnectMessage(Program.Settings.Version, Listeners.UdpEndPoint.Port);
                ITcpMessage resultConnect = Senders.Tcp(connectMessage);
                switch (resultConnect)
                {
                    case SuccessConnectionMessage successConnectionMessage:
                        Program.MyID = successConnectionMessage.YourID;
                        Program.IDList = successConnectionMessage.IDList;
                        break;
                    case FailureMessage failureMessage:
                        throw failureMessage.ExceptionMessage;
                }
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
            Melon<Program>.Logger.Msg("StartButton.OnPress() finished!");
        }
    }
}
