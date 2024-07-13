using UnityEngine;

namespace YuchiGames.POM.Client.Assets
{
    public static class InfoUI
    {
        private static bool s_show = false;
        public static bool Show
        {
            get => s_show;
            set => s_show = value;
        }

        public static void Ping()
        {
            if (!s_show)
                return;
            GUI.Label(new Rect(1860, 0, 60, 30), $"<color=black><size=15>Ping: {NetworkManager.Ping}</size></color>");
        }
    }
}