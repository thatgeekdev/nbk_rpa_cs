using OpenQA.Selenium;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using NBK_RPA_CS.Services;
using NBK_RPA_CS.Config;
using NBK_RPA_CS.Processors;
using NBK_RPA_CS.Helpers;



namespace NBK_RPA_CS.Services
{
    public class BotService
    {
        private readonly ConfigService _config;
        private readonly LoggerService _logger;
        private readonly IWebDriver _driver;
        private readonly string _downloadDir;
        private readonly FileProcessor _processor;
        private readonly ExportService _export;
        private readonly ExcelBalanceteService _excelService;

        public BotService(ConfigService config, LoggerService logger, ExportService export)
        {
            _config = config;
            _logger = logger;
            _export = export;

            _downloadDir = Path.Combine(Path.GetTempPath(), "rpa_downloads");
            Directory.CreateDirectory(_downloadDir);

            _driver = WebDriverFactory.CreateChromeDriver(_downloadDir, headless: false);
            _processor = new FileProcessor(_logger);
            _excelService = new ExcelBalanceteService("Exports");
        }

        public void Run()
        {
            var allRecords = new List<Record>();
            try
            {
                _logger.Info($"🌐 Acedendo a: {_config.StartUrl}");
                _driver.Navigate().GoToUrl(_config.StartUrl);

                WaitHelper.TryHandleAlert(_driver, _logger);

                var links = _driver.FindElements(By.CssSelector("table a[download]"))
                                   .Where(a => !string.IsNullOrEmpty(a.GetAttribute("href")))
                                   .ToList();

                _logger.Info($"🔗 Encontrados {links.Count} ficheiros.");

                foreach (var link in links)
                {
                    var url = link.GetAttribute("href");
                    var name = link.GetAttribute("download") ?? Path.GetFileName(url);
                    var path = Path.Combine(_downloadDir, name ?? string.Empty);

                    try
                    {
                        _logger.Info($"⬇️ Downloading: {name}");
                        using (var client = new HttpClient())
                        {
                            var data = client.GetByteArrayAsync(url).Result;
                            File.WriteAllBytes(path, data);
                        }

                        var records = _processor.ProcessFile(path);
                        if (records.Any())
                        {
                            var output = _export.ExportToCsv(records, Path.GetFileNameWithoutExtension(name) + "_clean.csv");
                            _logger.Info($"📦 Exported: {output}");
                            // Acumula para o Balancete final
                            allRecords.AddRange(records);
                        }
                        else
                        {
                            _logger.Warn($"Nenhum dado válido em {name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Erro com {name}: {ex.Message}");

                    }
                }
                     if (allRecords.Any())
    {
        // Converte
        var allRecordsTxt = allRecords.Select(r => r.ToRecordTxt(
            periodo: r.Periodo,          // você pode adaptar conforme extração
            vencimentosBrutos: r.VencimentosBrutos,
            bonus: r.Bonus,
            seguros: r.Seguros,
            outros: r.Outros,
            paymentMethod: r.PaymentMethod,
            receiptReference: r.ReceiptReference,
            managerSignature: r.ManagerSignature,
            date: r.Date
        )).ToList();

        var balancetePath = _excelService.Generate(allRecordsTxt);
        _logger.Info($"📊 Balancete final gerado: {balancetePath}");
    }

                    else
                    {
                        _logger.Warn("Nenhum registro válido encontrado. Balancete final não gerado.");
                    }
            }
            finally
            {
                _driver.Quit();
            }
        }
    }
}
