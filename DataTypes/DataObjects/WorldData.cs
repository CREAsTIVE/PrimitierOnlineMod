using MessagePack;

namespace YuchiGames.POM.Shared.DataObjects
{
    public class GlobalWorldData
    {
        public int Seed { get; init; }
        public float Time { get; set; }
        public HashSet<string> UserIDs { get; init; }
        public List<SVector3> PlayerPositions { get; init; }
        public List<float> PlayerAngles { get; init; }
        public float PlayerMaxLife { get; init; }
        public List<float> PlayerLives { get; init; }
        public List<SVector3> RespawnPositions { get; init; }
        public List<float> RespawnAngles { get; init; }
        public List<SVector3> CameraPositions { get; init; }
        public List<SQuaternion> CameraRotations { get; init; }
        public List<SVector3> HolsterLeftPositions { get; init; }
        public List<SVector3> HolsterRightPositions { get; init; }
        public List<Chunk> Chunks { get; init; }
        public HashSet<SVector2> GeneratedChunks { get; init; }

        public GlobalWorldData(
            int seed,
            float time,
            float playerMaxLife)
        {
            Seed = seed;
            Time = time;
            UserIDs = new HashSet<string>();
            PlayerPositions = new List<SVector3>();
            PlayerAngles = new List<float>();
            PlayerMaxLife = playerMaxLife;
            PlayerLives = new List<float>();
            RespawnPositions = new List<SVector3>();
            RespawnAngles = new List<float>();
            CameraPositions = new List<SVector3>();
            CameraRotations = new List<SQuaternion>();
            HolsterLeftPositions = new List<SVector3>();
            HolsterRightPositions = new List<SVector3>();
            Chunks = new List<Chunk>();
            GeneratedChunks = new HashSet<SVector2>();
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
        public SVector3 PlayerPos { get; set; }
        [Key(3)]
        public float PlayerAngle { get; set; }
        [Key(4)]
        public float PlayerMaxLife { get; set; }
        [Key(5)]
        public float PlayerLife { get; set; }
        [Key(6)]
        public SVector3 RespawnPos { get; set; }
        [Key(7)]
        public float RespawnAngle { get; set; }
        [Key(8)]
        public SVector3 CameraPos { get; set; }
        [Key(9)]
        public SQuaternion CameraRot { get; set; }
        [Key(10)]
        public SVector3 HolsterLeftPos { get; set; }
        [Key(11)]
        public SVector3 HolsterRightPos { get; set; }

        [SerializationConstructor]
        public LocalWorldData(
            int seed,
            float time,
            SVector3 playerPos,
            float playerAngle,
            float playerMaxLife,
            float playerLife,
            SVector3 respawnPos,
            float respawnAngle,
            SVector3 cameraPos,
            SQuaternion cameraRot,
            SVector3 holsterLeftPos,
            SVector3 holsterRightPos)
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
            PlayerPos = new SVector3(130, 5, 130);
            PlayerAngle = 0;
            PlayerMaxLife = 1000;
            PlayerLife = 1000;
            RespawnPos = new SVector3(130, 5, 130);
            RespawnAngle = 0;
            CameraPos = new SVector3(0, 0, 0);
            CameraRot = new SQuaternion(0, 0, 0, 0);
            HolsterLeftPos = new SVector3(-0.2f, 0, 0.12f);
            HolsterRightPos = new SVector3(0.2f, 0, 0.12f);
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
        public SVector3 Position { get; set; }
        [Key(1)]
        public SQuaternion Rotation { get; set; }
        [Key(2)]
        public List<Cube> Cubes { get; init; }

        [SerializationConstructor]
        public Group(SVector3 position, SQuaternion rotation, List<Cube> cubes)
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
        public SVector3 Position { get; set; }
        [Key(1)]
        public SQuaternion Rotation { get; set; }
        [Key(2)]
        public SVector3 Scale { get; set; }
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
            SVector3 position,
            SQuaternion rotation,
            SVector3 scale,
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
        public SVector2 Right { get; set; }
        [Key(1)]
        public SVector2 Left { get; set; }
        [Key(2)]
        public SVector2 Top { get; set; }
        [Key(3)]
        public SVector2 Bottom { get; set; }
        [Key(4)]
        public SVector2 Front { get; set; }
        [Key(5)]
        public SVector2 Back { get; set; }
    }
}
