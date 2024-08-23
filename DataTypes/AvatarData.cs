using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [MessagePackObject]
    public class VRMPosData
    {
        [Key(0)]
        public PosRot Head { get; }
        [Key(1)]
        public PosRot Hips { get; }
        [Key(2)]
        public PosRot Spine { get; }
        [Key(3)]
        public PosRot LUpperArm { get; }
        [Key(4)]
        public PosRot RUpperArm { get; }
        [Key(5)]
        public PosRot LLowerArm { get; }
        [Key(6)]
        public PosRot RLowerArm { get; }
        [Key(7)]
        public PosRot LHand { get; }
        [Key(8)]
        public PosRot RHand { get; }
        [Key(9)]
        public PosRot LUpperLeg { get; }
        [Key(10)]
        public PosRot RUpperLeg { get; }
        [Key(11)]
        public PosRot LLowerLeg { get; }
        [Key(12)]
        public PosRot RLowerLeg { get; }
        [Key(13)]
        public PosRot LFoot { get; }
        [Key(14)]
        public PosRot RFoot { get; }

        [SerializationConstructor]
        public VRMPosData(PosRot head, PosRot hips, PosRot spine, PosRot lUpperArm, PosRot rUpperArm, PosRot lLowerArm, PosRot rLowerArm, PosRot lHand, PosRot rHand, PosRot lUpperLeg, PosRot rUpperLeg, PosRot lLowerLeg, PosRot rLowerLeg, PosRot lFoot, PosRot rFoot)
        {
            Head = head;
            Hips = hips;
            Spine = spine;
            LUpperArm = lUpperArm;
            RUpperArm = rUpperArm;
            LLowerArm = lLowerArm;
            RLowerArm = rLowerArm;
            LHand = lHand;
            RHand = rHand;
            LUpperLeg = lUpperLeg;
            RUpperLeg = rUpperLeg;
            LLowerLeg = lLowerLeg;
            RLowerLeg = rLowerLeg;
            LFoot = lFoot;
            RFoot = rFoot;
        }
    }

    [MessagePackObject]
    public class PlayerPositionData
    {
        [Key(0)]
        public PosRot Head { get; }
        [Key(1)]
        public PosRot LeftHand { get; }
        [Key(2)]
        public PosRot RightHand { get; }

        [SerializationConstructor]
        public PlayerPositionData(PosRot head, PosRot leftHand, PosRot rightHand)
        {
            Head = head;
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }
}