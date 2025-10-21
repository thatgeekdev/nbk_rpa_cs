using System;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using NBK_RPA_CS.Config;
using NBK_RPA_CS.Processors;

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
            Directory.CreateDirectory(_downloadDir);

            _driver = WebDriverFactory.CreateChromeDriver(_downloadDir, headless: false);
            _processor = new FileProcessor(_logger);
        }

        public void Run()
        {
            try
            {
                _logger.Info($"Navigating to {_config.StartUrl}");
                _driver.Navigate().GoToUrl(_config.StartUrl);

                var links = _driver.FindElements(By.CssSelector("a[href]"))
                    .Where(a => a.GetAttribute("href") != null &&
                                (a.GetAttribute("href").EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ||
                                 a.GetAttribute("href").EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                                 a.GetAttribute("href").EndsWith(".xls", StringComparison.OrdinalIgnoreCase) ||
                                 a.GetAttribute("href").EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                _logger.Info($"Found {links.Count} file links.");

                foreach (var link in links)
                {
                    try
                    {
                        var url = link.GetAttribute("href");
                        _logger.Info($"Downloading {url}");
                        try { link.Click(); } catch { _driver.Navigate().GoToUrl(url); }

                        var file = WaitForDownloadedFile(_downloadDir, TimeSpan.FromSeconds(_config.DownloadWaitSeconds));
                        if (file == null)
                        {
                            _logger.Warn("Download not detected within timeout.");
                            continue;
                        }

                        _logger.Info($"Downloaded: {file}");

                        var records = _processor.ProcessFile(file);
                        if (!records.Any())
                        {
                            _logger.Warn("No valid records extracted from file.");
                            continue;
                        }

                        var outPath = _export.ExportNormalized(records);
                        _logger.Info($"Exported normalized CSV: {outPath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error processing link: " + ex.Message);
                    }
                }
            }
            finally
            {
                try { _driver.Quit(); } catch { }
            }
        }

        private string? WaitForDownloadedFile(string dir, TimeSpan timeout)
        {
            var stop = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < stop)
            {
                var files = Directory.GetFiles(dir)
                    .Where(f => !f.EndsWith(".crdownload") && !f.EndsWith(".tmp"))
                    .ToList();
                if (files.Any()) return files.OrderByDescending(File.GetLastWriteTimeUtc).First();
                Thread.Sleep(500);
            }
            return null;
        }
    }
}
