using UnityEngine;
using YuchiGames.POM.DataTypes;

namespace YuchiGames.POM.Client
{
    public static class Utils
    {
        public static PosRot ConvertToPosRot(Transform transform)
        {
            PosRot posRot = new PosRot(
                new float[3] { transform.position.x, transform.position.y, transform.position.z },
                new float[3] { transform.rotation.x, transform.rotation.y, transform.rotation.z }
                );
            return posRot;
        }
    }
}