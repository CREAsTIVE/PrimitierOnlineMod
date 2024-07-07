using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(SuccessMessage))]
    [Union(1, typeof(FailureMessage))]
    [Union(2, typeof(ConnectMessage))]
    [Union(3, typeof(SuccessConnectionMessage))]
    [Union(4, typeof(JoinedMessage))]
    public interface ITcpMessage { }

    [MessagePackObject]
    public struct SuccessMessage : ITcpMessage { }

    [MessagePackObject]
    public struct FailureMessage : ITcpMessage
    {
        [Key(0)]
        public Exception ExceptionMessage { get; }

        [SerializationConstructor]
        public FailureMessage(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }

    [MessagePackObject]
    public struct ConnectMessage : ITcpMessage
    {
        [Key(0)]
        public string Version { get; }
        [Key(1)]
        public int Port { get; }

        [SerializationConstructor]
        public ConnectMessage(string version, int port)
        {
            Version = version;
            Port = port;
        }
    }

    [MessagePackObject]
    public struct SuccessConnectionMessage : ITcpMessage
    {
        [Key(0)]
        public int YourID { get; }
        [Key(1)]
        public int[] IDList { get; }

        [SerializationConstructor]
        public SuccessConnectionMessage(int yourID, int[] idList)
        {
            YourID = yourID;
            IDList = idList;
        }
    }

    [MessagePackObject]
    public struct JoinedMessage : ITcpMessage
    {
        [Key(0)]
        public int ID { get; }

        [SerializationConstructor]
        public JoinedMessage(int id)
        {
            ID = id;
        }
    }
}