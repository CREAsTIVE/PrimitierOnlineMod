using MessagePack;
using YuchiGames.POM.Shared.DataObjects;
using System.Net.Sockets;
using System.ComponentModel;

namespace YuchiGames.POM.Shared
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(PlayerPositionMessage))]
    [Union(3, typeof(PlayerPositionUpdateMessage))]

    [Union(4, typeof(GroupUpdateMessage))]
    [Union(5, typeof(GroupDestroyedMessage))]
    [Union(6, typeof(GroupSetHostMessage))]
    [Union(7, typeof(GroupQuickUpdateMessage))]
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
        public PlayerPositionData PlayerPos { get; set; } = null!;
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

    // CUBES DATA:

    [MessagePackObject]
    public class GroupUpdateMessage : IGameDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Udp;

        [Key(0)]
        public Group GroupData { get; set; } = null!;
        [Key(1)]
        public ObjectUID GroupUID { get; set; }
    }

    [MessagePackObject]
    public class GroupDestroyedMessage : IGameDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Udp;
        [Key(0)]
        public ObjectUID GroupID { get; set; }
    }

    [MessagePackObject]
    public class GroupSetHostMessage : IGameDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Udp;

        [Key(0)]
        public ObjectUID GroupID { get; set; }
        [Key(1)]
        public int NewHostID { get; set; }
    }

    [MessagePackObject]
    public class GroupQuickUpdateMessage : IGameDataMessage
    {

        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Udp;

        [Key(0)]
        public ObjectUID ObjectUID { get; set; }
        [Key(1)]
        public SVector3 Position { get; set; }
        [Key(2)]
        public SQuaternion Rotation { get; set; }
        [Key(3)]
        public SVector3 Velocity { get; set; }
        [Key(4)]
        public SVector3 AngularVelocity { get; set; }
    }
}