using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(HeightCalibrator), nameof(HeightCalibrator.ShowTitleMenu))]
    class HeightCalibrator_ShowTitleMenu
    {
        private static void Postfix()
        {
            Assets.StartButton.Initialize();
            GameObject settingsTabObject = GameObject.Find(
                "/Player/XR Origin/Camera Offset/LeftHand Controller/RealLeftHand/MenuWindowL/Windows/MainCanvas/SettingsTab");
            settingsTabObject.transform.Find("DayNightCycleButton").gameObject.SetActive(false);
            GameObject distanceSettingsObject = settingsTabObject.transform.Find("DistanceSettings").gameObject;
            distanceSettingsObject.transform.Find("Text_1").gameObject.SetActive(false);
            distanceSettingsObject.transform.Find("Value_1").gameObject.SetActive(false);
            distanceSettingsObject.transform.Find("UpButton_1").gameObject.SetActive(false);
            distanceSettingsObject.transform.Find("DownButton_1").gameObject.SetActive(false);

            GameObject titleMainCanvas = GameObject.Find(
                "/TitleSpace/TitleMenu/MainCanvas");
            titleMainCanvas.transform.Find("AvatarVisibilityButton").gameObject.SetActive(false);
            titleMainCanvas.transform.Find("AvatarScale").gameObject.SetActive(false);
        }
    }
}