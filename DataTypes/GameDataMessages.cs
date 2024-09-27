using MessagePack;
using YuchiGames.POM.Shared.DataObjects;
using System.Net.Sockets;

namespace YuchiGames.POM.Shared
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(PlayerPositionMessage))]
    public interface IGameDataMessage
    {
        public int FromID { get; }
        public ProtocolType Protocol { get; }
    }

    [MessagePackObject]
    public class JoinMessage : IGameDataMessage
    {
        [IgnoreMember]
        public int FromID { get; } = -1;
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [Key(0)]
        public int JoinID { get; }

        [SerializationConstructor]
        public JoinMessage(int joinID)
        {
            JoinID = joinID;
        }
    }

    [MessagePackObject]
    public class LeaveMessage : IGameDataMessage
    {
        [IgnoreMember]
        public int FromID { get; } = -1;
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [Key(0)]
        public int LeaveID { get; }

        [SerializationConstructor]
        public LeaveMessage(int leaveID)
        {
            LeaveID = leaveID;
        }
    }

    [MessagePackObject]
    public class PlayerPositionMessage : IGameDataMessage
    {
        [Key(0)]
        public int FromID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Udp;
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