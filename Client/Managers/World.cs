namespace YuchiGames.POM.Client.Managers
{
    public static class World
    {
        private static int s_seed = 0;
        public static int Seed
        {
            get => s_seed;
            set => s_seed = value;
        }
    }
}