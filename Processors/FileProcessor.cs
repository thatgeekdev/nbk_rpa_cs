using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using OfficeOpenXml;
using NBK_RPA_CS.Services;


namespace NBK_RPA_CS.Processors
{
    public class FileProcessor
    {
        private readonly LoggerService _logger;
        private static readonly string[] RequiredFields =
            { "Nome", "Email", "Contacto", "Estado Civil", "Salário", "Salário Líquido" };

        public FileProcessor(LoggerService logger)
        {
            _logger = logger;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public List<Record> ProcessFile(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            try
            {
                switch (ext)
                {
                    case ".csv":
                        return ParseCsv(path);
                    case ".xlsx":
                        return ParseExcel(path);
                    case ".json":
                        return ParseJson(path);
                    default:
                        _logger.Warn($"Formato não suportado: {ext}");
                        return new();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao processar {path}: {ex.Message}");
                return new();
            }
        }

        private List<Record> ParseCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            if (lines.Length < 2) return new();

            var headers = lines[0].Split(',').Select(h => h.Trim()).ToArray();
            var indexes = headers.Select((h, i) => new { h, i })
                                 .Where(x => RequiredFields.Contains(x.h, StringComparer.OrdinalIgnoreCase))
                                 .ToDictionary(x => x.h, x => x.i);

            var list = new List<Record>();
            foreach (var line in lines.Skip(1))
            {
                var cols = line.Split(',');
                list.Add(Record.FromArray(cols, indexes));
            }

            return list.Where(r => r != null && !string.IsNullOrWhiteSpace(r.Name)).ToList();
        }

        private List<Record> ParseExcel(string path)
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var sheet = package.Workbook.Worksheets.FirstOrDefault();
            if (sheet == null) return new();

            var headers = Enumerable.Range(1, sheet.Dimension.Columns)
                                    .Select(i => sheet.Cells[1, i].Text.Trim())
                                    .ToList();

            var indexes = headers.Select((h, i) => new { h, i })
                                 .Where(x => RequiredFields.Contains(x.h, StringComparer.OrdinalIgnoreCase))
                                 .ToDictionary(x => x.h, x => x.i + 1);

            var list = new List<Record>();
            for (int row = 2; row <= sheet.Dimension.Rows; row++)
                list.Add(Record.FromExcel(sheet, row, indexes));

            return list.Where(r => r != null && !string.IsNullOrWhiteSpace(r.Name)).ToList();
        }

        private List<Record> ParseJson(string path)
        {
            var json = File.ReadAllText(path);
            var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
            if (items == null) return new();

            return items.Select(i => Record.FromJson(i)).Where(r => r != null).ToList();
        }
    }
}
