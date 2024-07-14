namespace YuchiGames.POM.DataTypes
{
    public class ServerSettings
    {
        public int Port { get; }
        public int MaxPlayers { get; }
        public int MaxDataSize { get; }

        public ServerSettings(int port, int maxPlayers, int maxDataSize)
        {
            Port = port;
            MaxPlayers = maxPlayers;
            MaxDataSize = maxDataSize;
        }
    }
}
