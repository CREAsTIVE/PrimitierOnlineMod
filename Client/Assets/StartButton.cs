using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace YuchiGames.POM.Client.Assets
{
    public class StartButton : MonoBehaviour
    {
        public static void Initialize()
        {
            GameObject startButton = GameObject.Find("StartButton");
            Destroy(startButton.GetComponent<ObjectActivateButton>());
            Button button = startButton.GetComponent<Button>();
            button.onClick.AddListener((UnityAction)OnClick);
        }

        public static void OnClick()
        {
            TextMeshPro infoText = GameObject.Find("InfoText").GetComponent<TextMeshPro>();

            Il2CppReferenceArray<GameObject> destroyObjects = new Il2CppReferenceArray<GameObject>(4);
            destroyObjects[0] = GameObject.Find("/TitleSpace/InfoText");
            destroyObjects[1] = GameObject.Find("/TitleSpace/VRtone/BoardGrip");
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            int malletCount = 2;
            foreach (GameObject gameObject in allGameObjects)
            {
                if (gameObject.name == "Mallet")
                {
                    destroyObjects[malletCount] = gameObject;
                    malletCount++;
                }
            }

            Il2CppReferenceArray<GameObject> enableObjects = new Il2CppReferenceArray<GameObject>(4);
            GameObject saveLoadObject = GameObject.Find("/Player/XR Origin/Camera Offset/LeftHand Controller/RealLeftHand/MenuWindowL/Windows/MainCanvas/SystemTab");
            enableObjects[0] = saveLoadObject.transform.Find("SaveLoad/SaveButton").gameObject;
            enableObjects[1] = saveLoadObject.transform.Find("SaveLoad/LoadButton").gameObject;
            enableObjects[2] = saveLoadObject.transform.Find("DieButton").gameObject;
            enableObjects[3] = saveLoadObject.transform.Find("BlueprintButton").gameObject;

            LoadingSequence loadingSequence = GameObject.FindObjectOfType<LoadingSequence>();
            loadingSequence.StartLoading(1, infoText, destroyObjects, enableObjects);
        }
    }
}