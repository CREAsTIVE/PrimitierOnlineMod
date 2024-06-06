using MelonLoader;
using Microsoft.Extensions.Configuration;
using YuchiGames.POM.Client.Data;

namespace YuchiGames.POM.Client
{
    public class Program : MelonMod
    {
        public static ClientSettings? settings;

        public override void OnInitializeMelon()
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build();
                settings = config.Get<ClientSettings>();

                if (settings is null)
                    throw new Exception("Settings not found.");
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
        }
    }
}
