using System.Collections.Generic;
using System.IO;

namespace NBK_RPA_CS.Services
{
    public class ExportService
    {
        private readonly string _exportsDir;

        public ExportService(string exportsDir)
        {
            _exportsDir = exportsDir;
            Directory.CreateDirectory(_exportsDir);
        }

        public string ExportNormalized(IEnumerable<Record> records)
        {
            var outPath = Path.Combine(_exportsDir, $"normalized_{System.DateTime.Now:yyyyMMddHHmmss}.csv");
            using var writer = new StreamWriter(outPath);
            writer.WriteLine("Name,Value");
            foreach (var r in records)
            {
                writer.WriteLine($"{r.Name},{r.Value}");
            }
            return outPath;
        }
    }
}
