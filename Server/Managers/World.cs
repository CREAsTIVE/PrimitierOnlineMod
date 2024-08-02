using System.Text;
using System.Text.Json;
using System.IO.Compression;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Server.Managers
{
    public static class World
    {
        private static int s_seed;
        public static int Seed
        {
            get => s_seed;
        }
        private static WorldData s_worldData;
        public static WorldData WorldData
        {
            get => s_worldData;
        }

        static World()
        {
            if (File.Exists("WorldData.world"))
            {
                byte[] jsonBytes;
                using (FileStream fs = new FileStream("WorldData.world", FileMode.Open))
                using (GZipStream gz = new GZipStream(fs, CompressionMode.Decompress))
                {
                    jsonBytes = new byte[fs.Length];
                    gz.Read(jsonBytes, 0, jsonBytes.Length);
                }
                string json = Encoding.UTF8.GetString(jsonBytes);
                s_worldData = JsonSerializer.Deserialize<WorldData>(json);
                s_seed = s_worldData.Seed;
            }
            else
            {
                if (Program.Settings.Seed == 0)
                {
                    s_seed = new Random().Next();
                }
                else
                {
                    s_seed = Program.Settings.Seed;
                }

                s_worldData = new WorldData(s_seed, 10, false);
            }
        }

        public static void Save()
        {
            string json = JsonSerializer.Serialize(s_worldData);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            using (FileStream fs = new FileStream("WorldData.world", FileMode.Create))
            using (GZipStream gz = new GZipStream(fs, CompressionMode.Compress))
            {
                gz.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }
    }
}