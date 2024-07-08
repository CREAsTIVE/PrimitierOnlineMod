namespace YuchiGames.POM.DataTypes
{
    public class ServerSettings
    {
        public int Port { get; set; }
        public int MaxPlayers { get; set; }

        public ServerSettings(int port, int maxPlayers)
        {
            Port = port;
            MaxPlayers = maxPlayers;
        }
    }
}
