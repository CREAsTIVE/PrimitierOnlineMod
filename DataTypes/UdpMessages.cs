using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(SendPlayerPosMessage))]
    [Union(1, typeof(TestMessage))]
    public interface IUdpMessage
    {
        int ID { get; }
    }

    [MessagePackObject]
    public struct SendPlayerPosMessage : IUdpMessage
    {
        [Key(0)]
        public int ID { get; }
        [Key(1)]
        public bool IsVRMBody { get; }
        [Key(2)]
        public BaseBody BaseBody { get; }
        [Key(3)]
        public VRMBody VrmBody { get; }

        [SerializationConstructor]
        public SendPlayerPosMessage(int id, bool isVRMBody, BaseBody baseBody, VRMBody vrmBody)
        {
            ID = id;
            IsVRMBody = isVRMBody;
            BaseBody = baseBody;
            VrmBody = vrmBody;
        }
    }

    [MessagePackObject]
    public struct TestMessage : IUdpMessage
    {
        [Key(0)]
        public int ID { get; }

        [SerializationConstructor]
        public TestMessage(int id)
        {
            ID = id;
        }
    }
}