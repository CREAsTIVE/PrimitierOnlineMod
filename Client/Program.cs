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
