using MessagePack;

namespace YuchiGames.POM.Data
{
    [Union(0, typeof(SendPlayerPosMessage))]
    public interface IUdpMessage { }

    [MessagePackObject]
    public struct SendPlayerPosMessage : IUdpMessage
    {
        [Key(0)]
        public int PlayerId { get; set; }
        [Key(1)]
        public PlayerModel PlayerPos { get; set; }

        [SerializationConstructor]
        public SendPlayerPosMessage(int playerId, PlayerModel playerPos)
        {
            PlayerId = playerId;
            PlayerPos = playerPos;
        }
    }
}