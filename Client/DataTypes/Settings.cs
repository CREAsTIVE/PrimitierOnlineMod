namespace YuchiGames.POM.DataTypes
{
    public class ClientSettings
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public ClientSettings(string ip, int port)
        {
            IP = ip;
            Port = port;
        }
    }
}