using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(ConnectMessage))]
    [Union(1, typeof(SuccessConnectionMessage))]
    [Union(2, typeof(JoinedMessage))]
    [Union(3, typeof(DisconnectMessage))]
    [Union(4, typeof(SuccessMessage))]
    [Union(5, typeof(FailureMessage))]
    public interface ITcpMessage { }

    [MessagePackObject]
    public struct ConnectMessage : ITcpMessage
    {
        [Key(0)]
        public string Version { get; }
        [Key(1)]
        public int TcpPort { get; }
        [Key(2)]
        public int UdpPort { get; }

        [SerializationConstructor]
        public ConnectMessage(string version, int tcpPort, int udpPort)
        {
            Version = version;
            TcpPort = tcpPort;
            UdpPort = udpPort;
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

    [MessagePackObject]
    public struct DisconnectMessage : ITcpMessage { }

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
}