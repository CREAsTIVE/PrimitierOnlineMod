using MessagePack;
using YuchiGames.POM.Client.Data.Methods;

namespace YuchiGames.POM.Client.Data.Serialization
{
    public static class CommandsSerializer
    {
        public static object Deserialize(byte[] bytes)
        {
            try
            {
                switch (MessagePackSerializer.Deserialize<MethodsName>(bytes).Name)
                {
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
