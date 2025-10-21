using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace NBK_RPA_CS.Services
{
    public static class WebDriverFactory
    {
        public static IWebDriver CreateChromeDriver(string downloadDir, bool headless = true)
        {
            if (!Directory.Exists(downloadDir))
                Directory.CreateDirectory(downloadDir);

            var options = new ChromeOptions();
            options.AddUserProfilePreference("download.default_directory", Path.GetFullPath(downloadDir));
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("safebrowsing.enabled", true);

            if (headless)
                options.AddArgument("--headless=new");

            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            return new ChromeDriver(options);
        }
    }
}
