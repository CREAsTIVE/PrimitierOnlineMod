using MessagePack;
using YuchiGames.POM.Server.Data.Methods;

namespace YuchiGames.POM.Server.Data.Serialization
{
    public static class MethodsSerializer
    {
        public static object Deserialize(byte[] bytes)
        {
            try
            {
                switch (MessagePackSerializer.Deserialize<MethodsName>(bytes).Name)
                {
                    case "Connect":
                        return MessagePackSerializer.Deserialize<Connect>(bytes);
                    case "Disconnect":
                        return MessagePackSerializer.Deserialize<Disconnect>(bytes);
                    case "Error":
                        return MessagePackSerializer.Deserialize<Error>(bytes);
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
                return MessagePackSerializer.Serialize(obj);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred during serialization.", e);
            }
        }
    }
}
