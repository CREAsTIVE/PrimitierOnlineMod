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
                    case "ConnectMethod":
                        return MessagePackSerializer.Deserialize<ConnectMethod>(bytes);
                    case "DisconnectMethod":
                        return MessagePackSerializer.Deserialize<DisconnectMethod>(bytes);
                    case "SuccessMethod":
                        return MessagePackSerializer.Deserialize<SuccessMethod>(bytes);
                    case "FailureMethod":
                        return MessagePackSerializer.Deserialize<FailureMethod>(bytes);
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
