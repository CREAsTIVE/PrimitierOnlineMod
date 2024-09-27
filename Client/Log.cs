using MelonLoader;

namespace YuchiGames.POM.Client
{
    public static class Log
    {
        private static int s_logLevel = 1;

        static Log()
        {
            switch (Program.Settings.MinimumLogLevel)
            {
                case "Debug":
                    s_logLevel = 0;
                    break;
                case "Information":
                    s_logLevel = 1;
                    break;
                case "Warning":
                    s_logLevel = 2;
                    break;
                case "Error":
                    s_logLevel = 3;
                    break;
                case "BigError":
                    s_logLevel = 4;
                    break;
            }
        }

        public static void Debug(string message)
        {
            if (s_logLevel <= 0)
                Melon<Program>.Logger.Msg(message);
        }
        public static void Debug(object obj)
        {
            if (s_logLevel <= 0)
                Melon<Program>.Logger.Msg(obj);
        }

        public static void Information(string message)
        {
            if (s_logLevel <= 1)
                Melon<Program>.Logger.Msg(message);
        }
        public static void Information(object obj)
        {
            if (s_logLevel <= 1)
                Melon<Program>.Logger.Msg(obj);
        }

        public static void Warning(string message)
        {
            if (s_logLevel <= 2)
                Melon<Program>.Logger.Warning(message);
        }
        public static void Warning(object obj)
        {
            if (s_logLevel <= 2)
                Melon<Program>.Logger.Warning(obj);
        }

        public static void Error(string message)
        {
            if (s_logLevel <= 3)
                Melon<Program>.Logger.Error(message);
        }
        public static void Error(Exception exception)
        {
            if (s_logLevel <= 3)
                Melon<Program>.Logger.Error(exception.Message);
        }
        public static void Error(object obj)
        {
            if (s_logLevel <= 3)
                Melon<Program>.Logger.Error(obj);
        }

        public static void BigError(string message)
        {
            if (s_logLevel <= 4)
                Melon<Program>.Logger.BigError(message);
        }
        public static void BigError(Exception exception)
        {
            if (s_logLevel <= 4)
                Melon<Program>.Logger.BigError(exception.Message);
        }
    }
}