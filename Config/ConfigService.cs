using System;
using System.IO;

namespace NBK_RPA_CS.Config
{
    public class ConfigService
    {
        public string StartUrl { get; private set; } = "https://rpa.xidondzo.com/";
        public string LogsDir { get; private set; } = "Logs";
        public string ExportsPath { get; private set; } = "Exports";

        public ConfigService()
        {
            // Garante que as pastas existem
            Directory.CreateDirectory(LogsDir);
            Directory.CreateDirectory(ExportsPath);
        }
    }
}
