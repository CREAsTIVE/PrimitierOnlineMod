namespace YuchiGames.POM.DataTypes
{
    public struct PosRot
    {
        public float[] Pos { get; }
        public float[] Rot { get; }

        public PosRot(float[] pos, float[] rot)
        {
            if (pos.Length != 3 || rot.Length != 3)
                throw new ArgumentException("Invalid array length");
            Pos = pos;
            Rot = rot;
        }
    }

    public struct BaseBody
    {
        public PosRot Head { get; }
        public PosRot LeftHand { get; }
        public PosRot RightHand { get; }

        public BaseBody(PosRot head, PosRot leftHand, PosRot rightHand)
        {
            Head = head;
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }

    public struct VRMBody
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

        public VRMBody(PosRot head, PosRot hips, PosRot spine, PosRot lUpperArm, PosRot rUpperArm, PosRot lLowerArm, PosRot rLowerArm, PosRot lHand, PosRot rHand, PosRot lUpperLeg, PosRot rUpperLeg, PosRot lLowerLeg, PosRot rLowerLeg, PosRot lFoot, PosRot rFoot)
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
}
