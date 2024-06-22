using MessagePack;

namespace YuchiGames.POM.DataTypes
{
    [Union(0, typeof(SendPlayerPosMessage))]
    public interface IUdpMessage { }

    [MessagePackObject]
    public struct SendPlayerPosMessage : IUdpMessage
    {
        [Key(0)]
        public int PlayerId { get; }
        [Key(1)]
        public bool IsVRMBody { get; }
        [Key(2)]
        public BaseBody BaseBody { get; }
        [Key(3)]
        public VRMBody VrmBody { get; }

        public SendPlayerPosMessage(int playerId, bool isVRMBody, BaseBody baseBody, VRMBody vrmBody)
        {
            PlayerId = playerId;
            IsVRMBody = isVRMBody;
            BaseBody = baseBody;
            VrmBody = vrmBody;
        }
    }
}