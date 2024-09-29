using MessagePack;
using YuchiGames.POM.Shared.DataObjects;
using System.Net.Sockets;

namespace YuchiGames.POM.Shared
{
    [Union(0, typeof(RequestServerInfoMessage))]
    [Union(1, typeof(ServerInfoMessage))]
    [Union(2, typeof(RequestNewChunkDataMessage))]
    [Union(3, typeof(ChunkDataMessage))]
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
        public int MaxPlayers { get; set; }
        [Key(1)]
        public LocalWorldData WorldData { get; set; } = null!;
        [Key(2)]
        public bool IsDayNightCycle { get; set; }
        [Key(3)]
        public int UID { get; set; }
    }


    // WARNING:
    // If that packet was sended to client, it should generate chunk data and then return that data back to server
    // If that packet was sended to server, it should try to find that chunk inside save data and return in, or request for generating that chunk
    [MessagePackObject]
    public class RequestNewChunkDataMessage : IServerDataMessage
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Tcp;

        [Key(0)]
        public SVector2Int ChunkPos { get; set; }
    }
    [MessagePackObject]
    public class ChunkDataMessage : IServerDataMessage // TODO: Add chunk owner (who will claim host for every object inside that chunk)
    {
        [IgnoreMember]
        public ProtocolType Protocol => ProtocolType.Tcp;

        [Key(0)]
        public Chunk Chunk { get; set; } = null!;
        [Key(1)]
        public SVector2Int Pos { get; set; }
    }
}