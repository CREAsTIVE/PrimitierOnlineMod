namespace YuchiGames.POM.Server.Data.Settings
{
    class ServerSettings
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public int Port { get; set; }
        public int MaxPlayer { get; set; }

        public ServerSettings(string name, string description, string version, int port, int maxPlayer)
        {
            Name = name;
            Description = description;
            Version = version;
            Port = port;
            MaxPlayer = maxPlayer;
        }
    }
}
