namespace YuchiGames.POM.Client.Data.Settings
{
    public class ClientSettings
    {
        public string UserName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }

        public ClientSettings(string username, string ipaddress, int port)
        {
            UserName = username;
            IPAddress = ipaddress;
            Port = port;
        }
    }
}
