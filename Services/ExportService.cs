using System;
using System.Collections.Generic;
using System.IO;

namespace NBK_RPA_CS.Services
{
    public class ExportService
    {
        private readonly string _exportDir;

        public ExportService(string exportDir)
        {
            _exportDir = exportDir;
            if (!Directory.Exists(_exportDir))
                Directory.CreateDirectory(_exportDir);
        }

        public string ExportToCsv(List<Record> records, string fileName = "output.csv")
        {
            var filePath = Path.Combine(_exportDir, fileName);

            using (var writer = new StreamWriter(filePath))
            {
                // Cabeçalho
                writer.WriteLine("Nome,Email,Contacto,Estado Civil,Salário Líquido");

                foreach (var record in records)
                {
                    // Escreve os valores separados por vírgula
                    writer.WriteLine($"{record.Name},{record.Email},{record.Contact},{record.MaritalStatus},{record.Salary}");
                }
            }

            return filePath;
        }
    }
}
