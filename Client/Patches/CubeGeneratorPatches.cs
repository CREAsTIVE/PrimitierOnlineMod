
using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using YuchiGames.POM.Client.Managers;
using YuchiGames.POM.Shared;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateChunk))]
    public static class CubeGeneratorGenerateChunkPatch
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
    public static class CubeGeneratorDestroyChunkPatch
    {
        public static void Prefix(Il2CppSystem.Collections.Generic.List<Vector2Int> __0)
        {
            // Collect all owned cubes and transfer ownership to another player
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

    [HarmonyPatch(typeof(Grabber), nameof(Grabber.Grab))]
    public static class GrabberPatch
    {
        // __0 - is remote
        public static void Postfix(Grabber __instance, bool __0)
        {
            if (__instance.bond != null)
                Network.ClaimHost(__instance.bond.GetComponent<GroupSyncerComponent>().UID);
        }
    }

    [HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
    public static class Patch_Il2CppDetourMethodPatcher
    {
        public static bool Prefix(System.Exception ex)
        {
            MelonLogger.Error("During invoking native->managed trampoline", ex);
            return false;
        }
    }

    [HarmonyPatch(typeof(RigidbodyManager), nameof(RigidbodyManager.Start))]
    public static class RigidbodyManagerPatch
    {
        public static void Postfix(RigidbodyManager __instance)
        {
            if (__instance.gameObject.GetComponent<GroupSyncerComponent>() == null)
                __instance.gameObject.AddComponent<GroupSyncerComponent>();
        }
    }
}
