using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using YuchiGames.POM.Client.Assets;
using YuchiGames.POM.Client.Network;
using YuchiGames.POM.DataTypes;

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
                if (!ToggleOnline.IsOnline)
                    return true;
                ITcpMessage connectMessage = new ConnectMessage(Program.Settings.Version, Listeners.TcpEndPoint.Port, Listeners.UdpEndPoint.Port);
                ITcpMessage receiveMessage = Senders.Tcp(connectMessage);

                switch (receiveMessage)
                {
                    case SuccessConnectionMessage successConnectionMessage:
                        Program.MyID = successConnectionMessage.YourID;
                        Melon<Program>.Logger.Msg($"MyID: {Program.MyID}");
                        return true;
                    case FailureMessage failureMessage:
                        throw failureMessage.ExceptionMessage;
                }
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e);
            }
            return false;
        }

        private static void Postfix()
        {
            Melon<Program>.Logger.Msg("StartButton.OnPress() Postfix called!");
        }
    }
}
