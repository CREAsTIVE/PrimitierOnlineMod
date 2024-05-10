using MessagePack;

namespace YuchiGames.POM.Server.Data.Files
{
    [MessagePackObject]
    public class VRMData
    {
        [Key(0)]
        public string Name { get; set; } = "VRMData";
        [Key(1)]
        public byte[] Data { get; set; }

        [SerializationConstructor]
        public VRMData(byte[] data)
        {
            Data = data;
        }
    }

    [MessagePackObject]
    public class MapData
    {
        [Key(0)]
        public string Name { get; set; } = "MapData";
        [Key(1)]
        public byte[] Data { get; set; }

        [SerializationConstructor]
        public MapData(byte[] data)
        {
            Data = data;
        }
    }
}