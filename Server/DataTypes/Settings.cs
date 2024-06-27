namespace YuchiGames.POM.DataTypes
{
    public class ServerSettings
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public int TcpPort { get; set; }
        public int UdpPort { get; set; }
        public int MaxPlayer { get; set; }

        public ServerSettings(string name, string description, string version, int tcpPort, int udpPort, int maxPlayer)
        {
            Name = name;
            Description = description;
            Version = version;
            TcpPort = tcpPort;
            UdpPort = udpPort;
            MaxPlayer = maxPlayer;
        }
    }
}
