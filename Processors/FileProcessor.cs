using System;
using System.Collections.Generic;
using System.IO;

namespace NBK_RPA_CS.Services
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

            var records = new List<Record>();

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                // Ignora linhas vazias ou cabeçalhos
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.Contains("Nome Completo"))
                    continue;

                // Supondo que os campos estão separados por tab ou múltiplos espaços
                var parts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 6)
                    continue;

                // Ajuste: pega os 6 campos que nos interessam
                var record = new Record
                {
                    Name = parts[1],
                    Email = parts[2],
                    Contact = parts[3],
                    MaritalStatus = parts[4],
                    Salary = parts[5]
                };

                records.Add(record);
            }

            _logger.Info($"Extracted {records.Count} records from {Path.GetFileName(filePath)}");
            return records;
        }
    }
}
