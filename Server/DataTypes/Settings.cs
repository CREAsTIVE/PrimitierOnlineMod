namespace YuchiGames.POM.DataTypes
{
    public class ServerSettings
    {
        public int Port { get; }
        public int MaxPlayers { get; }
        public int Seed { get; }
        public Serilog Serilog { get; }

        public ServerSettings()
        {
            Port = 54162;
            MaxPlayers = 16;
            Seed = 0;
            Serilog = new Serilog(
                new List<string>
                {
                    "Serilog.Settings.Configuration",
                    "Serilog.Sinks.Console",
                    "Serilog.Sinks.File"
                },
                "Debug",
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
        public List<string> Using { get; }
        public string MinimumLevel { get; }
        public List<WriteTo> WriteTo { get; }

        public Serilog(List<string> using_, string minimumLevel, List<WriteTo> writeTo)
        {
            Using = using_;
            MinimumLevel = minimumLevel;
            WriteTo = writeTo;
        }
    }
    public class WriteTo
    {
        public string Name { get; }
        public Args Args { get; }

        public WriteTo(string name, Args args = null)
        {
            Name = name;
            Args = args;
        }
    }
    public class Args
    {
        public string Path { get; }
        public string RollingInterval { get; }

        public Args(string path, string rollingInterval)
        {
            Path = path;
            RollingInterval = rollingInterval;
        }
    }
}
