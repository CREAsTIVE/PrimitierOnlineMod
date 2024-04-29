namespace Server.Data
{
    public class Plyaer
    {
        public PosRot LeftHand { get; set; }
        public PosRot RightHand { get; set; }

        public Plyaer(PosRot leftHand, PosRot rightHand)
        {
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }

    public class Object
    {
        public string UUID { get; set; }
        public PosRot PosRot { get; set; }

        public Object(string uuid, PosRot posRot)
        {
            UUID = uuid;
            PosRot = posRot;
        }
    }

    public class PosRot
    {
        public float[] Pos { get; set; }
        public float[] Rot { get; set; }

        public PosRot(float[] pos, float[] rot)
        {
            Pos = pos;
            Rot = rot;
        }
    }
}
