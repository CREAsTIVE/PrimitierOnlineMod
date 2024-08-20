using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [MessagePackObject]
    public struct Position
    {
        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }
        [Key(2)]
        public float Z { get; set; }

        [SerializationConstructor]
        public Position(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    [MessagePackObject]
    public struct Position2
    {
        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }

        [SerializationConstructor]
        public Position2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    [MessagePackObject]
    public struct Rotation
    {
        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }
        [Key(2)]
        public float Z { get; set; }
        [Key(3)]
        public float W { get; set; }

        [SerializationConstructor]
        public Rotation(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }

    [MessagePackObject]
    public struct Scale
    {
        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }
        [Key(2)]
        public float Z { get; set; }

        [SerializationConstructor]
        public Scale(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    [MessagePackObject]
    public struct PosRot
    {
        [Key(0)]
        public Position Position { get; set; }
        [Key(1)]
        public Rotation Rotation { get; set; }

        [SerializationConstructor]
        public PosRot(Position position, Rotation rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
