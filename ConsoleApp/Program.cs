using System.Net;
using System.Net.Sockets;
using MessagePack;

[MessagePackObject]
public class Connect
{
    [Key(0)]
    public string Name { get; set; } = "ConnectMethod";
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
public class SuccessMethod
{
    [Key(0)]
    public string Name { get; } = "SuccessMethod";
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
            // TCP通信で接続する
            using (TcpClient client = new TcpClient())
            {
                client.Connect(endPoint);

                // Connectクラスのインスタンスを作成
                SuccessMethod connect = new SuccessMethod();

                // Connectクラスのインスタンスをバイナリに変換
                byte[] bytes = MessagePackSerializer.Serialize(connect);

                using (NetworkStream stream = client.GetStream())
                {
                    // バイナリデータを送信
                    stream.Write(bytes, 0, bytes.Length);
                }

                client.Close();
            }

            Thread.Sleep(1000);

            // UDP通信で接続する
            using (UdpClient client = new UdpClient())
            {
                // Connectクラスのインスタンスを作成
                SuccessMethod connect = new SuccessMethod();

                // Connectクラスのインスタンスをバイナリに変換
                byte[] bytes = MessagePackSerializer.Serialize(connect);

                for (int i = 0; i < 50; i++)
                {
                    // バイナリデータを送信
                    client.Send(bytes, bytes.Length, endPoint);
                }

                client.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}