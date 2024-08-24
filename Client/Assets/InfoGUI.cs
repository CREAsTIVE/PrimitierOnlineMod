using Il2Cpp;
using UnityEngine;
using YuchiGames.POM.Client.Managers;

namespace YuchiGames.POM.Client.Assets
{
    public class InfoGUI : MonoBehaviour
    {
        private static bool s_isShow = false;
        public static bool IsShow
        {
            get => s_isShow;
            set => s_isShow = value;
        }

        public static void OnGUI()
        {
            if (!s_isShow)
                return;
            GUI.Label(new Rect(0, 0, 1920, 20), $"<color=black>Seed: {TerrainGenerator.worldSeed}</color>");
            GUI.Label(new Rect(0, 20, 1920, 20), $"<color=black>Chunk: {CubeGenerator.PlayerChunkPos.x}, {CubeGenerator.PlayerChunkPos.y}</color>");
            if (Network.IsConnected)
            {
                GUI.Label(new Rect(0, 40, 1920, 20), $"<color=black>ServerStatus: CONNECTED</color>");
                GUI.Label(new Rect(0, 60, 1920, 20), $"<color=black>Client ID: {Network.ID}</color>");
                GUI.Label(new Rect(0, 80, 1920, 20), $"<color=black>Ping: {Network.Ping}</color>");
            }
            else
            {
                GUI.Label(new Rect(0, 40, 1920, 20), $"<color=black>ServerStatus: NOT CONNECT</color>");
            }
        }
    }
}