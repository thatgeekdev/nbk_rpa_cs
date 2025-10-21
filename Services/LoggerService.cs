using System;
using System.IO;

namespace NBK_RPA_CS.Services
{
    public class LoggerService
    {
        private readonly string _logPath;

        public LoggerService(string logPath = null!)
        {
            _logPath = logPath ?? Path.Combine("Logs", "log.txt");
            var dir = Path.GetDirectoryName(_logPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public void Info(string message) => Log("INFO", message);
        public void Warn(string message) => Log("WARN", message);
        public void Error(string message) => Log("ERROR", message);

        private void Log(string level, string message)
        {
            var text = $"[{level}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
            Console.WriteLine(text);
            if (!string.IsNullOrEmpty(_logPath))
            {
                File.AppendAllText(_logPath, text + "\n");
            }
        }
    }
}
