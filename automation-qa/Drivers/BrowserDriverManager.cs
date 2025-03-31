using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
namespace automation_qa.Drivers
{
    public class BrowserDriverManager
    {
        private IWebDriver? _driver;

        // Конструктор должен иметь то же имя, что и класс
        public BrowserDriverManager()
        {
            _driver = null;
        }

        public void InitializeDriver(string browser)
        {
            switch (browser.ToLower())
            {
                case "chrome":
                    var chromeOptions = new ChromeOptions();
                    _driver = new ChromeDriver(chromeOptions);
                    break;
                case "firefox":
                    var firefoxOptions = new FirefoxOptions();
                    _driver = new FirefoxDriver(firefoxOptions);
                    break;
                default:
                    throw new ArgumentException($"Unsupported browser: {browser}");
            }
        }

        // Остальные методы без изменений
        public IWebDriver GetDriver()
        {
            return _driver ?? throw new InvalidOperationException("Driver has not been initialized. Call InitializeDriver first.");
        }

        public void QuitDriver()
        {
            _driver?.Quit();
            _driver = null;
        }

        public void SetImplicitWait(TimeSpan timeout)
        {
            if (_driver == null)
                throw new InvalidOperationException("Driver has not been initialized.");
            _driver.Manage().Timeouts().ImplicitWait = timeout;
        }

        public void MaximizeWindow()
        {
            if (_driver == null)
                throw new InvalidOperationException("Driver has not been initialized.");
            _driver.Manage().Window.Maximize();
        }

        public void NavigateToUrl(string url)
        {
            if (_driver == null)
                throw new InvalidOperationException("Driver has not been initialized.");
            _driver.Navigate().GoToUrl(url);
        }
    }
}