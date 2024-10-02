
using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using YuchiGames.POM.Client.Managers;
using YuchiGames.POM.Shared;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateChunk))]
    public static class CubeGenerator_GenerateChunk_Patch
    {
        public static bool Prefix(Vector2Int __0)
        {
            if (Network.IsConnected)
            {
                Network.Send(new RequestNewChunkDataMessage() { ChunkPos = __0.ToShared() });

                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.DestroyChunks))]
    public static class CubeGenerator_DestroyChunk_Patch
    {
        public static void Prefix(Il2CppSystem.Collections.Generic.List<Vector2Int> __0)
        {
            foreach (var obj in Network.SyncedObjects.Values)
                if (__0.Contains(CubeGenerator.WorldToChunkPos(obj.transform.position)))
                    obj.Unload();
        } 
        public static void Postfix(Il2CppSystem.Collections.Generic.List<Vector2Int> __0)
        {
            if (Network.IsConnected)
            {
                foreach (var pos in __0)
                {
                    Network.Send(new ChunkUnloadMessage()
                    {
                        Pos = pos.ToShared(),
                    });

                    Network.Send(new SavedChunkDataMessage()
                    {
                        Chunk = DataConverter.ToChunk(SaveAndLoad.chunkDict[pos]),
                        Pos = pos.ToShared()
                    });
                }
            }
        }
    }
}
