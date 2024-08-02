namespace YuchiGames.POM.DataTypes
{
    public struct VRMPosData
    {
        public PosRot Head { get; }
        public PosRot Hips { get; }
        public PosRot Spine { get; }
        public PosRot LUpperArm { get; }
        public PosRot RUpperArm { get; }
        public PosRot LLowerArm { get; }
        public PosRot RLowerArm { get; }
        public PosRot LHand { get; }
        public PosRot RHand { get; }
        public PosRot LUpperLeg { get; }
        public PosRot RUpperLeg { get; }
        public PosRot LLowerLeg { get; }
        public PosRot RLowerLeg { get; }
        public PosRot LFoot { get; }
        public PosRot RFoot { get; }

        public VRMPosData(PosRot[] posRots)
        {
            Head = posRots[0];
            Hips = posRots[1];
            Spine = posRots[2];
            LUpperArm = posRots[3];
            RUpperArm = posRots[4];
            LLowerArm = posRots[5];
            RLowerArm = posRots[6];
            LHand = posRots[7];
            RHand = posRots[8];
            LUpperLeg = posRots[9];
            RUpperLeg = posRots[10];
            LLowerLeg = posRots[11];
            RLowerLeg = posRots[12];
            LFoot = posRots[13];
            RFoot = posRots[14];
        }
    }

    public struct BaseBodyData
    {
        public PosRot Head { get; }
        public PosRot LeftHand { get; }
        public PosRot RightHand { get; }

        public BaseBodyData(PosRot head, PosRot leftHand, PosRot rightHand)
        {
            Head = head;
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }
}