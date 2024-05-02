using MessagePack;

namespace YuchiGames.POM.Server.Data.Commands
{
    [MessagePackObject]
    public class CommandName
    {
        [Key(0)]
        public string Name { get; set; }

        public CommandName(string name)
        {
            Name = name;
        }
    }

    [MessagePackObject]
    public class Connect
    {
        [Key(0)]
        public string Name { get; set; } = "Connect";
        [Key(1)]
        public string UserID { get; set; }
        [Key(2)]
        public string UserName { get; set; }
        [Key(3)]
        public string Version { get; set; }

        public Connect(string userID, string userName, string version)
        {
            UserID = userID;
            UserName = userName;
            Version = version;
        }
    }

    [MessagePackObject]
    public class Disconnect
    {
        [Key(0)]
        public string Name { get; set; } = "Disconnect";
    }

    [MessagePackObject]
    public class Error
    {
        [Key(0)]
        public string Name { get; set; } = "Error";
        [Key(1)]
        public Exception ExceptionMessage { get; set; }

        public Error(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}