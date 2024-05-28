using MessagePack;

namespace YuchiGames.POM.Server.Data.TcpMessages
{
    interface IMessage
    {
        string Name { get; }
    }

    [MessagePackObject]
    public class ConnectMessage : IMessage
    {
        [Key(0)]
        public string Name { get; } = "ConnectMessage";
        [Key(1)]
        public string Version { get; set; }

        [SerializationConstructor]
        public ConnectMessage(string version)
        {
            Version = version;
        }
    }

    [MessagePackObject]
    public class DisconnectMessage : IMessage
    {
        [Key(0)]
        public string Name { get; } = "DisconnectMessage";
    }

    [MessagePackObject]
    public class SuccessConnectionMessage : IMessage
    {
        [Key(0)]
        public string Name { get; } = "SuccessConnectionMessage";
        [Key(1)]
        public string YourID { get; set; }
        [Key(2)]
        public string[] IDList { get; set; }
        
        [SerializationConstructor]
        public SuccessConnectionMessage(string yourID, string[] idList)
        {
            YourID = yourID;
            IDList = idList;
        }
    }

    [MessagePackObject]
    public class SuccessMessage : IMessage
    {
        [Key(0)]
        public string Name { get; } = "SuccessMessage";
    }

    [MessagePackObject]
    public class FailureMessage : IMessage
    {
        [Key(0)]
        public string Name { get; } = "FailureMessage";
        [Key(1)]
        public Exception ExceptionMessage { get; set; }

        [SerializationConstructor]
        public FailureMessage(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}