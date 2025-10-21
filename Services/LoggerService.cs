using System;
using System.IO;

namespace NBK_RPA_CS.Services
{
    public class LoggerService
    {
        private readonly string _logDir;
        private readonly string _logFile;

        public LoggerService(string logDir = "Logs")
        {
            // Cria a pasta Logs (se não existir)
            _logDir = logDir;
            Directory.CreateDirectory(_logDir);

            // Cria o ficheiro único com timestamp
            _logFile = Path.Combine(_logDir, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        }

        public void Info(string message) => WriteLog("INFO", message);
        public void Warn(string message) => WriteLog("WARN", message);
        public void Error(string message) => WriteLog("ERROR", message);

        private void WriteLog(string level, string message)
        {
            var logMessage = $"[{level}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
            Console.WriteLine(logMessage);

            // Guarda no ficheiro também
            File.AppendAllText(_logFile, logMessage + Environment.NewLine);
        }
    }
}
