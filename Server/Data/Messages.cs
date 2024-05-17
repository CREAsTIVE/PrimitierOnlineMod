using MessagePack;
using YuchiGames.POM.Server.Data.Models;

namespace YuchiGames.POM.Server.Data.Messages
{
    interface IMessage
    {
        string Name { get; }
    }

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
    public class ConnectMessage : IMessage
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
    public class DisconnectMessage : IMessage
    {
        [Key(0)]
        public string Name { get; } = "DisconnectMessage";
    }

    [MessagePackObject]
    public class SendPlayerPosMessage : IMessage
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
    public class SuccessMessage : IMessage
    {
        [Key(0)]
        public string Name { get; } = "SuccessMessage";
        [Key(1)]
        public string MessageName { get; set; }

        [SerializationConstructor]
        public SuccessMessage(string messageName)
        {
            MessageName = messageName;
        }
    }

    [MessagePackObject]
    public class FailureMessage : IMessage
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