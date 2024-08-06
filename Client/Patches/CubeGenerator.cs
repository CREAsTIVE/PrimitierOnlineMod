using Il2Cpp;
using HarmonyLib;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateChunk))]
    class CubeGenerator_GenerateChunk
    {
        static void Prefix()
        {
            Log.Debug("CubeGenerator.GenerateChunk() Prefix called");
        }
    }

    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateNewChunk))]
    class CubeGenerator_GenerateNewChunk
    {
        static void Prefix()
        {
            Log.Debug("CubeGenerator.GenerateNewChunk() Prefix called");
        }
    }

    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateNearChunks))]
    class CubeGenerator_GenerateNearChunks
    {
        static void Prefix()
        {
            Log.Debug("CubeGenerator.GenerateNearChunks() Prefix called");
        }
    }

    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateSavedChunk))]
    class CubeGenerator_GenerateSavedChunk
    {
        static void Prefix()
        {
            Log.Debug("CubeGenerator.GenerateSavedChunk() Prefix called");
        }
    }

    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.DestroyFarChunks))]
    class CubeGenerator_DestroyFarChunks
    {
        static void Prefix()
        {
            Log.Debug("CubeGenerator.DestroyFarChunks() Prefix called");
        }
    }

    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.DestroyChunks))]
    class CubeGenerator_DestroyChunks
    {
        static void Prefix()
        {
            Log.Debug("CubeGenerator.DestroyChunks() Prefix called");
        }
    }
}