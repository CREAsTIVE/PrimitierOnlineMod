namespace YuchiGames.POM.DataTypes
{
    public struct VRMFile
    {
        public byte[] Data { get; }

        public VRMFile(byte[] data)
        {
            Data = data;
        }
    }
}