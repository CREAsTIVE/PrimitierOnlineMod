using UnityEngine;
using YuchiGames.POM.Client.Managers;

namespace YuchiGames.POM.Client.Assets
{
    public static class InfoUI
    {
        private static bool s_isShow = false;
        public static bool IsShow
        {
            get => s_isShow;
            set => s_isShow = value;
        }

        public static void ShowUI()
        {
            if (!s_isShow)
                return;
            if (Network.IsConnected)
            {
                GUI.Label(new Rect(1720, 0, 200, 20), $"<color=black>Ping: {Network.Ping}</color>");
            }
            else
            {
                GUI.Label(new Rect(1720, 0, 200, 20), $"<color=black>Ping: NOT CONNECT</color>");
            }
            GUI.Label(new Rect(1720, 20, 200, 20), $"<color=black>Seed: {World.Seed}</color>");
        }
    }
}