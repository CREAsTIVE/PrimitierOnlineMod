using MessagePack;
using System.Net.Sockets;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(UploadVRMMessage))]
    [Union(3, typeof(AvatarPositionMessage))]
    [Union(4, typeof(GeneratedChunkMessage))]
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
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [Key(1)]
        public int JoinID { get; }

        [SerializationConstructor]
        public JoinMessage(int fromID, int joinID)
        {
            FromID = fromID;
            JoinID = joinID;
        }
    }

    [MessagePackObject]
    public struct LeaveMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [Key(1)]
        public int LeaveID { get; }

        [SerializationConstructor]
        public LeaveMessage(int fromID, int leaveID)
        {
            FromID = fromID;
            LeaveID = leaveID;
        }
    }

    [MessagePackObject]
    public struct UploadVRMMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [Key(1)]
        public byte[] Data { get; }

        [SerializationConstructor]
        public UploadVRMMessage(int fromID, byte[] data)
        {
            FromID = fromID;
            Data = data;
        }
    }

    [MessagePackObject]
    public struct AvatarPositionMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Udp;
        [Key(1)]
        public VRMPosData VRMPosData { get; }

        [SerializationConstructor]
        public AvatarPositionMessage(int fromID, VRMPosData vrmPosData)
        {
            FromID = fromID;
            VRMPosData = vrmPosData;
        }
    }

    [MessagePackObject]
    public struct GeneratedChunkMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Udp;
        [Key(1)]
        public Chunk ChunkData { get; }

        [SerializationConstructor]
        public GeneratedChunkMessage(int fromID, Chunk chunkData)
        {
            FromID = fromID;
            ChunkData = chunkData;
        }
    }
}