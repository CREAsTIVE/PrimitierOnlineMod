using MessagePack;

namespace YuchiGames.POM.Shared.DataObjects
{
    [MessagePackObject]
    public class AuthData
    {
        [Key(0)]
        public string Version { get; }

        [SerializationConstructor]
        public AuthData(string version)
        {
            Version = version;
        }
    }
}