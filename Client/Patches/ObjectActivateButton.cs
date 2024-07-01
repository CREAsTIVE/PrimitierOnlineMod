using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace YuchiGames.POM.Client.Patches
{
    [HarmonyPatch(typeof(ObjectActivateButton), nameof(ObjectActivateButton.OnPress))]
    class ObjectActivateButton_OnPress
    {
        static bool Prefix()
        {
            Melon<Program>.Logger.Msg("ObjectActivateButton_OnPress.Prefix() called");

            try
            {
                var enabledObject = Traverse.Create<ObjectActivateButton>().Field("NativeFieldInfoPtr_enableObjects").GetValue();
                Melon<Program>.Logger.Msg($"enabledObject: {enabledObject.ToString()}");
                var disabledObject = Traverse.Create<ObjectActivateButton>().Field("NativeFieldInfoPtr_disableObjects").GetValue();
                Melon<Program>.Logger.Msg($"disabledObject: {disabledObject.ToString()}");
                var toggleObject = Traverse.Create<ObjectActivateButton>().Field("NativeFieldInfoPtr_toggleObjects").GetValue();
                Melon<Program>.Logger.Msg($"toggleObject: {toggleObject.ToString()}");

                return true;
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e);
            }

            return false;
        }

        static void Postfix()
        {
            Melon<Program>.Logger.Msg("ObjectActivateButton_OnPress.Postfix() called");

            try
            {
                //var get_toggleObjects = Traverse.Create<ObjectActivateButton>().Property("toggleObjects").GetValue<GameObject>();
                //Melon<Program>.Logger.Msg($"get_toggleObjects: {get_toggleObjects.ToString()}");
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e);
            }
        }
    }

    [HarmonyPatch(typeof(ObjectActivateButton), nameof(ObjectActivateButton.toggleObjects))]
    class ObjectActivateButton_toggleObjects
    {
        static void Postfix()
        {
            Melon<Program>.Logger.Msg("ObjectActivateButton_toggleObjects.Postfix() called");

            try
            {
                var get_toggleObjects = Traverse.Create<ObjectActivateButton>().Property("toggleObjects").GetValue<GameObject>();
                Melon<Program>.Logger.Msg($"get_toggleObjects: {get_toggleObjects.ToString()}");
            }
            catch (Exception e)
            {
                Melon<Program>.Logger.Error(e);
            }
        }
    }
}