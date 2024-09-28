using MessagePack;
using YuchiGames.POM.Shared.DataObjects;
using System.Net.Sockets;

namespace YuchiGames.POM.Shared
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(PlayerPositionMessage))]
    [Union(3, typeof(PlayerPositionUpdateMessage))]
    public interface IGameDataMessage : IDataMessage
    {
        [IgnoreMember]
        byte IDataMessage.Channel => 0x00;
    }

    [MessagePackObject]
    public class JoinMessage : IGameDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Tcp;


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
        public ProtocolType Protocol => ProtocolType.Tcp;


        [Key(0)]
        public int LeaveID { get; }

        [SerializationConstructor]
        public LeaveMessage(int leaveID)
        {
            LeaveID = leaveID;
        }
    }

    // Client->Server self pos
    [MessagePackObject]
    public class PlayerPositionMessage : IGameDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Udp;

        [Key(0)]
        public PlayerPositionData PlayerPos { get; }

        [SerializationConstructor]
        public PlayerPositionMessage(PlayerPositionData playerPos)
        {
            PlayerPos = playerPos;
        }
    }

    // Server->Client player pos
    [MessagePackObject]
    public class PlayerPositionUpdateMessage : IGameDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Udp;

        [Key(0)]
        public PlayerPositionData PlayerPos { get; }

        [Key(1)]
        public int PlayerID { get; }

        [SerializationConstructor]
        public PlayerPositionUpdateMessage(PlayerPositionData playerPos, int playerID)
        {
            PlayerPos = playerPos;
            this.PlayerID = playerID;
        }
    }
}