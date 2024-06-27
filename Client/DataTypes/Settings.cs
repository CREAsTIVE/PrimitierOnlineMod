namespace YuchiGames.POM.DataTypes
{
    public class ClientSettings
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string IP { get; set; }
        public int TcpPort { get; set; }
        public int UdpPort { get; set; }

        public ClientSettings(string name, string version, string ip, int tcpPort, int udpPort)
        {
            Name = name;
            Version = version;
            IP = ip;
            TcpPort = tcpPort;
            UdpPort = udpPort;
        }
    }
}