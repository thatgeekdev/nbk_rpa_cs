using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using NBK_RPA_CS.Services;



namespace NBK_RPA_CS.Helpers
{
    public static class WaitHelper
    {
        public static IWebElement WaitForElement(IWebDriver driver, By by, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => d.FindElement(by));
        }

        public static bool TryHandleAlert(IWebDriver driver, LoggerService logger, int timeoutSeconds = 3)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                var alert = driver.SwitchTo().Alert();
                logger.Info("Alert present: " + alert.Text);
                alert.Accept();
                logger.Info("Alert accepted.");
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
            catch (Exception ex)
            {
                logger.Warn("Alert handling error: " + ex.Message);
                return false;
            }
        }
    }
}
