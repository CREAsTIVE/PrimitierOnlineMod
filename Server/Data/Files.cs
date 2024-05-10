using MessagePack;

namespace YuchiGames.POM.Server.Data.Files
{
    [MessagePackObject]
    public class VRMFile
    {
        [Key(0)]
        public string Name { get; set; } = "VRMFile";
        [Key(1)]
        public byte[] Data { get; set; }

        [SerializationConstructor]
        public VRMFile(byte[] data)
        {
            Data = data;
        }
    }

    [MessagePackObject]
    public class MapFile
    {
        [Key(0)]
        public string Name { get; set; } = "MapFile";
        [Key(1)]
        public byte[] Data { get; set; }

        [SerializationConstructor]
        public MapFile(byte[] data)
        {
            Data = data;
        }
    }
}