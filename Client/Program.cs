using MelonLoader;

namespace YuchiGames.POM.Client
{
    public class Program : MelonMod
    {
        public override void OnInitializeMelon()
        {
            try
            {

            }
            catch (Exception e)
            {
                MelonLogger.Error(e.Message);
            }
        }
    }
}
