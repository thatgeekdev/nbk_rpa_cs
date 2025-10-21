using System;
using System.IO;

namespace NBK_RPA_CS.Config
{
    public class ConfigService
    {
        public string StartUrl { get; private set; } = "https://rpa.xidondzo.com/";
        public string LogsPath { get; private set; } = Path.Combine("Logs", "log.txt");
        public string ExportsPath { get; private set; } = "Exports";
        public int DownloadWaitSeconds { get; private set; } = 20;

        public ConfigService()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogsPath) ?? "Logs");
            Directory.CreateDirectory(ExportsPath);
        }
    }
}
