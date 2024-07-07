using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using System.Reflection;
using YuchiGames.POM.Client.Network;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(StartButton), nameof(StartButton.OnPress))]
    class StartButton_OnPress
    {
        static bool Prefix()
        {
            Melon<Program>.Logger.Msg("StartButton_OnPress.Prefix() called");

            try
            {
                AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
                if (assemblyName.Version is null)
                    throw new Exception("Assembly version is null");
                ITcpMessage connectMessage = new ConnectMessage(assemblyName.Version.ToString(), Program.ListenPort);
                Sender sender = new Sender(Program.Settings.IP, Program.Settings.Port);
                ITcpMessage receiveMessage = sender.Tcp(connectMessage);
                switch (receiveMessage)
                {
                    case SuccessConnectionMessage successConnectionMessage:
                        Program.MyID = successConnectionMessage.YourID;
                        Program.IDList = successConnectionMessage.IDList;
                        Melon<Program>.Logger.Msg("Connected to server.");
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

        static void Postfix()
        {
            Melon<Program>.Logger.Msg("StartButton_OnPress.Postfix() called");
        }
    }
}