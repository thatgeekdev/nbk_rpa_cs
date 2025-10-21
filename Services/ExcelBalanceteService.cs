using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace NBK_RPA_CS.Services
{
    public class ExcelBalanceteService
    {
        private readonly string _exportDir;

        public ExcelBalanceteService(string exportDir)
        {
            _exportDir = exportDir;
            Directory.CreateDirectory(_exportDir);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public string Generate(List<RecordTxt> records, string fileName = "BalanceteNedbank.xlsx")
        {
            
            if (records == null || !records.Any())
                throw new ArgumentException("Nenhum registro fornecido.");

            var outPath = Path.Combine(_exportDir, fileName);

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Balancete");

            int row = 1;

            // Título grande verde
            ws.Cells[row, 1].Value = "BALANCETE NEDBANK";
            ws.Cells[row, 1, row, 10].Merge = true;
            ws.Cells[row, 1].Style.Font.Size = 18;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.Font.Color.SetColor(Color.White);
            ws.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.Green);
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row += 2;

            var grouped = records.GroupBy(r => r.Name);

            foreach (var group in grouped)
            {
                var first = group.First();

                // Informações principais do funcionário
                ws.Cells[row, 1].Value = $"Nome: {first.Name}";
                ws.Cells[row, 2].Value = $"E-mail: {first.Email}";
                ws.Cells[row, 3].Value = $"Contacto / Tel: {first.Contact}";
                ws.Cells[row, 4].Value = $"Estado Civil: {first.MaritalStatus}";
                ws.Cells[row, 5].Value = $"Salário Líquido: {first.NetSalary}";
                ws.Cells[row, 6].Value = $"Período Inicial: {first.Periodo}";
                row += 2;

                // Cabeçalho tabela amarelo
                var headers = new string[]
                {
                    "Período","Vencimentos brutos","Bónus","Seguros","Salário Líquido","OUTROS",
                    "Pagamento via","Referência recibo","Assinatura gestor","Data"
                };

                for (int c = 0; c < headers.Length; c++)
                {
                    ws.Cells[row, c + 1].Value = headers[c];
                    ws.Cells[row, c + 1].Style.Font.Bold = true;
                    ws.Cells[row, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, c + 1].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                }
                row++;

                // Corpo da tabela verde claro
                foreach (var r in group)
                {
                    ws.Cells[row, 1].Value = r.Periodo;
                    ws.Cells[row, 2].Value = r.VencimentosBrutos;
                    ws.Cells[row, 3].Value = r.Bonus;
                    ws.Cells[row, 4].Value = r.Seguros;
                    ws.Cells[row, 5].Value = r.NetSalary;
                    ws.Cells[row, 6].Value = r.Outros;
                    ws.Cells[row, 7].Value = r.PaymentMethod;
                    ws.Cells[row, 8].Value = r.ReceiptReference;
                    ws.Cells[row, 9].Value = r.ManagerSignature;
                    ws.Cells[row, 10].Value = r.Date.ToString("yyyy/MM/dd");

                    // Verde claro
                    ws.Cells[row, 1, row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 10].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

                    row++;
                }

                row += 2; // espaço entre funcionários
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            package.SaveAs(new FileInfo(outPath));
            return outPath;
        }
    }
}
