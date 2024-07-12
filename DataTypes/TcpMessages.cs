using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(JoinedMessage))]
    [Union(1, typeof(LeftMessage))]
    public interface ITcpMessage { }

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
    public struct LeftMessage : ITcpMessage
    {
        [Key(0)]
        public int ID { get; }

        [SerializationConstructor]
        public LeftMessage(int id)
        {
            ID = id;
        }
    }
}