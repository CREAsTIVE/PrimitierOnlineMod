using Il2Cpp;
using YuchiGames.POM.Shared.DataObjects;
using UnityEngine;
using YuchiGames.POM.Shared;

namespace YuchiGames.POM.Client.Managers
{
    public static class World
    {
        public static LocalWorldData WorldData { get; private set; } = new();
        public static SaveAndLoad.SaveData SaveData { get; private set; } = new();

        public static void LoadWorldData(LocalWorldData localWorldData)
        {
            WorldData = localWorldData;
            SaveData = new()
            {
                seed = WorldData.Seed,
                time = WorldData.Time,
                playerMaxLife = WorldData.PlayerMaxLife,
                playerPos = WorldData.Player.Position.ToUnity(),
                playerAngle = 0f,
                playerLife = WorldData.Player.Life,
                respawnPos = WorldData.RespawnPosition.ToUnity(),
                respawnAngle = 0f,
                holsterPositions = new Il2CppSystem.Collections.Generic.List<Vector3>().Apply(l =>
                {
                    l.Add(WorldData.Player.LeftHolsterPosition.ToUnity());
                    l.Add(WorldData.Player.RightHolsterPosition.ToUnity());
                }),
                chunks = new(),
            };
            /*s_worldData = localWorldData;
            s_seed = s_worldData.Seed;
            s_saveData.seed = s_worldData.Seed;
            s_saveData.time = s_worldData.Time;
            s_saveData.playerMaxLife = s_worldData.PlayerMaxLife;
            s_saveData.playerPos = DataConverter.ToUnity(s_worldData.PlayerPos);
            s_saveData.playerAngle = s_worldData.PlayerAngle;
            s_saveData.playerLife = s_worldData.PlayerLife;
            s_saveData.respawnPos = DataConverter.ToUnity(s_worldData.RespawnPos);
            s_saveData.respawnAngle = s_worldData.RespawnAngle;
            s_saveData.cameraPos = DataConverter.ToUnity(s_worldData.CameraPos);
            s_saveData.cameraRot = DataConverter.ToUnity(s_worldData.CameraRot);
            Il2CppSystem.Collections.Generic.List<Vector3> holsterPositions = new Il2CppSystem.Collections.Generic.List<Vector3>();
            holsterPositions.Add(DataConverter.ToUnity(s_worldData.HolsterLeftPos));
            holsterPositions.Add(DataConverter.ToUnity(s_worldData.HolsterRightPos));
            s_saveData.holsterPositions = holsterPositions;*/
        }
    }
}