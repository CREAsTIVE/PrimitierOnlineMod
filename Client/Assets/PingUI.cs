using UnityEngine;
using YuchiGames.POM.Client.Network;

namespace YuchiGames.POM.Client.Assets
{
    class PingUI
    {
        public static void DrawPing()
        {
            GUI.Label(new Rect(1860, 0, 60, 30), $"<color=black><size=15>Ping: {Sender.Ping}</size></color>");
        }
    }
}