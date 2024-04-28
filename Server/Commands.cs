using System.Text;
using System.Text.Json;

namespace Server.Commands
{
    public static class Serializer
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
                    case "Error":
                        return JsonSerializer.Deserialize<Error>(json)!;
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
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Version { get; set; }

        public Connect(string userID, string userName, string version)
        {
            UserID = userID;
            UserName = userName;
            Version = version;
        }
    }

    public class Disconnect
    {
        public string Name { get; set; } = "Disconnect";
    }

    public class Error
    {
        public string Name { get; set; } = "Error";
        public Exception ExceptionMessage { get; set; }

        public Error(Exception exception)
        {
            ExceptionMessage = exception;
        }
    }
}
