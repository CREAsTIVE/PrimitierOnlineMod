using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    public class GlobalWorldData
    {
        public int Seed { get; init; }
        public float Time { get; set; }
        public HashSet<string> UserIDs { get; init; }
        public List<Position> PlayerPositions { get; init; }
        public List<float> PlayerAngles { get; init; }
        public float PlayerMaxLife { get; init; }
        public List<float> PlayerLives { get; init; }
        public List<Position> RespawnPositions { get; init; }
        public List<float> RespawnAngles { get; init; }
        public List<Position> CameraPositions { get; init; }
        public List<Rotation> CameraRotations { get; init; }
        public List<Position> HolsterLeftPositions { get; init; }
        public List<Position> HolsterRightPositions { get; init; }
        public List<Chunk> Chunks { get; init; }
        public HashSet<Position2> GeneratedChunks { get; init; }

        public GlobalWorldData(
            int seed,
            float time,
            float playerMaxLife)
        {
            Seed = seed;
            Time = time;
            UserIDs = new HashSet<string>();
            PlayerPositions = new List<Position>();
            PlayerAngles = new List<float>();
            PlayerMaxLife = playerMaxLife;
            PlayerLives = new List<float>();
            RespawnPositions = new List<Position>();
            RespawnAngles = new List<float>();
            CameraPositions = new List<Position>();
            CameraRotations = new List<Rotation>();
            HolsterLeftPositions = new List<Position>();
            HolsterRightPositions = new List<Position>();
            Chunks = new List<Chunk>();
            GeneratedChunks = new HashSet<Position2>();
        }
    }

    [MessagePackObject]
    public class LocalWorldData
    {
        [Key(0)]
        public int Seed { get; init; }
        [Key(1)]
        public float Time { get; set; }
        [Key(2)]
        public Position PlayerPos { get; set; }
        [Key(3)]
        public float PlayerAngle { get; set; }
        [Key(4)]
        public float PlayerMaxLife { get; set; }
        [Key(5)]
        public float PlayerLife { get; set; }
        [Key(6)]
        public Position RespawnPos { get; set; }
        [Key(7)]
        public float RespawnAngle { get; set; }
        [Key(8)]
        public Position CameraPos { get; set; }
        [Key(9)]
        public Rotation CameraRot { get; set; }
        [Key(10)]
        public Position HolsterLeftPos { get; set; }
        [Key(11)]
        public Position HolsterRightPos { get; set; }

        [SerializationConstructor]
        public LocalWorldData(
            int seed,
            float time,
            Position playerPos,
            float playerAngle,
            float playerMaxLife,
            float playerLife,
            Position respawnPos,
            float respawnAngle,
            Position cameraPos,
            Rotation cameraRot,
            Position holsterLeftPos,
            Position holsterRightPos)
        {
            Seed = seed;
            Time = time;
            PlayerPos = playerPos;
            PlayerAngle = playerAngle;
            PlayerMaxLife = playerMaxLife;
            PlayerLife = playerLife;
            RespawnPos = respawnPos;
            RespawnAngle = respawnAngle;
            CameraPos = cameraPos;
            CameraRot = cameraRot;
            HolsterLeftPos = holsterLeftPos;
            HolsterRightPos = holsterRightPos;
        }

        public LocalWorldData()
        {
            Seed = 0;
            Time = 0;
            PlayerPos = new Position(130, 5, 130);
            PlayerAngle = 0;
            PlayerMaxLife = 1000;
            PlayerLife = 1000;
            RespawnPos = new Position(130, 5, 130);
            RespawnAngle = 0;
            CameraPos = new Position(0, 0, 0);
            CameraRot = new Rotation(0, 0, 0, 0);
            HolsterLeftPos = new Position(-0.2f, 0, 0.12f);
            HolsterRightPos = new Position(0.2f, 0, 0.12f);
        }
    }

    [MessagePackObject]
    public class Chunk
    {
        [Key(0)]
        public int X { get; init; }
        [Key(1)]
        public int Z { get; init; }
        [Key(2)]
        public List<Group> Groups { get; init; }

        [SerializationConstructor]
        public Chunk(int x, int z, List<Group> groups)
        {
            X = x;
            Z = z;
            Groups = groups;
        }
    }

    [MessagePackObject]
    public class Group
    {
        [Key(0)]
        public Position Position { get; set; }
        [Key(1)]
        public Rotation Rotation { get; set; }
        [Key(2)]
        public List<Cube> Cubes { get; init; }

        [SerializationConstructor]
        public Group(Position position, Rotation rotation, List<Cube> cubes)
        {
            Position = position;
            Rotation = rotation;
            Cubes = cubes;
        }
    }

    [MessagePackObject]
    public class Cube
    {
        [Key(0)]
        public Position Position { get; set; }
        [Key(1)]
        public Rotation Rotation { get; set; }
        [Key(2)]
        public Scale Scale { get; set; }
        [Key(3)]
        public float LifeRatio { get; set; }
        [Key(4)]
        public Anchor Anchor { get; set; }
        [Key(5)]
        public Substance Substance { get; set; }
        [Key(6)]
        public CubeName Name { get; set; }
        [Key(7)]
        public List<int> Connections { get; init; }
        [Key(8)]
        public float Temperature { get; set; }
        [Key(9)]
        public bool IsBurning { get; set; }
        [Key(10)]
        public float BurnedRatio { get; set; }
        [Key(11)]
        public SectionState SectionState { get; set; }
        [Key(12)]
        public UVOffset UVOffset { get; init; }
        [Key(13)]
        public List<string> Behaviors { get; init; }
        [Key(14)]
        public List<string> States { get; init; }

        [SerializationConstructor]
        public Cube(
            Position position,
            Rotation rotation,
            Scale scale,
            float lifeRatio,
            Anchor anchor,
            Substance substance,
            CubeName name,
            List<int> connections,
            float temperature,
            bool isBurning,
            float burnedRatio,
            SectionState sectionState,
            UVOffset uvOffset,
            List<string> behaviors,
            List<string> states)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            LifeRatio = lifeRatio;
            Anchor = anchor;
            Substance = substance;
            Name = name;
            Connections = connections;
            Temperature = temperature;
            IsBurning = isBurning;
            BurnedRatio = burnedRatio;
            SectionState = sectionState;
            UVOffset = uvOffset;
            Behaviors = behaviors;
            States = states;
        }
    }

    public enum Anchor
    {
        Free,
        Temporary,
        Permanent
    }

    public enum Substance
    {
        Stone,
        Wood,
        Iron,
        Grass,
        Leaf,
        Slime,
        CookedSlime,
        Pyrite,
        RedSlime,
        CookedRedSlime,
        Monument,
        Hematite,
        Wheat,
        WheatStalk,
        DryGrass,
        Bread,
        AncientAlloy,
        AncientPlastic,
        AncientDrone,
        AncientEngine,
        Gold,
        Silver,
        Clay,
        Brick,
        Cactus,
        Niter,
        Sulfur,
        Gunpowder,
        GreenSlime,
        Apple,
        Pinecone,
        Rubberwood,
        RubberSeed,
        RawRubber,
        Rubber,
        ConiferWood,
        Ice,
        QuartzSand,
        Glass,
        Helium,
        TungstenOre,
        Tungsten,
        SolarCell,
        LED,
        ElectricMotor,
        Battery,
        AncientLightweightPlastic,
        YellowSlime,
        CookedYellowSlime,
        RepairFiller,
        AncientSuicideDrone,
        BossCore,
        MixedAcid,
        Nitrocellulose,
        RocketEngine,
        MoonRock,
        MoonMonument
    }

    public enum CubeName
    {
        None,
        RespawnPoint,
        BeamTurret,
        HomingBeamTurret,
        DroneSpawner,
        SuicideDroneSpawner,
        BearingOuter,
        BearingAxis,
        EngineBody,
        EngineAxis,
        ElectricMotorBody,
        ElectricMotorAxis,
        SlimeAlive,
        RedSlimeAlive,
        GreenSlimeAlive,
        YellowSlimeAlive
    }

    public enum SectionState
    {
        Right = 1,
        Left = 2,
        Top = 4,
        Bottom = 8,
        Front = 0x10,
        Back = 0x20
    }

    [MessagePackObject]
    public class UVOffset
    {
        [Key(0)]
        public Position2 Right { get; set; }
        [Key(1)]
        public Position2 Left { get; set; }
        [Key(2)]
        public Position2 Top { get; set; }
        [Key(3)]
        public Position2 Bottom { get; set; }
        [Key(4)]
        public Position2 Front { get; set; }
        [Key(5)]
        public Position2 Back { get; set; }
    }
}
