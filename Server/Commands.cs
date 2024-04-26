using System.Text;
using System.Text.Json;

namespace Server
{
    public static class Commands
    {
        public static object Deserialize(byte[] bytes)
        {
            try
            {
                JsonElement json = JsonDocument.Parse(bytes).RootElement;
                switch (json.GetProperty("Name").GetString())
                {
                    case "Connect":
                        return JsonSerializer.Deserialize<Connect>(json)!;
                    case "Disconnect":
                        return JsonSerializer.Deserialize<Disconnect>(json)!;
                    default:
                        throw new Exception("Invalid command.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred during deserialization.", e);
            }
        }

        public static byte[] Serialize(object obj)
        {
            try
            {
                return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred during serialization.", e);
            }
        }
    }

    public class Connect
    {
        public string Name { get; set; } = "Connect";
        public string UserName { get; set; } = "";

        public Connect(string userName)
        {
            UserName = userName;
        }
    }

    public class Disconnect
    {
        public string Name { get; set; } = "Disconnect";
        // public string UserName { get; set; } = "";

        // public Disconnect(string userName)
        // {
        //     UserName = userName;
        // }
    }
}
