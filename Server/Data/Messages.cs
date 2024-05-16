using MessagePack;
using YuchiGames.POM.Server.Data.Models;

namespace YuchiGames.POM.Server.Data.Messages
{
    [MessagePackObject]
    public class MessagesName
    {
        [Key(0)]
        public string Name { get; }

        [SerializationConstructor]
        public MessagesName(string name)
        {
            Name = name;
        }
    }

    [MessagePackObject]
    public class ConnectMessage
    {
        [Key(0)]
        public string Name { get; } = "ConnectMessage";
        [Key(1)]
        public string Version { get; set; }

        [SerializationConstructor]
        public ConnectMessage(string version)
        {
            Version = version;
        }
    }

    [MessagePackObject]
    public class DisconnectMessage
    {
        [Key(0)]
        public string Name { get; } = "DisconnectMessage";
    }

    [MessagePackObject]
    public class SendPlayerPosMessage
    {
        [Key(0)]
        public string Name { get; } = "SendPlayerPosMessage";
        [Key(1)]
        public string PlayerID { get; set; }
        [Key(2)]
        public PlayerModel PlayerPos { get; set; }

        [SerializationConstructor]
        public SendPlayerPosMessage(string playerID, PlayerModel playerPos)
        {
            PlayerID = playerID;
            PlayerPos = playerPos;
        }
    }

    [MessagePackObject]
    public class SuccessMessage
    {
        [Key(0)]
        public string Name { get; } = "SuccessMessage";
    }

    [MessagePackObject]
    public class FailureMessage
    {
        [Key(0)]
        public string Name { get; } = "FailureMessage";
        [Key(1)]
        public Exception ExceptionMessage { get; set; }

        [SerializationConstructor]
        public FailureMessage(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}