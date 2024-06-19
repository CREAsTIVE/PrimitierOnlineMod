using MessagePack;

namespace YuchiGames.POM.Data
{
    [Union(0, typeof(ConnectMessage))]
    [Union(1, typeof(SuccessConnectionMessage))]
    [Union(2, typeof(DisconnectMessage))]
    [Union(3, typeof(SuccessMessage))]
    [Union(4, typeof(FailureMessage))]
    public interface ITcpMessage { }

    [MessagePackObject]
    public struct ConnectMessage : ITcpMessage
    {
        [Key(0)]
        public string Version { get; set; }

        [SerializationConstructor]
        public ConnectMessage(string version)
        {
            Version = version;
        }
    }

    [MessagePackObject]
    public struct SuccessConnectionMessage : ITcpMessage
    {
        [Key(0)]
        public int YourID { get; set; }
        [Key(1)]
        public int[] IDList { get; set; }

        [SerializationConstructor]
        public SuccessConnectionMessage(int yourID, int[] idList)
        {
            YourID = yourID;
            IDList = idList;
        }
    }

    [MessagePackObject]
    public struct DisconnectMessage : ITcpMessage { }

    [MessagePackObject]
    public struct SuccessMessage : ITcpMessage { }

    [MessagePackObject]
    public struct FailureMessage : ITcpMessage
    {
        [Key(0)]
        public Exception ExceptionMessage { get; set; }

        [SerializationConstructor]
        public FailureMessage(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}