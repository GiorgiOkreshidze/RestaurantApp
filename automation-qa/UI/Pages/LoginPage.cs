using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace automation_qa.UI.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _emailField = By.XPath("/html/body/div/div/div/section/div/form/div/div[1]/input");
        private readonly By _passwordField = By.XPath("/html/body/div/div/div/section/div/form/div/div[2]/div/input");
        private readonly By _loginButton = By.XPath("/html/body/div/div/div/section/div/form/button");
        private readonly By _createAccountLink = By.XPath("/html/body/div/div/div/section/div/form/p[2]/a");

        private readonly By _emailError = By.XPath("/html/body/div/div/div/section/div/form/div/div[1]/p");
        private readonly By _passwordError = By.XPath("/html/body/div/div/div/section/div/form/div/div[2]/p");
        private readonly By _generalError = By.CssSelector(".bg-error-light");
        private readonly By errorToastLocator = By.XPath("//div[contains(@class, 'Toastify__toast--error') and contains(@class, 'Toastify__toast-theme--light')]");
        private readonly By eyeIconButton = By.XPath("/html/body/div/div/div/section/div/form/div/div[2]/div/button");

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public LoginPage EnterEmail(string email)
        {
            _driver.FindElement(_emailField).Clear();
            _driver.FindElement(_emailField).SendKeys(email);
            return this;
        }

        public LoginPage EnterPassword(string password)
        {
            _driver.FindElement(_passwordField).Clear();
            _driver.FindElement(_passwordField).SendKeys(password);
            return this;
        }

        public void ClickLogin()
        {
            _driver.FindElement(_loginButton).Click();
        }

        public void ClickCreateAccount()
        {
            _driver.FindElement(_createAccountLink).Click();
        }

        public bool IsEmailErrorDisplayed()
        {
            return IsElementDisplayed(_emailError);
        }

        public bool IsPasswordErrorDisplayed()
        {
            return IsElementDisplayed(_passwordError);
        }

        public By GetEmailErrorLocator()
        {
            return _emailError;
        }

        public By GetPasswordFieldLocator()
        {
            return _passwordField;
        }

        public By GetPasswordErrorLocator()
        {
            return _passwordError;
        }

        private bool IsElementDisplayed(By locator)
        {
            try
            {
                return _driver.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void Login(string email, string password)
        {
            EnterEmail(email)
                .EnterPassword(password);
            ClickLogin();
        }

        public bool IsLoginPage()
        {
            return _driver.Url.Contains("/login");
        }

        public By GetEmailFieldLocator()
        {
            return _emailField;
        }

        public By GetCreateAccountLinkLocator()
        {
            return _createAccountLink;
        }

        public By GetEyeIconButtonLocator()
        {
            return eyeIconButton;
        }

    }
}
