using MessagePack;
using YuchiGames.POM.Shared.DataObjects;
using System.Net.Sockets;

namespace YuchiGames.POM.Shared
{
    [Union(0, typeof(RequestServerInfoMessage))]
    [Union(1, typeof(ServerInfoMessage))]
    public interface IServerDataMessage
    {
        public int FromID { get; }
        public int ToID { get; }
        public ProtocolType Protocol { get; }
        // public bool IsLarge { get; }
    }

    [MessagePackObject]
    public class RequestServerInfoMessage : IServerDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Tcp;

        [IgnoreMember]
        public int ToID { get; } = -1;

        [Key(0)]
        public int FromID { get; }
        [Key(1)]
        public string UserGUID { get; }

        [SerializationConstructor]
        public RequestServerInfoMessage(int fromID, string userGUID)
        {
            FromID = fromID;
            UserGUID = userGUID;
        }
    }

    [MessagePackObject]
    public class ServerInfoMessage : IServerDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Tcp;

        [IgnoreMember]
        public int FromID { get; } = -1;
        [Key(0)]
        public int ToID { get; }
        [Key(1)]
        public int MaxPlayers { get; }
        [Key(2)]
        public LocalWorldData WorldData { get; }
        [Key(3)]
        public bool IsDayNightCycle { get; }

        [SerializationConstructor]
        public ServerInfoMessage(int toID, int maxPlayers, LocalWorldData worldData, bool isDayNightCycle)
        {
            ToID = toID;
            MaxPlayers = maxPlayers;
            WorldData = worldData;
            IsDayNightCycle = isDayNightCycle;
        }
    }
}