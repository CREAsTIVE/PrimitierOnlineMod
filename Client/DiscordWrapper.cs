namespace YuchiGames.POM.Client.DiscordWrapper
{
    public static class DiscordSetup
    {
        public static Discord.Discord? discord;

        public static void Initialize()
        {
            discord = new Discord.Discord(1239248231737856060, (ulong)Discord.CreateFlags.Default);
        }
    }

    public static class DiscordActivity
    {
        static Discord.ActivityManager? activityManager;
        public static void Initialize()
        {
            activityManager = DiscordSetup.discord!.GetActivityManager();
        }

        public static void UpdateActivity(string state, string details)
        {
            Discord.Activity activity = new Discord.Activity
            {
                State = state,
                Details = details,
                Type = Discord.ActivityType.Playing
            };

            activityManager!.UpdateActivity(activity, (result) =>
            {
                if (result == Discord.Result.Ok)
                {
                    Console.WriteLine("Activity updated.");
                }
                else
                {
                    Console.WriteLine("Failed to update activity.");
                }
            });
        }

        public static void ClearActivity()
        {
            if (activityManager == null)
            {
                Console.WriteLine("Activity manager is null.");
                return;
            }

            activityManager.ClearActivity((result) =>
            {
                if (result == Discord.Result.Ok)
                {
                    Console.WriteLine("Activity cleared.");
                }
                else
                {
                    Console.WriteLine("Failed to clear activity.");
                }
            });
        }
    }
}
