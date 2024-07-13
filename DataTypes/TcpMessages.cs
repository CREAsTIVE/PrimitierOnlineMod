using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(UpdateVRMMessage))]
    public interface ITcpMessage { }

    [MessagePackObject]
    public struct JoinMessage : ITcpMessage
    {
        [Key(0)]
        public int ID { get; }

        [SerializationConstructor]
        public JoinMessage(int id)
        {
            ID = id;
        }
    }

    [MessagePackObject]
    public struct LeaveMessage : ITcpMessage
    {
        [Key(0)]
        public int ID { get; }

        [SerializationConstructor]
        public LeaveMessage(int id)
        {
            ID = id;
        }
    }

    [MessagePackObject]
    public struct UpdateVRMMessage : ITcpMessage
    {
        [Key(0)]
        public int ID { get; }
        [Key(1)]
        public byte[] Data { get; }

        [SerializationConstructor]
        public UpdateVRMMessage(int id, byte[] data)
        {
            ID = id;
            Data = data;
        }
    }
}