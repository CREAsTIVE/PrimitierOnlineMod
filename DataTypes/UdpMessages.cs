using MessagePack;

namespace YuchiGames.POM.Data
{
    [Union(0, typeof(SendPlayerPosMessage))]
    public interface IUdpMessage { }

    [MessagePackObject]
    public struct SendPlayerPosMessage : IUdpMessage
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