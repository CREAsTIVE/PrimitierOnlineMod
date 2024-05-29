using System.Net;
using System.Net.Sockets;
using MessagePack;

[MessagePackObject]
public class ConnectMessage
{
    [Key(0)]
    public string Name { get; } = "ConnectMessage";
    [Key(1)]
    public string Version { get; set; }
    [Key(2)]
    public string UserName { get; set; }

    [SerializationConstructor]
    public ConnectMessage(string version, string userName)
    {
        Version = version;
        UserName = userName;
    }
}

[MessagePackObject]
public class SuccessConnectionMessage
{
    [Key(0)]
    public string Name { get; } = "SuccessConnectionMessage";
    [Key(1)]
    public int YourID { get; set; }
    [Key(2)]
    public int[] IDList { get; set; }

    [SerializationConstructor]
    public SuccessConnectionMessage(int yourID, int[] idList)
    {
        YourID = yourID;
        IDList = idList;
    }
}

[MessagePackObject]
public class Player
{
    [Key(0)]
    public PosRot LeftHand { get; set; }
    [Key(1)]
    public PosRot RightHand { get; set; }

    [SerializationConstructor]
    public Player(PosRot leftHand, PosRot rightHand)
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

[MessagePackObject]
public class MessagesName
{
    [Key(0)]
    public string Name { get; }

    [SerializationConstructor]
    public MessagesName(string name)
    {
        Name = name;
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
                ConnectMessage connect = new ConnectMessage("0.0.0", "YuchiGames");

                byte[] bytes = new byte[1024];
                // Connectクラスのインスタンスをバイナリに変換
                bytes = MessagePackSerializer.Serialize(connect);

                using (NetworkStream stream = client.GetStream())
                {
                    // バイナリデータを送信
                    stream.Write(bytes, 0, bytes.Length);
                    byte[] receiveBytes = new byte[1024];
                    stream.Read(receiveBytes, 0, bytes.Length);
                    switch (MessagePackSerializer.Deserialize<MessagesName>(receiveBytes).Name)
                    {
                        case "SuccessConnectionMessage":
                            SuccessConnectionMessage connectionMessage = MessagePackSerializer.Deserialize<SuccessConnectionMessage>(receiveBytes);
                            Console.WriteLine($"Your ID: {connectionMessage.YourID}, ID List: {string.Join(", ", connectionMessage.IDList)}");
                            break;
                        case "FailureMessage":
                            throw new Exception("Received failure message.");
                        default:
                            throw new Exception("Received unknown message.");
                    }
                }

                client.Close();
            }

            Thread.Sleep(1000);

            // UDP通信で接続する
            using (UdpClient client = new UdpClient())
            {
                Player player = new Player(new PosRot(new float[] { 1.0f, 2.0f, 3.0f }, new float[] { 0.0f, 0.0f, 0.0f }), new PosRot(new float[] { 4.0f, 5.0f, 6.0f }, new float[] { 0.0f, 0.0f, 0.0f }));

                byte[] bytes = new byte[1024];
                // Connectクラスのインスタンスをバイナリに変換
                bytes = MessagePackSerializer.Serialize(player);

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