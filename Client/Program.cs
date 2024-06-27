using MelonLoader;
using Microsoft.Extensions.Configuration;
using YuchiGames.POM.DataTypes;
using YuchiGames.POM.Client.Network;
using YuchiGames.POM.Client.Assets;

namespace YuchiGames.POM.Client
{
    public class Program : MelonMod
    {
        private static ClientSettings? s_settings;
        public static ClientSettings Settings
        {
            get
            {
                if (s_settings is null)
                    throw new Exception("Settings not found.");
                return s_settings;
            }
        }
        private static int s_myID;
        public static int MyID
        {
            get
            {
                return s_myID;
            }
            set
            {
                s_myID = value;
            }
        }
        private static int[]? s_idList;
        public static int[] IDList
        {
            get
            {
                if (s_idList is null)
                    throw new Exception("ID list not found.");
                return s_idList;
            }
            set
            {
                s_idList = value;
            }
        }

        public override void OnInitializeMelon()
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile($"{Directory.GetCurrentDirectory()}/Mods/settings.json")
                    .Build();
                s_settings = config.Get<ClientSettings>();
                if (s_settings is null)
                    throw new Exception("Settings not found.");

                Thread tcpThread = new Thread(Listeners.Tcp);
                Thread udpThread = new Thread(Listeners.Udp);
                tcpThread.Start();
                udpThread.Start();
            }
            catch (Exception e)
            {
                LoggerInstance.Error(e.Message);
            }
        }
    }
}
