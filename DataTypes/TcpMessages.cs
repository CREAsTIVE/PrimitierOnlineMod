using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(JoinMessage))]
    [Union(1, typeof(LeaveMessage))]
    [Union(2, typeof(UploadVRMMessage))]
    [Union(3, typeof(ServerInfoMessage))]
    public interface ITcpMessage { }

    [MessagePackObject]
    public struct JoinMessage : ITcpMessage
    {
        [Key(0)]
        public int ID { get; }

        [SerializationConstructor]
        public JoinMessage(int id)
        {
            ID = id;
        }
    }

    [MessagePackObject]
    public struct LeaveMessage : ITcpMessage
    {
        [Key(0)]
        public int ID { get; }

        [SerializationConstructor]
        public LeaveMessage(int id)
        {
            ID = id;
        }
    }

    [MessagePackObject]
    public struct UploadVRMMessage : ITcpMessage
    {
        [Key(0)]
        public int ID { get; }
        [Key(1)]
        public byte[] Data { get; }

        [SerializationConstructor]
        public UploadVRMMessage(int id, byte[] data)
        {
            ID = id;
            Data = data;
        }
    }

    [MessagePackObject]
    public struct ServerInfoMessage : ITcpMessage
    {
        [Key(0)]
        public int MaxPlayers { get; }
        [Key(1)]
        public byte[][] AvatarData { get; }

        [SerializationConstructor]
        public ServerInfoMessage(int maxPlayers, byte[][] avatarData)
        {
            MaxPlayers = maxPlayers;
            AvatarData = avatarData;
        }
    }
}