﻿using MessagePack;
using System.Net.Sockets;

namespace YuchiGames.POM.DataTypes
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