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
            using (TcpClient client = new TcpClient())
            {
                client.Connect(endPoint);

                IMessage connect = new ConnectMessage("0.0.0", "YuchiGames");

                byte[] data = new byte[1024];
                data = MessagePackSerializer.Serialize(connect);

                Console.WriteLine($"Client send buffer size: {data.Length}");
                Console.WriteLine($"Client send buffer: {BitConverter.ToString(data)}");

                using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(data, 0, data.Length);
                    int buffer2Length = 1024;
                    byte[] buffer2 = new byte[buffer2Length];
                    // int readLengthBytes = 0;

                    // while (readLengthBytes < buffer2Length)
                    // {
                    //     readLengthBytes += stream.Read(buffer2, readLengthBytes, buffer2Length - readLengthBytes);
                    // }
                    stream.Read(buffer2, 0, buffer2Length);

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

            using (UdpClient client = new UdpClient())
            {
                IMessage message = new SuccessMessage();

                byte[] bytes = MessagePackSerializer.Serialize(message);

                for (int i = 0; i < 50; i++)
                {
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