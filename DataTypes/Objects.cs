using MessagePack;

namespace YuchiGames.POM.Data
{
    [Union(0, typeof(BaseBody))]
    [Union(1, typeof(VRMBody))]
    [Union(2, typeof(PrimitiveObject))]
    public interface IObject { }

    [MessagePackObject]
    public struct BaseBody : IObject
    {
        [Key(0)]
        public PosRot LeftHand { get; set; }
        [Key(1)]
        public PosRot RightHand { get; set; }

        [SerializationConstructor]
        public BaseBody(PosRot leftHand, PosRot rightHand)
        {
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }

    [MessagePackObject]
    public struct VRMBody : IObject
    {
        [Key(0)]
        public PosRot Head { get; set; }
        [Key(1)]
        public PosRot LeftHand { get; set; }
        [Key(2)]
        public PosRot RightHand { get; set; }

        [SerializationConstructor]
        public VRMBody(PosRot head, PosRot leftHand, PosRot rightHand)
        {
            Head = head;
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }

    [MessagePackObject]
    public struct PrimitiveObject : IObject
    {
        [Key(0)]
        public string UUID { get; set; }
        [Key(1)]
        public PosRot Position { get; set; }

        [SerializationConstructor]
        public PrimitiveObject(string uuid, PosRot position)
        {
            UUID = uuid;
            Position = position;
        }
    }

    public struct PosRot
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
