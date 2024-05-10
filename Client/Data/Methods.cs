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
    public class Connect
    {
        [Key(0)]
        public string Name { get; set; } = "Connect";
        [Key(1)]
        public string Version { get; set; }

        [SerializationConstructor]
        public Connect(string version)
        {
            Version = version;
        }
    }

    [MessagePackObject]
    public class Disconnect
    {
        [Key(0)]
        public string Name { get; set; } = "Disconnect";
    }

    [MessagePackObject]
    public class Success
    {
        [Key(0)]
        public string Name { get; set; } = "Success";
    }

    [MessagePackObject]
    public class Failure
    {
        [Key(0)]
        public string Name { get; set; } = "Failure";
        [Key(1)]
        public Exception ExceptionMessage { get; set; }

        [SerializationConstructor]
        public Failure(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}