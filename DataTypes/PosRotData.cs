namespace YuchiGames.POM.DataTypes
{
    public struct Position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Position(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public struct Position2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Position2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public struct Rotation
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Rotation(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }

    public struct Scale
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Scale(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public struct PosRot
    {
        public Position Position { get; set; }
        public Rotation Rotation { get; set; }

        public PosRot(Position position, Rotation rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
