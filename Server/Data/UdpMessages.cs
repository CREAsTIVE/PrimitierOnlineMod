﻿using MessagePack;

namespace YuchiGames.POM.Server.Data
{
    [Union(0, typeof(SendPlayerPosMessage))]
    public interface IUdpMessage { }

    [MessagePackObject]
    public class SendPlayerPosMessage : IUdpMessage
    {
        [Key(0)]
        public PlayerModel PlayerPos { get; set; }

        [SerializationConstructor]
        public SendPlayerPosMessage(PlayerModel playerPos)
        {
            PlayerPos = playerPos;
        }
    }
}