using Il2Cpp;
using UnityEngine;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client.Managers
{
    public static class Player
    {
        private static GameObject s_basePlayerObjects;
        private static GameObject[] s_playerObjects;
        private static Transform[,] s_playerObjectTransforms;

        static Player()
        {
            s_playerObjects = new GameObject[Network.ServerInfo.MaxPlayers];
            s_playerObjectTransforms = new Transform[Network.ServerInfo.MaxPlayers, 3];

            s_basePlayerObjects = new GameObject("BaseBody");
            s_basePlayerObjects.active = false;
            s_basePlayerObjects.transform.position = Vector3.zero;

            AvatarVisibility avatarVisibility = GameObject.Find("/System").GetComponent<AvatarVisibility>();

            GameObject s_headObject = GameObject.Instantiate(avatarVisibility.proxyHead);
            s_headObject.name = "Head";
            s_headObject.transform.parent = s_basePlayerObjects.transform;

            GameObject s_leftHandObject = GameObject.Instantiate(avatarVisibility.proxyLeftHand);
            GameObject.Destroy(s_leftHandObject.GetComponent<ProxyFingerController>());
            s_leftHandObject.name = "LeftHand";
            s_leftHandObject.transform.parent = s_basePlayerObjects.transform;

            GameObject s_rightHandObject = GameObject.Instantiate(avatarVisibility.proxyRightHand);
            GameObject.Destroy(s_rightHandObject.GetComponent<ProxyFingerController>());
            s_rightHandObject.name = "RightHand";
            s_rightHandObject.transform.parent = s_basePlayerObjects.transform;

            s_playerObjectTransforms[Network.ID, 0] = avatarVisibility.proxyHead.transform;
            s_playerObjectTransforms[Network.ID, 1] = avatarVisibility.proxyLeftHand.transform;
            s_playerObjectTransforms[Network.ID, 2] = avatarVisibility.proxyRightHand.transform;
        }

        public static void SpawnPlayer(int id)
        {
            s_playerObjects[id] = GameObject.Instantiate(s_basePlayerObjects);
            s_playerObjects[id].name = $"BaseBody_Player{id}";
            s_playerObjects[id].active = true;
            s_playerObjectTransforms[id, 0] = s_playerObjects[id].transform.Find("Head");
            s_playerObjectTransforms[id, 1] = s_playerObjects[id].transform.Find("LeftHand");
            s_playerObjectTransforms[id, 2] = s_playerObjects[id].transform.Find("RightHand");
        }

        public static void DespawnPlayer(int id)
        {
            GameObject.Destroy(s_playerObjects[id]);
            s_playerObjectTransforms[id, 0] = new Transform();
            s_playerObjectTransforms[id, 1] = new Transform();
            s_playerObjectTransforms[id, 2] = new Transform();
        }

        public static PlayerPositionData GetPlayerPosition()
        {
            PlayerPositionData posData = new PlayerPositionData(
                DataConverter.ToPosRot(s_playerObjectTransforms[Network.ID, 0]),
                DataConverter.ToPosRot(s_playerObjectTransforms[Network.ID, 1]),
                DataConverter.ToPosRot(s_playerObjectTransforms[Network.ID, 2]));
            return posData;
        }

        public static void SetPlayerPosition(int id, PlayerPositionData posData)
        {
            if (s_playerObjects[id] == null)
                return;
            s_playerObjectTransforms[id, 0].position = DataConverter.ToVector3(posData.Head.Position);
            s_playerObjectTransforms[id, 0].rotation = DataConverter.ToQuaternion(posData.Head.Rotation);
            s_playerObjectTransforms[id, 1].position = DataConverter.ToVector3(posData.LeftHand.Position);
            s_playerObjectTransforms[id, 1].rotation = DataConverter.ToQuaternion(posData.LeftHand.Rotation);
            s_playerObjectTransforms[id, 2].position = DataConverter.ToVector3(posData.RightHand.Position);
            s_playerObjectTransforms[id, 2].rotation = DataConverter.ToQuaternion(posData.RightHand.Rotation);
        }
    }
}