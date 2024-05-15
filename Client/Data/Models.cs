using MessagePack;

namespace YuchiGames.POM.Client.Data.Models
{
    [MessagePackObject]
    public class PlayerModel
    {
        [Key(0)]
        public string Name { get; set;} = "PlayerModel";
        [Key(1)]
        public PosRot Hips { get; set; }
        [Key(2)]
        public PosRot Hips_Left { get; set; }
        [Key(3)]
        public PosRot Hips_Left_UpperLeg { get; set; }
        [Key(4)]
        public PosRot Hips_Left_UpperLeg_LowerLeg { get; set; }
        [Key(5)]
        public PosRot Hips_Left_UpperLeg_LowerLeg_Foot { get; set; }
        [Key(6)]
        public PosRot Hips_Right { get; set; }
        [Key(7)]
        public PosRot Hips_Right_UpperLeg { get; set; }
        [Key(8)]
        public PosRot Hips_Right_UpperLeg_LowerLeg { get; set; }
        [Key(9)]
        public PosRot Hips_Right_UpperLeg_LowerLeg_Foot { get; set; }
        [Key(10)]
        public PosRot Spine { get; set; }
        [Key(11)]
        public PosRot Chest { get; set; }
        [Key(12)]
        public PosRot Chest_Left { get; set; }
        [Key(13)]
        public PosRot Chest_Left_UpperArm { get; set; }
        [Key(14)]
        public PosRot Chest_Left_UpperArm_LowerArm { get; set; }
        [Key(15)]
        public PosRot Chest_Left_UpperArm_LowerArm_Hand { get; set; }
        [Key(16)]
        public PosRot Chest_Right { get; set; }
        [Key(17)]
        public PosRot Chest_Right_UpperArm { get; set; }
        [Key(18)]
        public PosRot Chest_Right_UpperArm_LowerArm { get; set; }
        [Key(19)]
        public PosRot Chest_Right_UpperArm_LowerArm_Hand { get; set; }
        [Key(20)]
        public PosRot Neck { get; set; }
        [Key(21)]
        public PosRot Head { get; set; }
        [Key(22)]
        public PosRot Head_left { get; set; }
        [Key(23)]
        public PosRot Head_right { get; set; }

        [SerializationConstructor]
        public PlayerModel(
            PosRot hips,
            PosRot hips_Left,
            PosRot hips_Left_UpperLeg,
            PosRot hips_Left_UpperLeg_LowerLeg,
            PosRot hips_Left_UpperLeg_LowerLeg_Foot,
            PosRot hips_Right,
            PosRot hips_Right_UpperLeg,
            PosRot hips_Right_UpperLeg_LowerLeg,
            PosRot hips_Right_UpperLeg_LowerLeg_Foot,
            PosRot spine,
            PosRot chest,
            PosRot chest_Left,
            PosRot chest_Left_UpperArm,
            PosRot chest_Left_UpperArm_LowerArm,
            PosRot chest_Left_UpperArm_LowerArm_Hand,
            PosRot chest_Right,
            PosRot chest_Right_UpperArm,
            PosRot chest_Right_UpperArm_LowerArm,
            PosRot chest_Right_UpperArm_LowerArm_Hand,
            PosRot neck,
            PosRot head,
            PosRot head_left,
            PosRot head_right
        )
        {
            Hips = hips;
            Hips_Left = hips_Left;
            Hips_Left_UpperLeg = hips_Left_UpperLeg;
            Hips_Left_UpperLeg_LowerLeg = hips_Left_UpperLeg_LowerLeg;
            Hips_Left_UpperLeg_LowerLeg_Foot = hips_Left_UpperLeg_LowerLeg_Foot;
            Hips_Right = hips_Right;
            Hips_Right_UpperLeg = hips_Right_UpperLeg;
            Hips_Right_UpperLeg_LowerLeg = hips_Right_UpperLeg_LowerLeg;
            Hips_Right_UpperLeg_LowerLeg_Foot = hips_Right_UpperLeg_LowerLeg_Foot;
            Spine = spine;
            Chest = chest;
            Chest_Left = chest_Left;
            Chest_Left_UpperArm = chest_Left_UpperArm;
            Chest_Left_UpperArm_LowerArm = chest_Left_UpperArm_LowerArm;
            Chest_Left_UpperArm_LowerArm_Hand = chest_Left_UpperArm_LowerArm_Hand;
            Chest_Right = chest_Right;
            Chest_Right_UpperArm = chest_Right_UpperArm;
            Chest_Right_UpperArm_LowerArm = chest_Right_UpperArm_LowerArm;
            Chest_Right_UpperArm_LowerArm_Hand = chest_Right_UpperArm_LowerArm_Hand;
            Neck = neck;
            Head = head;
            Head_left = head_left;
            Head_right = head_right;
        }
    }

    [MessagePackObject]
    public class ObjectModel
    {
        [Key(0)]
        public string Name { get; set; } = "ObjectModel";
        [Key(1)]
        public string UUID { get; set; }
        [Key(2)]
        public PosRot Position { get; set; }

        [SerializationConstructor]
        public ObjectModel(string uuid, PosRot position)
        {
            UUID = uuid;
            Position = position;
        }
    }

    [MessagePackObject]
    public class PosRot
    {
        [Key(0)]
        public float[] Pos { get; set; }
        [Key(1)]
        public float[] Rot { get; set; }

        [SerializationConstructor]
        public PosRot(float[] pos, float[] rot)
        {
            Pos = pos;
            Rot = rot;
        }
    }
}
