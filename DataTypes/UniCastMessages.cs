using MessagePack;
using System.Net.Sockets;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(ServerInfoMessage))]
    public interface IUniMessage
    {
        public int FromID { get; }
        public int ToID { get; }
        public ProtocolType Protocol { get; }
    }

    [MessagePackObject]
    public struct ServerInfoMessage : IUniMessage
    {
        [Key(0)]
        public int FromID { get; }
        [Key(1)]
        public int ToID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; }
        [Key(2)]
        public int MaxPlayers { get; }
        [Key(3)]
        public byte[][] AvatarData { get; }

        [SerializationConstructor]
        public ServerInfoMessage(int fromID, int toID, int maxPlayers, byte[][] avatarData)
        {
            FromID = fromID;
            ToID = toID;
            Protocol = ProtocolType.Tcp;
            MaxPlayers = maxPlayers;
            AvatarData = avatarData;
        }
    }
}