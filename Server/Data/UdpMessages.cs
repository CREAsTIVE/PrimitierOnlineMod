using MessagePack;
using YuchiGames.POM.Server.Data.Models;
using YuchiGames.POM.Server.Data.TcpMessages;

namespace YuchiGames.POM.Server.Data.UdpMessages
{
    [Union(0, typeof(SuccessConnectionMessage))]
    public interface IUdpMessage { }

    [MessagePackObject]
    public class SendPlayerPosMessage : IUdpMessage
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