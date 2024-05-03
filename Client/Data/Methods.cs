using MessagePack;

namespace YuchiGames.POM.Client.Data.Methods
{
    [MessagePackObject]
    public class MethodsName
    {
        [Key(0)]
        public string Name { get; set; }

        [SerializationConstructor]
        public MethodsName(string name)
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

        [SerializationConstructor]
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

        [SerializationConstructor]
        public Error(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}