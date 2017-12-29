using Microsoft.Extensions.PlatformAbstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReduxSamples
{
    public class BrowserDriver: IDisposable
    {
        private object _browserLock = new object();

        private ChromeDriverService _driverService;
        private ChromeOptions _options;
        private ChromeDriver _driver;

        public BrowserDriver()
        {
            _driverService = ChromeDriverService.CreateDefaultService(PlatformServices.Default.Application.ApplicationBasePath, "chromedriver.exe");
            _options = new ChromeOptions();
            //_options.AddAdditionalCapability("IsJavaScriptEnabled", true);
        }

        public void LaunchUrl(string targetURL)
        {
            if (_driver != null)
            {
                Close();
            }

            _driver = new ChromeDriver(_driverService, _options);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));

            _driver.Navigate().GoToUrl(targetURL);

            wait.Until(driver => driver.FindElement(By.TagName("body")));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            IEnumerable<LogEntry> logs = _driver.Manage().Logs.GetLog("browser");

            if (logs.Any(l => l.Level == LogLevel.Warning || l.Level == LogLevel.Severe))
            {
                throw new Exception($"Warnings/Errors logged: \n{string.Join("/n", logs.Select(l => l.Timestamp + ":::" + l.Message))}");
            }
        }

        public void Close()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _driverService?.Dispose();
                _driverService = null;
            }
        }
    }
}
