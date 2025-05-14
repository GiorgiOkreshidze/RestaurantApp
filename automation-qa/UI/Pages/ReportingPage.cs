using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace automation_qa.UI.Pages
{
    public class ReportingPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly By _reportsLink = By.XPath("/html/body/div/header/div/section/div[2]/a");
        private readonly By _reportTypeDropdown = By.XPath("//div[contains(text(), 'Select report type')]");

        public ReportingPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void NavigateToReports()
        {
            _driver.FindElement(_reportsLink).Click();
            _wait.Until(d => d.FindElement(_reportTypeDropdown).Displayed);
        }

        public bool IsReportsPageLoaded()
        {
            try
            {
                return _driver.FindElement(_reportTypeDropdown).Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}
