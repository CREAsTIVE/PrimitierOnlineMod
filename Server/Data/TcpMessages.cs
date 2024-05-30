using MessagePack;

namespace YuchiGames.POM.Server.Data.TcpMessages
{
    interface IMessage
    {
        string Name { get; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ConnectMessage : IMessage
    {
        // [Key(2)]
        public string Name { get; } = "ConnectMessage";
        // [Key(0)]
        public string Version { get; set; }
        // [Key(1)]
        public string UserName { get; set; }

        [SerializationConstructor]
        public ConnectMessage(string version, string userName)
        {
            Version = version;
            UserName = userName;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class DisconnectMessage : IMessage
    {
        // [Key(0)]
        public string Name { get; } = "DisconnectMessage";
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SuccessConnectionMessage
    {
        // [Key(2)]
        public string Name { get; } = "SuccessConnectionMessage";
        // [Key(0)]
        public int YourID { get; set; }
        // [Key(1)]
        public int[] IDList { get; set; }

        [SerializationConstructor]
        public SuccessConnectionMessage(int yourID, int[] idList)
        {
            YourID = yourID;
            IDList = idList;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SuccessMessage : IMessage
    {
        // [Key(0)]
        public string Name { get; } = "SuccessMessage";
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class FailureMessage : IMessage
    {
        // [Key(1)]
        public string Name { get; } = "FailureMessage";
        // [Key(0)]
        public Exception ExceptionMessage { get; set; }

        [SerializationConstructor]
        public FailureMessage(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}