using MelonLoader;
using Microsoft.Extensions.Configuration;
using YuchiGames.POM.Client.Network;
using YuchiGames.POM.DataTypes;

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
        private static Listener? s_listener;

        public override void OnInitializeMelon()
        {
            try
            {
                string path = $"{Directory.GetCurrentDirectory()}/Mods/settings.json";
                if (!File.Exists(path))
                    throw new FileNotFoundException();
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile(path)
                    .Build();
                s_settings = config.Get<ClientSettings>();
                if (s_settings is null)
                    throw new Exception("Settings not found.");

                s_listener = new Listener(s_settings.IP, s_settings.ListenPort);
                s_listener.Start();
            }
            catch (Exception e)
            {
                LoggerInstance.Error(e.Message);
            }
        }

        public override void OnApplicationQuit()
        {
            if (s_listener is null)
                throw new Exception("Listener not found.");
            s_listener.Stop();
        }
    }
}
