namespace YuchiGames.POM.DataTypes
{
    public class ClientSettings
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string MinimumLogLevel { get; set; }

        public ClientSettings(string ip, int port, string minimumLogLevel)
        {
            IP = ip;
            Port = port;
            MinimumLogLevel = minimumLogLevel;
        }
    }
}