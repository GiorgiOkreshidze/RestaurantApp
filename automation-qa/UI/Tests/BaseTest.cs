using System;
using NUnit.Framework;
using OpenQA.Selenium;
using automation_qa.Drivers;
using automation_qa.Framework;

namespace automation_qa.UI.Tests
{
    public class BaseTest
    {
        protected BrowserDriverManager DriverManager;
        protected IWebDriver Driver;

        [SetUp]
        public virtual void Setup()
        {
            DriverManager = new BrowserDriverManager();
            DriverManager.InitializeDriver(BaseConfiguration.DefaultBrowser);
            DriverManager.MaximizeWindow();
            DriverManager.SetImplicitWait(TimeSpan.FromSeconds(BaseConfiguration.DefaultTimeout));
            DriverManager.NavigateToUrl(BaseConfiguration.UiBaseUrl);

            Driver = DriverManager.GetDriver();
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (Driver != null)
            {
                Driver.Quit();
                Driver.Dispose();
                Driver = null;
            }

            DriverManager?.QuitDriver();
            DriverManager = null;
        }
    }
}