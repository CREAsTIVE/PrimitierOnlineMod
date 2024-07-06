namespace YuchiGames.POM.DataTypes
{
    public class ServerSettings
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Port { get; set; }
        public int MaxPlayer { get; set; }

        public ServerSettings(string name, string description, int port, int maxPlayer)
        {
            Name = name;
            Description = description;
            Port = port;
            MaxPlayer = maxPlayer;
        }
    }
}
