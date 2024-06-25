using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace YuchiGames.POM.Client.Assets
{
    class ToggleOnline : MelonMod
    {
        private static bool s_isOnline = true;
        public static bool IsOnline
        {
            get
            {
                return s_isOnline;
            }
        }

        public override async void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                if (sceneName == "Main")
                {
                    bool isReadyMainCanvas = false;
                    GameObject? mainCanvas = null;
                    GameObject onlineToggle = new GameObject("OnlineToggle");

                    onlineToggle.AddComponent<RectTransform>();
                    onlineToggle.AddComponent<Toggle>();

                    while (!isReadyMainCanvas)
                    {
                        await Task.Delay(100);
                        mainCanvas = GameObject.Find("TitleSpace/TitleMenu/MainCanvas");
                        if (mainCanvas is null)
                            continue;
                        isReadyMainCanvas = true;
                        LoggerInstance.Msg(mainCanvas.name);
                    }
                    if (mainCanvas is null)
                        throw new Exception("MainCanvas not found.");
                    onlineToggle.transform.SetParent(mainCanvas.transform);
                }
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e);
            }
        }
    }
}