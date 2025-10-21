using System;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using NBK_RPA_CS.Config;
using NBK_RPA_CS.Helpers;
using NBK_RPA_CS.Services;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace NBK_RPA_CS.Services
{
    public class BotService
    {
        private readonly ConfigService _config;
        private readonly LoggerService _logger;
        private readonly string _downloadDir;
        private readonly IWebDriver _driver;
        private readonly FileProcessor _processor;
        private readonly ExportService _export;

        public BotService(ConfigService config, LoggerService logger, ExportService export)
        {
            _config = config;
            _logger = logger;
            _export = export;

            _downloadDir = Path.Combine(Path.GetTempPath(), "rpa_downloads");
            if (!Directory.Exists(_downloadDir))
                Directory.CreateDirectory(_downloadDir);

            _driver = WebDriverFactory.CreateChromeDriver(_downloadDir, headless: true);
            _processor = new FileProcessor(_logger);
        }

        public void Run()
        {
            try
            {
                _logger.Info($"Navigating to {_config.StartUrl}");
                _driver.Navigate().GoToUrl(_config.StartUrl);

                // Pega todos os links de download na tabela
                var downloadLinks = _driver.FindElements(By.CssSelector("table a[download]"))
                                           .Where(a => !string.IsNullOrEmpty(a.GetAttribute("href")))
                                           .ToList();

                _logger.Info($"Found {downloadLinks.Count} downloadable files.");

                foreach (var link in downloadLinks)
                {
                    try
                    {
                        var url = link.GetAttribute("href");
                        var fileName = link.GetAttribute("download") ?? Path.GetFileName(url);
                        var filePath = Path.Combine(_downloadDir, fileName);

                        _logger.Info($"Downloading: {fileName}");

                        // Baixa arquivo via HTTP
                        using (var client = new HttpClient())
                        {
                            var data = client.GetByteArrayAsync(url).Result;
                            File.WriteAllBytes(filePath, data);
                        }

                        _logger.Info($"Downloaded file: {filePath}");

                        // Processa o arquivo e extrai apenas os campos desejados
                        List<Record> records = _processor.ProcessFile(filePath);
                        if (records == null || !records.Any())
                        {
                            _logger.Warn($"No records extracted from {fileName}.");
                            continue;
                        }

                        // Exporta CSV limpo
                        var outPath = _export.ExportToCsv(records, Path.GetFileNameWithoutExtension(fileName) + ".csv");
                        _logger.Info($"CSV exported: {outPath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error processing file {link.GetAttribute("download")}: {ex.Message}");
                    }
                }
            }
            finally
            {
                try { _driver.Quit(); } catch { }
            }
        }
    }
}
