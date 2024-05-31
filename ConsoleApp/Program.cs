using System.Net;
using System.Net.Sockets;
using MessagePack;

[Union(0, typeof(ConnectMessage))]
[Union(1, typeof(DisconnectMessage))]
[Union(2, typeof(SuccessConnectionMessage))]
[Union(3, typeof(SuccessMessage))]
[Union(4, typeof(FailureMessage))]
public interface IMessage { }

[MessagePackObject]
public class ConnectMessage : IMessage
{
    [Key(0)]
    public string Version { get; set; }
    [Key(1)]
    public string UserName { get; set; }

    [SerializationConstructor]
    public ConnectMessage(string version, string userName)
    {
        Version = version;
        UserName = userName;
    }
}

[MessagePackObject]
public class DisconnectMessage : IMessage { }

[MessagePackObject]
public class SuccessConnectionMessage : IMessage
{
    [Key(0)]
    public int YourID { get; set; }
    [Key(1)]
    public int[] IDList { get; set; }

    [SerializationConstructor]
    public SuccessConnectionMessage(int yourID, int[] idList)
    {
        YourID = yourID;
        IDList = idList;
    }
}

[MessagePackObject]
public class SuccessMessage : IMessage { }

[MessagePackObject]
public class FailureMessage : IMessage
{
    [Key(0)]
    public Exception ExceptionMessage { get; set; }

    [SerializationConstructor]
    public FailureMessage(Exception exception)
    {
        ExceptionMessage = exception;
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
                IMessage connect = new ConnectMessage("0.0.0", "YuchiGames");

                byte[] data = MessagePackSerializer.Serialize(connect);
                byte[] lengthBytes = new byte[4];
                lengthBytes = BitConverter.GetBytes(data.Length);
                byte[] buffer = new byte[lengthBytes.Length + data.Length];
                Buffer.BlockCopy(lengthBytes, 0, buffer, 0, lengthBytes.Length);
                Buffer.BlockCopy(data, 0, buffer, lengthBytes.Length, data.Length);

                Console.WriteLine($"Client send buffer size: {data.Length}");
                Console.WriteLine($"Client send buffer: {BitConverter.ToString(data)}");

                using (NetworkStream stream = client.GetStream())
                {
                    // バイナリデータを送信
                    stream.Write(buffer, 0, buffer.Length);
                    byte[] lengthBytes2 = new byte[4];
                    stream.Read(lengthBytes2, 0, lengthBytes2.Length);
                    int bufferLength = BitConverter.ToInt32(lengthBytes2, 0);
                    byte[] buffer2 = new byte[bufferLength];
                    int readLengthBytes = 0;

                    while (readLengthBytes < bufferLength)
                    {
                        readLengthBytes += stream.Read(buffer2, readLengthBytes, bufferLength - readLengthBytes);
                    }

                    Console.WriteLine($"Client receive buffer size: {buffer2.Length}");
                    Console.WriteLine($"Client receive buffer: {BitConverter.ToString(buffer2)}");
                    Console.WriteLine($"Client json: {MessagePackSerializer.ConvertToJson(buffer2)}");

                    switch (MessagePackSerializer.Deserialize<IMessage>(buffer2))
                    {
                        case SuccessConnectionMessage successConnectionMessage:
                            Console.WriteLine($"Your ID: {successConnectionMessage.YourID}, ID List: {string.Join(", ", successConnectionMessage.IDList)}");
                            break;
                        case FailureMessage failureMessage:
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
                IMessage message = new SuccessMessage();

                byte[] bytes = new byte[1024];
                // Connectクラスのインスタンスをバイナリに変換
                bytes = MessagePackSerializer.Serialize(message);

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