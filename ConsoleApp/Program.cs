using System.Net;
using System.Net.Sockets;
using MessagePack;

[MessagePackObject]
public class Connect
{
    [Key(0)]
    public string Name { get; set; } = "Connect";
    [Key(1)]
    public string UserID { get; set; }
    [Key(2)]
    public string UserName { get; set; }
    [Key(3)]
    public string Version { get; set; }

    [SerializationConstructor]
    public Connect(string userID, string userName, string version)
    {
        UserID = userID;
        UserName = userName;
        Version = version;
    }
}

[MessagePackObject]
public class Plyaer
{
    [Key(0)]
    public PosRot LeftHand { get; set; }
    [Key(1)]
    public PosRot RightHand { get; set; }

    [SerializationConstructor]
    public Plyaer(PosRot leftHand, PosRot rightHand)
    {
        LeftHand = leftHand;
        RightHand = rightHand;
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

class Program
{
    static void Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 54162);

        try
        {
            using (TcpClient client = new TcpClient(endPoint))
            {
                NetworkStream stream = client.GetStream();

                Connect connect = new Connect("123", "Yuchi", "1.0.0");
                stream.Write(MessagePackSerializer.Serialize(connect));
            }

            //using (UdpClient client = new UdpClient())
            //{
            //    client.Connect(endPoint);

            //    while (true)
            //    {
            //        client.Send(new byte[] { 0 });
            //    }
            //}
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}