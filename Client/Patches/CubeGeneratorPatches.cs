
using HarmonyLib;
using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YuchiGames.POM.Client.Managers;
using YuchiGames.POM.Shared;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.GenerateChunk))]
    public static class CubeGeneratorGenerateCubePatch
    {
        public static bool Prefix(Vector2Int pos)
        {
            Network.Send(new RequestNewChunkDataMessage() { ChunkPos = pos.ToShared() });

            return false;
        }
    }
}
