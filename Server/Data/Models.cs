using MessagePack;

namespace YuchiGames.POM.Server.Data.Models
{
    [Union(0, typeof(PlayerModel))]
    [Union(1, typeof(ObjectModel))]
    public interface IModel { }

    [MessagePackObject]
    public class PlayerModel : IModel
    {
        [Key(0)]
        public PosRot Head { get; set; }
        [Key(1)]
        public PosRot LeftHand { get; set; }
        [Key(2)]
        public PosRot RightHand { get; set; }

        [SerializationConstructor]
        public PlayerModel(PosRot head, PosRot leftHand, PosRot rightHand)
        {
            Head = head;
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }

    [MessagePackObject]
    public class ObjectModel : IModel
    {
        [Key(0)]
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
    public class PosRot : IModel
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
