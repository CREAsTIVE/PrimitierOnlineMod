using MessagePack;
using System.Net.Sockets;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(RequestServerInfoMessage))]
    [Union(1, typeof(ServerInfoMessage))]
    public interface IUniMessage
    {
        public int FromID { get; }
        public int ToID { get; }
        public ProtocolType Protocol { get; }
    }

    [MessagePackObject]
    public struct RequestServerInfoMessage : IUniMessage
    {
        [Key(0)]
        public int FromID { get; }
        [Key(1)]
        public int ToID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [Key(2)]
        public string UserGUID { get; }

        [SerializationConstructor]
        public RequestServerInfoMessage(int fromID, int toID, string userGUID)
        {
            FromID = fromID;
            ToID = toID;
            UserGUID = userGUID;
        }
    }

    [MessagePackObject]
    public struct ServerInfoMessage : IUniMessage
    {
        [Key(0)]
        public int FromID { get; }
        [Key(1)]
        public int ToID { get; }
        [IgnoreMember]
        public ProtocolType Protocol { get; } = ProtocolType.Tcp;
        [Key(2)]
        public int MaxPlayers { get; }
        [Key(3)]
        public byte[][] AvatarData { get; }
        [Key(4)]
        public LocalWorldData WorldData { get; }

        [SerializationConstructor]
        public ServerInfoMessage(int fromID, int toID, int maxPlayers, byte[][] avatarData, LocalWorldData worldData)
        {
            FromID = fromID;
            ToID = toID;
            MaxPlayers = maxPlayers;
            AvatarData = avatarData;
            WorldData = worldData;
        }
    }
}