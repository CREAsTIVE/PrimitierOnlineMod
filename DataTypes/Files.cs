using MessagePack;

namespace YuchiGames.POM.Data
{
    [Union(0, typeof(VRMFile))]
    [Union(1, typeof(MapFile))]
    public interface IFile { }

    [MessagePackObject]
    public struct VRMFile : IFile
    {
        [Key(0)]
        public byte[] Data { get; set; }

        [SerializationConstructor]
        public VRMFile(byte[] data)
        {
            Data = data;
        }
    }

    [MessagePackObject]
    public struct MapFile : IFile
    {
        [Key(0)]
        public byte[] Data { get; }
        [IgnoreMember]
        public int MaxLength { get; }

        [SerializationConstructor]
        public MapFile(byte[] data, int maxLength)
        {
            Data = data;
            MaxLength = maxLength;
        }
    }
}