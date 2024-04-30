using MelonLoader;

namespace Client
{
    public class Class1 : MelonMod
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
