using MessagePack;

namespace YuchiGames.POM.Client.Data.Methods
{
    [MessagePackObject]
    public class MethodsName
    {
        [Key(0)]
        public string Name { get; set; }

        [SerializationConstructor]
        public MethodsName(string name)
        {
            Name = name;
        }
    }

    [MessagePackObject]
    public class ConnectMethod
    {
        [Key(0)]
        public string Name { get; set; } = "ConnectMethod";
        [Key(1)]
        public string Version { get; set; }

        [SerializationConstructor]
        public ConnectMethod(string version)
        {
            Version = version;
        }
    }

    [MessagePackObject]
    public class DisconnectMethod
    {
        [Key(0)]
        public string Name { get; set; } = "DisconnectMethod";
    }

    [MessagePackObject]
    public class SuccessMethod
    {
        [Key(0)]
        public string Name { get; set; } = "SuccessMethod";
    }

    [MessagePackObject]
    public class FailureMethod
    {
        [Key(0)]
        public string Name { get; set; } = "FailureMethod";
        [Key(1)]
        public Exception ExceptionMessage { get; set; }

        [SerializationConstructor]
        public FailureMethod(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}