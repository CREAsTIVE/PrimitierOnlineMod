using MessagePack;

namespace YuchiGames.POM.Server.Data
{
    [Union(0, typeof(VRMFile))]
    [Union(1, typeof(MapFile))]
    public interface IFile { }

    [MessagePackObject]
    public class VRMFile : IFile
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
    public class MapFile : IFile
    {
        [Key(0)]
        public byte[] Data { get; }
        [IgnoreMember]
        public int MaxLength { get; }

        [SerializationConstructor]
        public MapFile(string path, int maxLength)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File not found", path);
            }
            if (fileInfo.Length > maxLength)
            {
                throw new InvalidDataException("File is too large");
            }

            using (FileStream fileStream = fileInfo.OpenRead())
            {
                Data = new byte[fileStream.Length];
                int numBytesToRead = (int)fileStream.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fileStream.Read(Data, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
            }
        }
    }
}