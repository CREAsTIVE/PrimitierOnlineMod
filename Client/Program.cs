using MelonLoader;
using Microsoft.Extensions.Configuration;
using UnityEngine;
using YuchiGames.POM.Client.Data.Methods;
using YuchiGames.POM.Client.Data.Settings;

namespace YuchiGames.POM.Client
{
    public class Program : MelonMod
    {
        public static ClientSettings? settings;

        public override void OnInitializeMelon()
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build();
                settings = config.Get<ClientSettings>();

                GameObject HeadObject = GameObject.Find("Ybot_head");
                GameObject LeftHand = GameObject.Find("LeftHand Controller").transform.Find("IkTarget/ProxyHand/HandPosition/R_Hand_Mesh").gameObject;
                GameObject RightHand = GameObject.Find("RightHand Controller").transform.Find("IkTarget/ProxyHand/HandPosition/R_Hand_Mesh").gameObject;

                ConnectMethod connectMethod = new ConnectMethod("0.0.0");

            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
        }
    }
}
