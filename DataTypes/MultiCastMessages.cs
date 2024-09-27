using MessagePack;
using YuchiGames.POM.Shared.DataObjects;
using System.Net.Sockets;

namespace YuchiGames.POM.Shared
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(PlayerPositionMessage))]
    public interface IMultiMessage
    {
        public int FromID { get; }
        public ProtocolType Protocol { get; }
        public bool IsLarge { get; }
    }

    [MessagePackObject]
    public class JoinMessage : IMultiMessage
    {
        [IgnoreMember]
        public int FromID { get; } = -1;
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [IgnoreMember]
        public bool IsLarge { get; } = false;
        [Key(0)]
        public int JoinID { get; }

        [SerializationConstructor]
        public JoinMessage(int joinID)
        {
            JoinID = joinID;
        }
    }

    [MessagePackObject]
    public class LeaveMessage : IMultiMessage
    {
        [IgnoreMember]
        public int FromID { get; } = -1;
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [IgnoreMember]
        public bool IsLarge { get; } = false;
        [Key(0)]
        public int LeaveID { get; }

        [SerializationConstructor]
        public LeaveMessage(int leaveID)
        {
            LeaveID = leaveID;
        }
    }

    [MessagePackObject]
    public class PlayerPositionMessage : IMultiMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Udp;
        [IgnoreMember]
        public bool IsLarge { get; } = false;
        [Key(1)]
        public PlayerPositionData PlayerPos { get; }

        [SerializationConstructor]
        public PlayerPositionMessage(int fromID, PlayerPositionData playerPos)
        {
            FromID = fromID;
            PlayerPos = playerPos;
        }
    }
}