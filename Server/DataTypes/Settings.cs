namespace YuchiGames.POM.Shared
{
    public class ServerSettings
    {
        public int Port { get; init; }
        public int MaxPlayers { get; init; }
        public int Seed { get; init; }
        public bool DayNightCycle { get; init; }
        public Serilog Serilog { get; init; }

        public ServerSettings()
        {
            Port = 54162;
            MaxPlayers = 16;
            Seed = 0;
            DayNightCycle = true;
            Serilog = new Serilog(
                new List<string>
                {
                    "Serilog.Settings.Configuration",
                    "Serilog.Sinks.Console",
                    "Serilog.Sinks.File"
                },
                "Information",
                new List<WriteTo>
                {
                    new WriteTo("Console"),
                    new WriteTo("File", new Args("Logs/.log", "Day"))
                }
            );
        }
    }

    public class Serilog
    {
        public List<string> Using { get; init; }
        public string MinimumLevel { get; init; }
        public List<WriteTo> WriteTo { get; init; }

        public Serilog(List<string> using_, string minimumLevel, List<WriteTo> writeTo)
        {
            Using = using_;
            MinimumLevel = minimumLevel;
            WriteTo = writeTo;
        }
    }
    public class WriteTo
    {
        public string Name { get; init; }
        public Args Args { get; init; }

        public WriteTo(string name, Args args = null!) // FIXME: make field nullable
        {
            Name = name;
            Args = args;
        }
    }
    public class Args
    {
        public string Path { get; init; }
        public string RollingInterval { get; init; }

        public Args(string path, string rollingInterval)
        {
            Path = path;
            RollingInterval = rollingInterval;
        }
    }
}
