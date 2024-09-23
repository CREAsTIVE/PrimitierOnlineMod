using System.Text;
using System.Text.Json;
using System.IO.Compression;
using Shared.DataObjects;

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
                s_worldData = new GlobalWorldData(s_seed, 10, 1000);
            }
            // Save();
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
            if (!s_worldData.UserIDs.Contains(userGUID))
            {
                s_worldData.UserIDs.Add(userGUID);
                s_worldData.PlayerPositions.Add(new SVector3(130, 5, 130));
                s_worldData.PlayerAngles.Add(0);
                s_worldData.PlayerLives.Add(s_worldData.PlayerMaxLife);
                s_worldData.RespawnPositions.Add(new SVector3(130, 5, 130));
                s_worldData.RespawnAngles.Add(0);
                s_worldData.CameraPositions.Add(new SVector3(0, 0, 0));
                s_worldData.CameraRotations.Add(new SQuaternion(0, 0, 0, 0));
                s_worldData.HolsterLeftPositions.Add(new SVector3(-0.2f, 0, 0.12f));
                s_worldData.HolsterRightPositions.Add(new SVector3(0.2f, 0, 0.12f));
            }
            int index = s_worldData.UserIDs.ToList().IndexOf(userGUID);
            LocalWorldData localWorldData = new LocalWorldData(
                s_worldData.Seed,
                s_worldData.Time,
                s_worldData.PlayerPositions[index],
                s_worldData.PlayerAngles[index],
                s_worldData.PlayerMaxLife,
                s_worldData.PlayerLives[index],
                s_worldData.RespawnPositions[index],
                s_worldData.RespawnAngles[index],
                s_worldData.CameraPositions[index],
                s_worldData.CameraRotations[index],
                s_worldData.HolsterLeftPositions[index],
                s_worldData.HolsterRightPositions[index]);
            return localWorldData;
        }
    }
}