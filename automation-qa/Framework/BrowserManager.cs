using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace automation_qa.Framework
{
    public class BrowserManager
    {
        public static IWebDriver InitializeBrowser(string browserName)
        {
            return browserName.ToLower() switch
            {
                "chrome" => new ChromeDriver(),
                "firefox" => new FirefoxDriver(),
                _ => throw new ArgumentException($"Unsupported browser: {browserName}")
            };
        }

        public static void CloseBrowser(IWebDriver driver)
        {
            driver?.Quit();
        }
    }
}
