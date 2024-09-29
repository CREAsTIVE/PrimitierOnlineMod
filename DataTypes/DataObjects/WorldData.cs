using MessagePack;

namespace YuchiGames.POM.Shared.DataObjects
{
    [MessagePackObject]
    public class PlayerData
    {
        [Key(0)]
        public SVector3 Position { get; set; }

        [Key(1)]
        public float Life { get; set; }

        [Key(2)]
        public SVector3 LeftHolsterPosition { get; set; }
        [Key(3)]
        public SVector3 RightHolsterPosition { get; set; }
    }
    public class WorldData
    {
        

        public int Seed { get; set; }
        public float Time { get; set; }

        // UUID: PlayerData
        public Dictionary<string, PlayerData> PlayersData { get; set; } = new();
        public Dictionary<SVector2Int, Chunk> ChunksData { get; set; } = new();
    }

    [MessagePackObject]
    public class LocalWorldData
    {
        [Key(0)]
        public int Seed { get; set; }
        [Key(1)]
        public float Time { get; set; }

        [Key(2)]
        public PlayerData Player { get; set; } = null!;

        [SerializationConstructor]
        private LocalWorldData() { }

        public static LocalWorldData From(WorldData worldData, string playerGUID) =>
            new LocalWorldData()
            {
                Player = worldData.PlayersData[playerGUID],
                Seed = worldData.Seed,
                Time = worldData.Time,
            };
    }

    [MessagePackObject]
    public class Chunk
    {
        [Key(0)]
        public List<Group> Groups { get; set; } = new();
    }

    [MessagePackObject]
    public class Group
    {
        [Key(0)]
        public SVector3 Position { get; set; }
        [Key(1)]
        public SQuaternion Rotation { get; set; }
        [Key(2)]
        public List<Cube> Cubes { get; set; } = new();
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
        public List<int> Connections { get; set; } = new();
        [Key(8)]
        public float Temperature { get; set; }
        [Key(9)]
        public bool IsBurning { get; set; }
        [Key(10)]
        public float BurnedRatio { get; set; }
        [Key(11)]
        public SectionState SectionState { get; set; }
        [Key(12)]
        public UVOffset UVOffset { get; set; } = new();
        [Key(13)]
        public List<string> Behaviors { get; set; } = new();
        [Key(14)]
        public List<string> States { get; set; } = new();
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
