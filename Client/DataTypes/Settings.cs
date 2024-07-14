namespace YuchiGames.POM.DataTypes
{
    public class ClientSettings
    {
        public string IP { get; }
        public int Port { get; }
        public string MinimumLogLevel { get; }

        public ClientSettings(string ip, int port, string minimumLogLevel)
        {
            IP = ip;
            Port = port;
            MinimumLogLevel = minimumLogLevel;
        }
    }
}