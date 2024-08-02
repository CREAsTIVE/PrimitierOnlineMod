namespace YuchiGames.POM.DataTypes
{
    public struct WorldData
    {
        public int Seed { get; }
        public float Time { get; set; }
        public bool IsTimeFrozen { get; set; }
        public List<string> UserGUID { get; set; }
        public List<Position> PlayerPositions { get; set; }
        public List<float> PlayerAngles { get; set; }
        public List<float> PlayerLives { get; set; }
        public List<Position> RespawnPositions { get; set; }
        public List<float> RespawnAngles { get; set; }
        public List<Position> CameraPos { get; set; }
        public List<Rotation> CameraRot { get; set; }
        public List<Position> HolsterLeftPositions { get; set; }
        public List<Position> HolsterRightPositions { get; set; }
        public List<Chunk> Chunks { get; set; }
        public List<List<bool>> GeneratedChunks { get; set; }

        public WorldData(int seed, int time, bool isTimeFrozen)
        {
            Seed = seed;
            Time = time;
            IsTimeFrozen = isTimeFrozen;
            UserGUID = new List<string>();
            PlayerPositions = new List<Position>();
            PlayerAngles = new List<float>();
            PlayerLives = new List<float>();
            RespawnPositions = new List<Position>();
            RespawnAngles = new List<float>();
            CameraPos = new List<Position>();
            CameraRot = new List<Rotation>();
            HolsterLeftPositions = new List<Position>();
            HolsterRightPositions = new List<Position>();
            Chunks = new List<Chunk>();
            GeneratedChunks = new List<List<bool>>();
        }
    }

    public struct Chunk
    {
        public int X { get; }
        public int Z { get; }
        public List<Group> Groups { get; set; }

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
            Groups = new List<Group>();
        }
    }

    public struct Group
    {
        public Position Position { get; set; }
        public Rotation Rotation { get; set; }
        public List<Cube> Cubes { get; set; }

        public Group(Position position, Rotation rotation)
        {
            Position = position;
            Rotation = rotation;
            Cubes = new List<Cube>();
        }
    }

    public struct Cube
    {
        public Position Position { get; set; }
        public Rotation Rotation { get; set; }
        public Scale Scale { get; set; }
        public float LifeRatio { get; set; }
        public Anchor Anchor { get; set; }
        public Substance Substance { get; set; }
        public CubeName Name { get; set; }
        public List<int> Connections { get; set; }
        public float Temperature { get; set; }
        public bool IsBurning { get; set; }
        public float BurnedRatio { get; set; }
        public SectionState SectionState { get; set; }
        public UVOffset UVOffset { get; set; }
        public List<string> Behaviors { get; set; }
        public List<string> States { get; set; }

        public Cube(Position position, Rotation rotation, Scale scale, float lifeRatio, Anchor anchor, Substance substance, CubeName name, float temperature, bool isBurning, float burnedRatio, SectionState sectionState, UVOffset uvOffset)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            LifeRatio = lifeRatio;
            Anchor = anchor;
            Substance = substance;
            Name = name;
            Connections = new List<int>();
            Temperature = temperature;
            IsBurning = isBurning;
            BurnedRatio = burnedRatio;
            SectionState = sectionState;
            UVOffset = uvOffset;
            Behaviors = new List<string>();
            States = new List<string>();
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
        Back = 0x2
    }

    public struct UVOffset
    {
        public Position2 Right { get; set; }
        public Position2 Left { get; set; }
        public Position2 Top { get; set; }
        public Position2 Bottom { get; set; }
        public Position2 Front { get; set; }
        public Position2 Back { get; set; }

        public UVOffset(Position2 right, Position2 left, Position2 top, Position2 bottom, Position2 front, Position2 back)
        {
            Right = right;
            Left = left;
            Top = top;
            Bottom = bottom;
            Front = front;
            Back = back;
        }
    }
}
