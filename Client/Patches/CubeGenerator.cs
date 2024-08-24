using Il2Cpp;
using HarmonyLib;

namespace YuchiGames.POM.Client.Patches
{
    //[HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateNearChunks))]
    //class CubeGenerator_GenerateNearChunks
    //{
    //    private static void Prefix()
    //    {
    //        Log.Debug("CubeGenerator.GenerateNearChunks() Prefix called");
    //    }
    //}

    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateChunk))]
    class CubeGenerator_GenerateChunk
    {
        private static bool Prefix()
        {
            Log.Debug("CubeGenerator.GenerateChunk() Prefix called");
            return false;
        }
    }

    //[HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateNewChunk))]
    //class CubeGenerator_GenerateNewChunk
    //{
    //    private static void Prefix()
    //    {
    //        Log.Debug("CubeGenerator.GenerateNewChunk() Prefix called");
    //    }
    //}

    //[HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateSavedChunk))]
    //class CubeGenerator_GenerateSavedChunk
    //{
    //    private static void Prefix()
    //    {
    //        Log.Debug("CubeGenerator.GenerateSavedChunk() Prefix called");
    //    }
    //}

    //[HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.DestroyFarChunks))]
    //class CubeGenerator_DestroyFarChunks
    //{
    //    private static void Prefix()
    //    {
    //        Log.Debug("CubeGenerator.DestroyFarChunks() Prefix called");
    //    }
    //}

    //[HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.DestroyChunks))]
    //class CubeGenerator_DestroyChunks
    //{
    //    private static void Prefix()
    //    {
    //        Log.Debug("CubeGenerator.DestroyChunks() Prefix called");
    //    }
    //}
}