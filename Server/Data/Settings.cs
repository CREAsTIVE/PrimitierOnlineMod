using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuchiGames.POM.Server.Data.Settings
{
    class ServerSettings
    {
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public string version { get; set; } = "0.0.0";
        public int port { get; set; } = 54162;
        public int maxPlayer { get; set; } = 10;
    }
}
