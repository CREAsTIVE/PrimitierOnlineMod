using MessagePack;

namespace YuchiGames.POM.DataTypes
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