using MessagePack;

namespace YuchiGames.POM.Data
{
    [Union(0, typeof(ConnectMessage))]
    [Union(1, typeof(DisconnectMessage))]
    [Union(2, typeof(SuccessConnectionMessage))]
    [Union(3, typeof(SuccessMessage))]
    [Union(4, typeof(FailureMessage))]
    public interface ITcpMessage { }

    /// <summary>
    /// ConnectMessage is sent by the client to the server to establish a connection.
    /// </summary>
    [MessagePackObject]
    public struct ConnectMessage : ITcpMessage
    {
        [Key(0)]
        public string Version { get; set; }
        [Key(1)]
        public string UserName { get; set; }

        [SerializationConstructor]
        public ConnectMessage(string version, string userName)
        {
            Version = version;
            UserName = userName;
        }
    }

    [MessagePackObject]
    public struct DisconnectMessage : ITcpMessage { }

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