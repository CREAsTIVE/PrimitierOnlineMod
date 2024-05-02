using MelonLoader;

namespace Client
{
    public class Program : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Hello, world!");
        }

        public override void OnFixedUpdate()
        {

        }
    }
}
