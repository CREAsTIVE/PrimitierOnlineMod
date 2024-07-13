using UnityEngine;

namespace YuchiGames.POM.Client.Assets
{
    public static class InfoUI
    {
        public static void Ping()
        {
            GUI.Label(new Rect(1860, 0, 60, 30), $"<color=black><size=15>Ping: {NetworkManager.Ping}</size></color>");
        }
    }
}