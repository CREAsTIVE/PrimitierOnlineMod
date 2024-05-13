using MelonLoader;
using YuchiGames.POM.Client.DiscordWrapper;

namespace YuchiGames.POM.Client
{
    public class Program : MelonMod
    {
        public override void OnInitializeMelon()
        {
            try
            {
                DiscordSetup.Initialize();
                DiscordActivity.Initialize();
                DiscordActivity.UpdateActivity("In the online", "Playing online mode");
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }
    }
}
