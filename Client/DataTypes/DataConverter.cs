using Il2Cpp;
using UnityEngine;
using YuchiGames.POM.Client.Assets;

namespace YuchiGames.POM.DataTypes
{
    public static class DataConverter
    {
        public static Vector3 ToVector3(Position position)
        {
            return new Vector3(position.X, position.Y, position.Z);
        }

        public static Vector3 ToVector3(Scale scale)
        {
            return new Vector3(scale.X, scale.Y, scale.Z);
        }

        public static Vector3 ToVector3(PosRot posRot)
        {
            return new Vector3(posRot.Position.X, posRot.Position.Y, posRot.Position.Z);
        }

        public static Vector2 ToVector2(Position2 position2)
        {
            return new Vector2(position2.X, position2.Y);
        }

        public static Vector2Int ToVector2Int(Position2Int position2)
        {
            return new Vector2Int(position2.X, position2.Y);
        }

        public static Quaternion ToQuaternion(Rotation rotation)
        {
            return new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
        }

        public static Quaternion ToQuaternion(PosRot posRot)
        {
            return new Quaternion(posRot.Rotation.X, posRot.Rotation.Y, posRot.Rotation.Z, posRot.Rotation.W);
        }

        public static Transform ToTransform(PosRot posRot)
        {
            Transform transform = new Transform();
            transform.position = ToVector3(posRot);
            transform.rotation = ToQuaternion(posRot);
            return transform;
        }

        public static Position ToPosition(Vector3 vector3)
        {
            return new Position(vector3.x, vector3.y, vector3.z);
        }

        public static Position ToPosition(Transform transform)
        {
            return new Position(transform.position.x, transform.position.y, transform.position.z);
        }

        public static Position2 ToPosition2(Vector2 vector2)
        {
            return new Position2(vector2.x, vector2.y);
        }

        public static Position2Int ToPosition2Int(Vector2Int vector2)
        {
            return new Position2Int(vector2.x, vector2.y);
        }

        public static Rotation ToRotation(Quaternion quaternion)
        {
            return new Rotation(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static Rotation ToRotation(Transform transform)
        {
            return new Rotation(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        }

        public static Scale ToScale(Vector3 vector3)
        {
            return new Scale(vector3.x, vector3.y, vector3.z);
        }

        public static PosRot ToPosRot(Transform transform)
        {
            return new PosRot(new Position(transform.position.x, transform.position.y, transform.position.z),
                new Rotation(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w));
        }

        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(List<T> systemList)
        {
            Il2CppSystem.Collections.Generic.List<T> il2cppList = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (T item in systemList)
            {
                il2cppList.Add(item);
            }
            return il2cppList;
        }

        public static List<T> ToSystemList<T>(Il2CppSystem.Collections.Generic.List<T> il2cppList)
        {
            List<T> systemList = new List<T>();
            foreach (T item in il2cppList)
            {
                systemList.Add(item);
            }
            return systemList;
        }

        public static Chunk ToChunk(SaveAndLoad.ChunkData chunkData)
        {
            List<Group> groups = new List<Group>();
            foreach (SaveAndLoad.GroupData groupData in chunkData.groups)
            {
                List<Cube> cubes = new List<Cube>();
                foreach (SaveAndLoad.CubeData cubeData in groupData.cubes)
                {
                    UVOffset uvOffset = new UVOffset()
                    {
                        Right = ToPosition2(cubeData.uvOffset.right),
                        Left = ToPosition2(cubeData.uvOffset.left),
                        Top = ToPosition2(cubeData.uvOffset.top),
                        Bottom = ToPosition2(cubeData.uvOffset.bottom),
                        Front = ToPosition2(cubeData.uvOffset.front),
                        Back = ToPosition2(cubeData.uvOffset.back)
                    };
                    Cube cube = new Cube(
                        ToPosition(cubeData.pos),
                        ToRotation(cubeData.rot),
                        ToScale(cubeData.scale),
                        cubeData.lifeRatio,
                        (Anchor)cubeData.anchor,
                        (Substance)cubeData.substance,
                        (CubeName)cubeData.name,
                        ToSystemList<int>(cubeData.connections),
                        cubeData.temperature,
                        cubeData.isBurning,
                        cubeData.burnedRatio,
                        (SectionState)cubeData.sectionState,
                        uvOffset,
                        ToSystemList<string>(cubeData.behaviors),
                        ToSystemList<string>(cubeData.states)
                        );
                    cubes.Add(cube);
                }
                Group group = new Group(
                    ToPosition(groupData.pos),
                    ToRotation(groupData.rot),
                    cubes
                    );
                groups.Add(group);
            }
            Chunk chunk = new Chunk(chunkData.x, chunkData.z, groups);
            return chunk;
        }

        public static SaveAndLoad.ChunkData ToChunkData(Chunk chunk)
        {
            List<SaveAndLoad.GroupData> groupDataList = new List<SaveAndLoad.GroupData>();
            foreach (Group group in chunk.Groups)
            {
                List<SaveAndLoad.CubeData> cubeDataList = new List<SaveAndLoad.CubeData>();
                foreach (Cube cube in group.Cubes)
                {
                    CubeAppearance.UVOffset uvOffset = new CubeAppearance.UVOffset()
                    {
                        right = ToVector2(cube.UVOffset.Right),
                        left = ToVector2(cube.UVOffset.Left),
                        top = ToVector2(cube.UVOffset.Top),
                        bottom = ToVector2(cube.UVOffset.Bottom),
                        front = ToVector2(cube.UVOffset.Front),
                        back = ToVector2(cube.UVOffset.Back)
                    };
                    SaveAndLoad.CubeData cubeData = new SaveAndLoad.CubeData()
                    {
                        pos = ToVector3(cube.Position),
                        rot = ToQuaternion(cube.Rotation),
                        scale = ToVector3(cube.Scale),
                        lifeRatio = cube.LifeRatio,
                        anchor = (CubeConnector.Anchor)cube.Anchor,
                        substance = (Il2Cpp.Substance)cube.Substance,
                        name = (Il2Cpp.CubeName)cube.Name,
                        connections = ToIl2CppList<int>(cube.Connections),
                        temperature = cube.Temperature,
                        isBurning = cube.IsBurning,
                        burnedRatio = cube.BurnedRatio,
                        sectionState = (CubeAppearance.SectionState)cube.SectionState,
                        uvOffset = uvOffset,
                        behaviors = ToIl2CppList<string>(cube.Behaviors),
                        states = ToIl2CppList<string>(cube.States)
                    };
                    cubeDataList.Add(cubeData);
                }
                SaveAndLoad.GroupData groupData = new SaveAndLoad.GroupData()
                {
                    pos = ToVector3(group.Position),
                    rot = ToQuaternion(group.Rotation),
                    cubes = ToIl2CppList(cubeDataList)
                };
                groupDataList.Add(groupData);
            }
            SaveAndLoad.ChunkData chunkData = new SaveAndLoad.ChunkData()
            {
                x = chunk.X,
                z = chunk.Z,
                groups = ToIl2CppList(groupDataList)
            };
            return chunkData;
        }

        public static Group ToGroup(SaveAndLoad.GroupData groupData)
        {
            List<Cube> cubes = new List<Cube>();
            foreach (SaveAndLoad.CubeData cubeData in groupData.cubes)
            {
                UVOffset uvOffset = new UVOffset()
                {
                    Right = ToPosition2(cubeData.uvOffset.right),
                    Left = ToPosition2(cubeData.uvOffset.left),
                    Top = ToPosition2(cubeData.uvOffset.top),
                    Bottom = ToPosition2(cubeData.uvOffset.bottom),
                    Front = ToPosition2(cubeData.uvOffset.front),
                    Back = ToPosition2(cubeData.uvOffset.back)
                };
                Cube cube = new Cube(
                    ToPosition(cubeData.pos),
                    ToRotation(cubeData.rot),
                    ToScale(cubeData.scale),
                    cubeData.lifeRatio,
                    (Anchor)cubeData.anchor,
                    (Substance)cubeData.substance,
                    (CubeName)cubeData.name,
                    ToSystemList<int>(cubeData.connections),
                    cubeData.temperature,
                    cubeData.isBurning,
                    cubeData.burnedRatio,
                    (SectionState)cubeData.sectionState,
                    uvOffset,
                    ToSystemList<string>(cubeData.behaviors),
                    ToSystemList<string>(cubeData.states)
                    );
                cubes.Add(cube);
            }
            Group group = new Group(
                ToPosition(groupData.pos),
                ToRotation(groupData.rot),
                cubes
                );
            return group;
        }
    }
}