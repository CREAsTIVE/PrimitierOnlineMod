namespace YuchiGames.POM.Shared
{
    public class ClientSettings
    {
        public string IP { get; init; }
        public int Port { get; init; }
        public string UserName { get; init; }
        public string MinimumLogLevel { get; init; }

        public ClientSettings()
        {
            IP = "127.0.0.1";
            Port = 54162;
            UserName = "AnonymousUser";
            MinimumLogLevel = "Information";
        }
    }
}