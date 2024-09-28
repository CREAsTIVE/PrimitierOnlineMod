using MessagePack;
using YuchiGames.POM.Shared.DataObjects;
using System.Net.Sockets;

namespace YuchiGames.POM.Shared
{
    [Union(0, typeof(RequestServerInfoMessage))]
    [Union(1, typeof(ServerInfoMessage))]
    public interface IServerDataMessage : IDataMessage
    {
        [IgnoreMember]
        byte IDataMessage.Channel => 0x01;
    }

    [MessagePackObject]
    public class RequestServerInfoMessage : IServerDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Tcp;


        [Key(0)]
        public string UserGUID { get; }

        [SerializationConstructor]
        public RequestServerInfoMessage(string userGUID)
        {
            UserGUID = userGUID;
        }
    }

    [MessagePackObject]
    public class ServerInfoMessage : IServerDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Tcp;


        [Key(0)]
        public int MaxPlayers { get; }
        [Key(1)]
        public LocalWorldData WorldData { get; }
        [Key(2)]
        public bool IsDayNightCycle { get; }

        [SerializationConstructor]
        public ServerInfoMessage(int maxPlayers, LocalWorldData worldData, bool isDayNightCycle)
        {
            MaxPlayers = maxPlayers;
            WorldData = worldData;
            IsDayNightCycle = isDayNightCycle;
        }
    }
}