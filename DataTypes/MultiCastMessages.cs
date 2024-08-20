using MessagePack;
using System.Net.Sockets;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(UploadVRMMessage))]
    [Union(3, typeof(AvatarPositionMessage))]
    public interface IMultiMessage
    {
        public int FromID { get; }
        public ProtocolType Protocol { get; }
        public bool IsLarge { get; }
    }

    /// <summary>
    /// -> Server multi send
    /// -> Client receive
    /// </summary>
    [MessagePackObject]
    public class JoinMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [IgnoreMember]
        public bool IsLarge { get; } = false;
        [Key(1)]
        public int JoinID { get; }

        [SerializationConstructor]
        public JoinMessage(int fromID, int joinID)
        {
            FromID = fromID;
            JoinID = joinID;
        }
    }

    /// <summary>
    /// -> Server multi send
    /// -> Client receive
    /// </summary>
    [MessagePackObject]
    public class LeaveMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [IgnoreMember]
        public bool IsLarge { get; } = false;
        [Key(1)]
        public int LeaveID { get; }

        [SerializationConstructor]
        public LeaveMessage(int fromID, int leaveID)
        {
            FromID = fromID;
            LeaveID = leaveID;
        }
    }

    /// <summary>
    /// -> Client send
    /// -> Server receive
    /// -> Server multi send
    /// -> Client receive
    [MessagePackObject]
    public class UploadVRMMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [IgnoreMember]
        public bool IsLarge { get; } = true;
        [Key(1)]
        public byte[] VRMData { get; }

        [SerializationConstructor]
        public UploadVRMMessage(int fromID, byte[] data)
        {
            FromID = fromID;
            VRMData = data;
        }
    }

    /// <summary>
    /// -> Client send
    /// -> Server receive
    /// -> Server multi send
    /// -> Client receive
    /// </summary>
    [MessagePackObject]
    public class AvatarPositionMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Udp;
        [IgnoreMember]
        public bool IsLarge { get; } = false;
        [Key(1)]
        public VRMPosData VRMPosData { get; }

        [SerializationConstructor]
        public AvatarPositionMessage(int fromID, VRMPosData vrmPosData)
        {
            FromID = fromID;
            VRMPosData = vrmPosData;
        }
    }
}