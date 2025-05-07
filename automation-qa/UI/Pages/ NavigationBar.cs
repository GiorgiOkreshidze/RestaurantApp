using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace automation_qa.UI.Pages
{
    public class NavigationBar
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _signInButton = By.CssSelector("a[href='/signin']");
        private readonly By _loginLink = By.CssSelector("[data-testid='login-link']");
        private readonly By _createAccountLink = By.CssSelector("a[href='/signup']");
        private readonly By _logoutLink = By.LinkText("Sign Out");
        private readonly By _reservationsLink = By.XPath("//a[contains(text(), 'Reservations')]");

        public NavigationBar(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void ClickSignInButton()
        {
            _driver.FindElement(_signInButton).Click();
        }

        public void GoToLoginPage()
        {
            ClickSignInButton();
        }

        public void GoToRegistrationPage()
        {
            _driver.FindElement(_signInButton).Click();

            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            wait.Until(driver => driver.FindElement(_createAccountLink)).Click();
        }

        public void Logout()
        {
            try
            {
                _driver.FindElement(_logoutLink).Click();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("");
            }
        }

        public bool IsUserLoggedIn()
        {
            try
            {
                return _driver.FindElement(_logoutLink).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void GoToReservationsPage()
        {
            try
            {
                _driver.FindElement(_reservationsLink).Click();
            }
            catch (NoSuchElementException)
            {
                // If the link isn't found, try navigating directly to the URL
                _driver.Navigate().GoToUrl(_driver.Url.Split('/')[0] + "/reservations");
            }
        }
    }
}
