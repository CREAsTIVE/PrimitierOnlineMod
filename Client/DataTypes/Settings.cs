namespace YuchiGames.POM.DataTypes
{
    public class ClientSettings
    {
        public string IP { get; }
        public int Port { get; }
        public string UserName { get; }
        public string MinimumLogLevel { get; }

        public ClientSettings(string ip, int port, string userName, string minimumLogLevel)
        {
            IP = ip;
            Port = port;
            UserName = userName;
            MinimumLogLevel = minimumLogLevel;
        }
    }
}