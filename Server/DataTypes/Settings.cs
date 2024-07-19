namespace YuchiGames.POM.DataTypes
{
    public class ServerSettings
    {
        public int Port { get; }
        public int MaxPlayers { get; }
        public int MaxDataSize { get; }
        public int? Seed { get; }

        public ServerSettings(int port, int maxPlayers, int maxDataSize, int seed)
        {
            Port = port;
            MaxPlayers = maxPlayers;
            MaxDataSize = maxDataSize;
            Seed = seed;
        }
    }
}
