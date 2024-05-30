using MessagePack;
using YuchiGames.POM.Server.Data.Models;

namespace YuchiGames.POM.Server.Data.UdpMessages
{
    interface IMessage { }

    [MessagePackObject]
    public class SendPlayerPosMessage : IMessage
    {
        [Key(0)]
        public string PlayerID { get; set; }
        [Key(1)]
        public PlayerModel PlayerPos { get; set; }

        [SerializationConstructor]
        public SendPlayerPosMessage(string playerID, PlayerModel playerPos)
        {
            PlayerID = playerID;
            PlayerPos = playerPos;
        }
    }
}