using MessagePack;

namespace YuchiGames.POM.Server.Data.Models
{
    [MessagePackObject]
    public class Plyaer
    {
        [Key(0)]
        public PosRot LeftHand { get; set; }
        [Key(1)]
        public PosRot RightHand { get; set; }

        public Plyaer(PosRot leftHand, PosRot rightHand)
        {
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }

    [MessagePackObject]
    public class Object
    {
        [Key(0)]
        public string UUID { get; set; }
        [Key(1)]
        public PosRot PosRot { get; set; }

        public Object(string uuid, PosRot posRot)
        {
            UUID = uuid;
            PosRot = posRot;
        }
    }

    [MessagePackObject]
    public class PosRot
    {
        [Key(0)]
        public float[] Pos { get; set; }
        [Key(1)]
        public float[] Rot { get; set; }

        public PosRot(float[] pos, float[] rot)
        {
            Pos = pos;
            Rot = rot;
        }
    }
}
