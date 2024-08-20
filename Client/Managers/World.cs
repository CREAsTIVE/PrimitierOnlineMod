using Il2Cpp;
using UnityEngine;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Managers
{
    public static class World
    {
        private static int s_seed;
        public static int Seed
        {
            get => s_seed;
        }
        private static LocalWorldData s_worldData;
        public static LocalWorldData WorldData
        {
            get => s_worldData;
        }
        private static SaveAndLoad.SaveData s_saveData;
        public static SaveAndLoad.SaveData SaveData
        {
            get => s_saveData;
        }

        static World()
        {
            s_seed = 0;
            s_worldData = new LocalWorldData();
            s_saveData = new SaveAndLoad.SaveData();
        }

        public static void Load(LocalWorldData localWorldData)
        {
            s_worldData = localWorldData;
            s_seed = s_worldData.Seed;
            s_saveData.seed = s_worldData.Seed;
            s_saveData.time = s_worldData.Time;
            s_saveData.playerPos = DataConverter.ToVector3(s_worldData.PlayerPos);
            s_saveData.playerAngle = s_worldData.PlayerAngle;
            s_saveData.playerLife = s_worldData.PlayerLife;
            s_saveData.respawnPos = DataConverter.ToVector3(s_worldData.RespawnPos);
            s_saveData.respawnAngle = s_worldData.RespawnAngle;
            s_saveData.cameraPos = DataConverter.ToVector3(s_worldData.CameraPos);
            s_saveData.cameraRot = DataConverter.ToQuaternion(s_worldData.CameraRot);
            Il2CppSystem.Collections.Generic.List<Vector3> holsterPositions = new Il2CppSystem.Collections.Generic.List<Vector3>();
            holsterPositions.Add(DataConverter.ToVector3(s_worldData.HolsterLeftPos));
            holsterPositions.Add(DataConverter.ToVector3(s_worldData.HolsterRightPos));
            s_saveData.holsterPositions = holsterPositions;
            Il2CppSystem.Collections.Generic.List<SaveAndLoad.ChunkData> chunkDataList = new Il2CppSystem.Collections.Generic.List<SaveAndLoad.ChunkData>();
            foreach (Chunk chunk in s_worldData.Chunks)
            {
                SaveAndLoad.ChunkData chunkData = new SaveAndLoad.ChunkData
                {
                    x = chunk.X,
                    z = chunk.Z
                };
                Il2CppSystem.Collections.Generic.List<SaveAndLoad.GroupData> groupDataList = new Il2CppSystem.Collections.Generic.List<SaveAndLoad.GroupData>();
                foreach (Group group in chunk.Groups)
                {
                    SaveAndLoad.GroupData groupData = new SaveAndLoad.GroupData
                    {
                        pos = DataConverter.ToVector3(group.Position),
                        rot = DataConverter.ToQuaternion(group.Rotation)
                    };
                    Il2CppSystem.Collections.Generic.List<SaveAndLoad.CubeData> cubeDataList = new Il2CppSystem.Collections.Generic.List<SaveAndLoad.CubeData>();
                    foreach (Cube cube in group.Cubes)
                    {
                        SaveAndLoad.CubeData cubeData = new SaveAndLoad.CubeData
                        {
                            pos = DataConverter.ToVector3(cube.Position),
                            rot = DataConverter.ToQuaternion(cube.Rotation),
                            scale = DataConverter.ToVector3(cube.Scale),
                            lifeRatio = cube.LifeRatio,
                            anchor = (CubeConnector.Anchor)(int)cube.Anchor,
                            substance = (Il2Cpp.Substance)(int)cube.Substance,
                            name = (Il2Cpp.CubeName)(int)cube.Name,
                            connections = DataConverter.ToIl2CppList(cube.Connections),
                            temperature = cube.Temperature,
                            isBurning = cube.IsBurning,
                            burnedRatio = cube.BurnedRatio,
                            sectionState = (CubeAppearance.SectionState)(int)cube.SectionState
                        };
                        cubeData.uvOffset.right = DataConverter.ToVector2(cube.UVOffset.Right);
                        cubeData.uvOffset.left = DataConverter.ToVector2(cube.UVOffset.Left);
                        cubeData.uvOffset.top = DataConverter.ToVector2(cube.UVOffset.Top);
                        cubeData.uvOffset.bottom = DataConverter.ToVector2(cube.UVOffset.Bottom);
                        cubeData.uvOffset.front = DataConverter.ToVector2(cube.UVOffset.Front);
                        cubeData.uvOffset.back = DataConverter.ToVector2(cube.UVOffset.Back);
                        cubeData.behaviors = DataConverter.ToIl2CppList(cube.Behaviors);
                        cubeData.states = DataConverter.ToIl2CppList(cube.States);
                        cubeDataList.Add(cubeData);
                    }
                    groupData.cubes = cubeDataList;
                    groupDataList.Add(groupData);
                }
                chunkData.groups = groupDataList;
                chunkDataList.Add(chunkData);
            }
            s_saveData.chunks = chunkDataList;
        }
    }
}