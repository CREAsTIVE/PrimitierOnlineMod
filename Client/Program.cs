using MelonLoader;
using Microsoft.Extensions.Configuration;
using UnityEngine;
using YuchiGames.POM.Client.Data;
using YuchiGames.POM.Client.Network.Senders;

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

                GameObject gameObject = new GameObject();
                //Tcp.Sender(new ConnectMessage());
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }

        public override void OnUpdate()
        {
            MelonLogger.Msg("OnUpdate Method");
        }

        public override void OnApplicationQuit()
        {
            MelonLogger.Msg("OnApplicationQuit Method");
        }
    }
}
