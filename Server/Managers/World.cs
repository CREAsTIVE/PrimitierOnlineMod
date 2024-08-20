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
                s_worldData = JsonSerializer.Deserialize<GlobalWorldData>(json)
                    ?? throw new Exception("WorldData not found.");
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
            if (s_worldData is null)
                throw new Exception("WorldData not found.");
            bool isContains = s_worldData.UserIDs.Contains(userGUID);
            if (!isContains)
            {
                s_worldData.UserIDs.Add(userGUID);
                s_worldData.PlayerPositions.Add(new Position(0, 0, 0));
                s_worldData.PlayerAngles.Add(0);
                s_worldData.PlayerLives.Add(1000);
                s_worldData.RespawnPositions.Add(new Position(0, 0, 0));
                s_worldData.RespawnAngles.Add(0);
                s_worldData.CameraPositions.Add(new Position(0, 0, 0));
                s_worldData.CameraRotations.Add(new Rotation(0, 0, 0, 0));
                s_worldData.HolsterLeftPositions.Add(new Position(0, 0, 0));
                s_worldData.HolsterRightPositions.Add(new Position(0, 0, 0));
            }
            int index = s_worldData.UserIDs.ToList().IndexOf(userGUID);
            LocalWorldData localWorldData = new LocalWorldData(
                isFirstTime: !isContains,
                seed: s_worldData.Seed,
                time: s_worldData.Time,
                isTimeFrozen: s_worldData.IsTimeFrozen,
                playerPos: s_worldData.PlayerPositions[index],
                playerAngle: s_worldData.PlayerAngles[index],
                playerLife: s_worldData.PlayerLives[index],
                respawnPos: s_worldData.RespawnPositions[index],
                respawnAngle: s_worldData.RespawnAngles[index],
                cameraPos: s_worldData.CameraPositions[index],
                cameraRot: s_worldData.CameraRotations[index],
                holsterLeftPos: s_worldData.HolsterLeftPositions[index],
                holsterRightPos: s_worldData.HolsterRightPositions[index],
                chunks: s_worldData.Chunks,
                generatedChunks: s_worldData.GeneratedChunks);
            return localWorldData;
        }
    }
}