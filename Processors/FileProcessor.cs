using System.Collections.Generic;
using NBK_RPA_CS.Services;

namespace NBK_RPA_CS.Processors
{
    public class FileProcessor
    {
        private readonly LoggerService _logger;

        public FileProcessor(LoggerService logger)
        {
            _logger = logger;
        }

        public List<Record> ProcessFile(string filePath)
        {
            _logger.Info($"Processing file: {filePath}");

            // ✅ Aqui você pode implementar lógica real de leitura XLS/CSV/JSON
            // Por enquanto, simulamos um registro
            return new List<Record>
            {
                new Record { Name = "Demo", Value = "123" }
            };
        }
    }
}
