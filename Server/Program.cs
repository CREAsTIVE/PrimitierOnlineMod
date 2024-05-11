using Microsoft.Extensions.Configuration;
using Serilog;
using YuchiGames.POM.Server.Data.Settings;
using YuchiGames.POM.Server.Network.Listeners;

namespace YuchiGames.POM.Server
{
    class Program
    {
        public static ServerSettings? settings;
        
        static void Main(string[] args)
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build();
                settings = config.Get<ServerSettings>();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();

                Thread tcpThread = new Thread(Tcp.Listener);
                Thread udpThread = new Thread(Udp.Listener);
                tcpThread.Start();
                udpThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}