using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace YuchiGames.POM.Client.Assets
{
    static class ToggleOnline
    {
        private static bool s_isOnline = true;
        public static bool IsOnline
        {
            get
            {
                return s_isOnline;
            }
        }

        public static void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                //if (sceneName == "Main")
                //{
                //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube.transform.position = new Vector3(0, 1.1f, 1.9f);

                //    bool isReadyMainCanvas = false;
                //    GameObject? mainCanvas = null;
                //    GameObject onlineToggle = new GameObject("OnlineToggle");

                //    GameObject.Instantiate(onlineToggle);
                //    onlineToggle.AddComponent<RectTransform>();
                //    onlineToggle.AddComponent<Toggle>();

                //    while (!isReadyMainCanvas)
                //    {
                //        await Task.Delay(100);
                //        mainCanvas = GameObject.Find("TitleSpace/TitleMenu/MainCanvas");
                //        if (mainCanvas is null)
                //            continue;
                //        isReadyMainCanvas = true;
                //        Melon<Program>.Logger.Msg(mainCanvas.name);
                //    }
                //    if (mainCanvas is null)
                //        throw new Exception("MainCanvas not found.");
                //    onlineToggle.transform.SetParent(mainCanvas.transform);
                //}
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e);
            }
        }
    }
}