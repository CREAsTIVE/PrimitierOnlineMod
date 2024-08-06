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
        private static GlobalWorldData s_worldData;
        public static GlobalWorldData WorldData
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
                s_worldData = JsonSerializer.Deserialize<GlobalWorldData>(json);
                s_seed = s_worldData.Seed;
            }
            else
            {
                if (Program.Settings.Seed == 0)
                {
                    s_seed = new Random().Next(-2147483648, 2147483647);
                }
                else
                {
                    s_seed = Program.Settings.Seed;
                }

                s_worldData = new GlobalWorldData(s_seed, 10, false);
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

        public static LocalWorldData GetLocalWorldData(string userGUID)
        {
            LocalWorldData localWorldData = new LocalWorldData(s_seed, s_worldData.Time, s_worldData.IsTimeFrozen);
            int index = s_worldData.UserIDs.ToList().IndexOf(userGUID);
            localWorldData.PlayerPos = s_worldData.PlayerPositions[index];
            localWorldData.PlayerAngle = s_worldData.PlayerAngles[index];
            localWorldData.PlayerLife = s_worldData.PlayerLives[index];
            localWorldData.RespawnPos = s_worldData.RespawnPositions[index];
            localWorldData.RespawnAngle = s_worldData.RespawnAngles[index];
            localWorldData.CameraPos = s_worldData.CameraPositions[index];
            localWorldData.CameraRot = s_worldData.CameraRotations[index];
            localWorldData.HolsterLeftPos = s_worldData.HolsterLeftPositions[index];
            localWorldData.HolsterRightPos = s_worldData.HolsterRightPositions[index];
            localWorldData.Chunks = s_worldData.Chunks;
            localWorldData.GeneratedChunks = s_worldData.GeneratedChunks;
            return localWorldData;
        }
    }
}