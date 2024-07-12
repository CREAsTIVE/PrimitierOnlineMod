using UnityEngine;

namespace YuchiGames.POM.Client.Assets
{
    public class PingUI
    {
        public static void DrawPing()
        {
            GUI.Label(new Rect(1860, 0, 60, 30), $"<color=black><size=15>Ping: {NetworkManager.Ping}</size></color>");
        }
    }
}