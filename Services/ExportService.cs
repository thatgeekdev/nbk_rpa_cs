using System.Collections.Generic;
using System.IO;

namespace NBK_RPA_CS.Services
{
    public class ExportService
    {
        private readonly string _dir;

        public ExportService(string exportDir)
        {
            _dir = exportDir;
            Directory.CreateDirectory(_dir);
        }

        public string ExportToCsv(List<Record> records, string fileName)
        {
            var path = Path.Combine(_dir, fileName);
            using var writer = new StreamWriter(path);
            writer.WriteLine("Nome,Email,Contacto,Estado Civil,Salário Líquido, Período");

            foreach (var r in records)
                writer.WriteLine($"{r.Name},{r.Email},{r.Contact},{r.MaritalStatus},{r.Salary}");

            return path;
        }
    }
}
