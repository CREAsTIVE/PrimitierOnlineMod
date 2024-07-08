namespace YuchiGames.POM.DataTypes
{
    public class ServerSettings
    {
        public int Port { get; set; }
        public int MaxPlayer { get; set; }

        public ServerSettings(int port, int maxPlayer)
        {
            Port = port;
            MaxPlayer = maxPlayer;
        }
    }
}
