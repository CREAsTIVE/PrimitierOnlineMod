using Newtonsoft.Json;

namespace YuchiGames.POM.Shared
{
    public class ClientSettings
    {
        //FIXME: private set maybe?
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 54162;
        public string UserName { get; set; } = "AnonymousUser";
        public string MinimumLogLevel { get; set; } = "Information";

        public static ClientSettings LoadFromFileOrCreateDefault(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var defaultSettings = new ClientSettings();
                File.WriteAllText(filePath, JsonConvert.SerializeObject(defaultSettings, Formatting.Indented));
                return defaultSettings;
            }

            return JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText(filePath)) 
                ?? throw new ArgumentException("Wrong configuration file format");
        }
    }
}