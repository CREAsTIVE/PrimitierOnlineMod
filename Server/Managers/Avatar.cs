using Serilog;

namespace YuchiGames.POM.Server.Managers
{
    public static class Avatar
    {
        private static string[] s_vrmFiles;

        static Avatar()
        {
            s_vrmFiles = new string[Program.Settings.MaxPlayers];
        }

        public static void UploadVRM(int id, byte[] data)
        {
            if (File.Exists(s_vrmFiles[id]))
                File.Delete(s_vrmFiles[id]);
            s_vrmFiles[id] = Path.GetTempFileName();
            File.WriteAllBytes(s_vrmFiles[id], data);
            Log.Debug($"Save temp file: {s_vrmFiles[id]}");
        }

        public static byte[][] GetAllVRMFiles()
        {
            byte[][] vrmFiles = new byte[Program.Settings.MaxPlayers][];
            for (int i = 0; i < Program.Settings.MaxPlayers; i++)
            {
                if (File.Exists(s_vrmFiles[i]))
                    vrmFiles[i] = File.ReadAllBytes(s_vrmFiles[i]);
            }
            return vrmFiles;
        }
    }
}