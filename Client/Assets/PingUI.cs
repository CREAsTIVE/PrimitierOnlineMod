using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using YuchiGames.POM.Client.Managers;

namespace YuchiGames.POM.Client.Assets
{
    public class PingUI : MonoBehaviour
    {
        private static TextMeshPro? s_pingTMP;

        public static void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            Transform parentTransform = GameObject.Find("/Player/XR Origin/Camera Offset/LeftHand Controller/RealLeftHand/MenuWindowL/Windows/MainCanvas").transform;
            GameObject fpsObject = parentTransform.Find("FpsText").gameObject;
            GameObject pingObject = Instantiate(fpsObject);
            pingObject.transform.SetParent(parentTransform);
            pingObject.name = "PingText";
            Transform pingTransform = pingObject.transform;
            pingTransform.localPosition = fpsObject.transform.localPosition + new Vector3(45, 0, 0);
            pingTransform.localRotation = fpsObject.transform.localRotation;
            pingTransform.localScale = fpsObject.transform.localScale;
            s_pingTMP = pingObject.GetComponent<TextMeshPro>();
            s_pingTMP.text = "Ping: -1";
            Destroy(pingObject.GetComponent<FpsText>());
        }

        public static void OnUpdate()
        {
            if (s_pingTMP is null)
                return;
            s_pingTMP.text = $"Ping: {Network.Ping}";
        }
    }
}