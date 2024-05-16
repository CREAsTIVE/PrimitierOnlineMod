using MessagePack;

namespace YuchiGames.POM.Server.Data.Models
{
    [MessagePackObject]
    public class PlayerModel
    {
        [Key(0)]
        public string Name { get; set; } = "PlayerModel";
        [Key(1)]
        public PosRot Head { get; set; }
        [Key(2)]
        public PosRot LeftHand { get; set; }
        [Key(3)]
        public PosRot RightHand { get; set; }

        [SerializationConstructor]
        public PlayerModel(
            PosRot head,
            PosRot leftHand,
            PosRot rightHand
        )
        {
            Head = head;
            LeftHand = leftHand;
            RightHand = rightHand;
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
