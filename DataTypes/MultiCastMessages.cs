using MessagePack;
using System.Net.Sockets;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(UploadVRMMessage))]
    public interface IMultiMessage
    {
        public int FromID { get; }
        public ProtocolType Protocol { get; }
    }

    [MessagePackObject]
    public struct JoinMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; }
        [Key(1)]
        public int JoinID { get; }

        [SerializationConstructor]
        public JoinMessage(int fromID, int joinID)
        {
            FromID = fromID;
            Protocol = ProtocolType.Tcp;
            JoinID = joinID;
        }
    }

    [MessagePackObject]
    public struct LeaveMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; }
        [Key(1)]
        public int LeaveID { get; }

        [SerializationConstructor]
        public LeaveMessage(int fromID, int leaveID)
        {
            FromID = fromID;
            Protocol = ProtocolType.Tcp;
            LeaveID = leaveID;
        }
    }

    [MessagePackObject]
    public struct UploadVRMMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; }
        [Key(1)]
        public byte[] Data { get; }

        [SerializationConstructor]
        public UploadVRMMessage(int fromID, byte[] data)
        {
            FromID = fromID;
            Protocol = ProtocolType.Tcp;
            Data = data;
        }
    }
}