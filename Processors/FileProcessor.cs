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
            { "Nome", "Email", "Contacto", "Estado Civil", "Salário", "Salário Líquido", "Período"};

        public FileProcessor(LoggerService logger)
        {
            _logger = logger;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public List<Record> ProcessFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.Warn("O caminho do arquivo está vazio ou nulo.");
                return new();
            }

            string ext = Path.GetExtension(path)?.ToLower() ?? string.Empty;
            try
            {
                return ext switch
                {
                    ".txt" => ParseTxt(path),
                    ".csv" => ParseCsv(path),
                    ".xlsx" => ParseExcel(path),
                    ".json" => ParseJson(path),
                    _ => LogAndReturnEmpty($"Formato não suportado: {ext}")
                };
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao processar {path}: {ex.Message}");
                return new();
            }
        }

        private List<Record> ParseTxt(string path)
        {
            if (!File.Exists(path))
            {
                _logger.Warn($"Arquivo TXT não encontrado: {path}");
                return new();
            }

            var lines = File.ReadAllLines(path);
            var list = new List<Record>();
            Record? current = null;
            RecordMonth? currentMonth = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("--- Registro:"))
                {
                    if (current != null)
                        list.Add(current);

                    current = new Record();
                }

                else if (current != null)
                {
                    // Dados pessoais
                    if (line.StartsWith("Nome:"))
                        current.Name = line.Substring("Nome:".Length).Trim();
                    else if (line.StartsWith("E-mail:"))
                        current.Email = line.Substring("E-mail:".Length).Trim();
                    else if (line.StartsWith("Contacto / Tel:"))
                        current.Contact = line.Substring("Contacto / Tel:".Length).Trim();
                    else if (line.StartsWith("Estado Civil:"))
                        current.MaritalStatus = line.Substring("Período:".Length).Trim();
                    else if (line.StartsWith("Salário Líquido:"))
                    {
                        var valueStr = line.Substring("Salário Líquido:".Length).Trim()
                                        .Replace("MZN", "").Replace(".", "").Replace(",", ".").Trim();
                        if (decimal.TryParse(valueStr, out var val))
                            current.NetSalary = val;
                    }
                        else if (line.StartsWith("Período:"))
                            current.Periodo = line.Substring("Período:".Length).Trim();
                        else if (line.StartsWith("Vencimentos Brutos:"))
                            current.VencimentosBrutos = line.Substring("Vencimentos Brutos:".Length).Trim();
                        else if (line.StartsWith("Bónus:"))
                            current.Bonus = line.Substring("Bónus:".Length).Trim();
                        else if (line.StartsWith("Seguros:"))
                            current.Seguros = line.Substring("Seguros:".Length).Trim();
                        else if (line.StartsWith("OUTROS:"))
                            current.Outros = line.Substring("OUTROS:".Length).Trim();
                        else if (line.StartsWith("Pagamento via:"))
                            current.PaymentMethod = line.Substring("Pagamento via:".Length).Trim();
                        else if (line.StartsWith("Referência recibo:"))
                            current.ReceiptReference = line.Substring("Referência recibo:".Length).Trim();
                        else if (line.StartsWith("Assinatura gestor:"))
                            current.ManagerSignature = line.Substring("Assinatura gestor:".Length).Trim();
                        else if (line.StartsWith("Data:"))
                        {
                            var dateStr = line.Substring("Data:".Length).Trim();
                            if (DateTime.TryParse(dateStr, out var dt))
                                current.Date = dt;
                        }
                }
            }

            if (current != null)
                list.Add(current);

            return list;
        }

        private List<Record> ParseCsv(string path)
        {
            if (!File.Exists(path))
            {
                _logger.Warn($"Arquivo CSV não encontrado: {path}");
                return new();
            }

            var lines = File.ReadAllLines(path);
            if (lines.Length < 2)
            {
                _logger.Warn("O arquivo CSV não contém dados suficientes.");
                return new();
            }

            var headers = lines[0].Split(',').Select(h => h.Trim()).ToArray();
            var indexes = headers.Select((h, i) => new { h, i })
                                 .Where(x => RequiredFields.Contains(x.h, StringComparer.OrdinalIgnoreCase))
                                 .ToDictionary(x => x.h, x => x.i);

            var list = new List<Record>();
            foreach (var line in lines.Skip(1))
            {
                var cols = line.Split(',');
                var record = Record.FromArray(cols, indexes);
                if (record != null)
                    list.Add(record);
            }

            return list.Where(r => !string.IsNullOrWhiteSpace(r?.Name)).ToList();
        }

        private List<Record> ParseExcel(string path)
        {
            if (!File.Exists(path))
            {
                _logger.Warn($"Arquivo Excel não encontrado: {path}");
                return new();
            }

            using var package = new ExcelPackage(new FileInfo(path));
            var sheet = package.Workbook.Worksheets.FirstOrDefault();
            if (sheet == null)
            {
                _logger.Warn("Planilha não encontrada no arquivo Excel.");
                return new();
            }

            var headers = Enumerable.Range(1, sheet.Dimension.Columns)
                                    .Select(i => sheet.Cells[1, i].Text.Trim())
                                    .ToList();

            var indexes = headers.Select((h, i) => new { h, i })
                                 .Where(x => RequiredFields.Contains(x.h, StringComparer.OrdinalIgnoreCase))
                                 .ToDictionary(x => x.h, x => x.i + 1);

            var list = new List<Record>();
            for (int row = 2; row <= sheet.Dimension.Rows; row++)
            {
                var record = Record.FromExcel(sheet, row, indexes);
                if (record != null)
                    list.Add(record);
            }

            return list.Where(r => !string.IsNullOrWhiteSpace(r?.Name)).ToList();
        }

        private List<Record> ParseJson(string path)
        {
            if (!File.Exists(path))
            {
                _logger.Warn($"Arquivo JSON não encontrado: {path}");
                return new List<Record>();
            }

            var json = File.ReadAllText(path);
            var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);

            if (items == null || items.Count == 0)
            {
                _logger.Warn("Arquivo JSON vazio ou inválido.");
                return new List<Record>();
            }

            // Filtra nulos explicitamente
            var records = items
                .Select(i => Record.FromJson(i))
                .Where(r => r != null)
                .Cast<Record>() // força para Record, pois já filtramos null
                .ToList();

            return records;
        }
        

        private List<Record> LogAndReturnEmpty(string message)
        {
            _logger.Warn(message);
            return new();
        }
    }
}
