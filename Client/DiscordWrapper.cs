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
                State = "In Play Mode",
                Details = "Playing the Trumpet!",
                Timestamps =
                {
                    Start = 5,
                },
                Assets =
                {
                    LargeImage = "foo largeImageKey", // Larger Image Asset Value
                    LargeText = "foo largeImageText", // Large Image Tooltip
                    SmallImage = "foo smallImageKey", // Small Image Asset Value
                    SmallText = "foo smallImageText", // Small Image Tooltip
                },
                Party =
                {
                    Id = "foo partyID",
                    Size = {
                        CurrentSize = 1,
                        MaxSize = 4,
                    },
                },
                Secrets =
                {
                    Match = "foo matchSecret",
                    Join = "foo joinSecret",
                    Spectate = "foo spectateSecret",
                },
                Instance = true,
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
