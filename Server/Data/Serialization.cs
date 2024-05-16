﻿using MessagePack;
using YuchiGames.POM.Server.Data.Messages;

namespace YuchiGames.POM.Server.Data.Serialization
{
    public static class MethodsSerializer
    {
        public static object Deserialize(byte[] bytes)
        {
            try
            {
                switch (MessagePackSerializer.Deserialize<MessagesName>(bytes).Name)
                {
                    case "ConnectMessage":
                        return MessagePackSerializer.Deserialize<ConnectMessage>(bytes);
                    case "DisconnectMessage":
                        return MessagePackSerializer.Deserialize<DisconnectMessage>(bytes);
                    case "SuccessMessage":
                        return MessagePackSerializer.Deserialize<SuccessMessage>(bytes);
                    case "FailureMessage":
                        return MessagePackSerializer.Deserialize<FailureMessage>(bytes);
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
