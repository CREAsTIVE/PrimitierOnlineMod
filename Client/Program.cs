using MelonLoader;
using Microsoft.Extensions.Configuration;
using YuchiGames.POM.Client.Data.Methods;
using YuchiGames.POM.Client.Data.Serialization;
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

                Connect connect = new Connect("1", "YuchiGames", "0.0.0");
                Tcp.Sender(CommandsSerializer.Serialize(connect));
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
