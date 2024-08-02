using UnityEngine;

namespace YuchiGames.POM.DataTypes
{
    public static class DataConverter
    {
        public static Vector3 ToVector3(Position position)
        {
            return new Vector3(position.X, position.Y, position.Z);
        }

        public static Vector3 ToVector3(PosRot posRot)
        {
            return new Vector3(posRot.Position.X, posRot.Position.Y, posRot.Position.Z);
        }

        public static Quaternion ToQuaternion(Rotation rotation)
        {
            return new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
        }

        public static Quaternion ToQuaternion(PosRot posRot)
        {
            return new Quaternion(posRot.Rotation.X, posRot.Rotation.Y, posRot.Rotation.Z, posRot.Rotation.W);
        }

        public static Transform ToTransform(PosRot posRot)
        {
            Transform transform = new Transform();
            transform.position = ToVector3(posRot);
            transform.rotation = ToQuaternion(posRot);
            return transform;
        }

        public static Position ToPosition(Vector3 vector3)
        {
            return new Position(vector3.x, vector3.y, vector3.z);
        }

        public static Position ToPosition(Transform transform)
        {
            return new Position(transform.position.x, transform.position.y, transform.position.z);
        }

        public static Rotation ToRotation(Quaternion quaternion)
        {
            return new Rotation(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static Rotation ToRotation(Transform transform)
        {
            return new Rotation(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        }

        public static PosRot ToPosRot(Transform transform)
        {
            return new PosRot(new Position(transform.position.x, transform.position.y, transform.position.z),
                new Rotation(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w));
        }
    }
}