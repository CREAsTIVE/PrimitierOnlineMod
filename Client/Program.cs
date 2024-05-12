using MelonLoader;
using Microsoft.Extensions.Configuration;
using YuchiGames.POM.Client.Data.Settings;
using YuchiGames.POM.Client.Network;

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

                Thread tcpThread = new Thread(Tcp.Listener);
                Thread udpThread = new Thread(Udp.Listener);
                tcpThread.Start();
                udpThread.Start();

                Discord.Discord discord = new Discord.Discord(1239248231737856060, (UInt64)Discord.CreateFlags.NoRequireDiscord);
                while (true)
                {
                    discord.RunCallbacks();
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }

        public override void OnFixedUpdate()
        {

        }
    }
}
