using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using automation_qa.UI.Pages;
using automation_qa.Framework;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Login")]
    public class LoginTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private LoginPage _loginPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _navigationBar = new NavigationBar(Driver);
            _loginPage = new LoginPage(Driver);
            
            _navigationBar.GoToLoginPage();
        }
        [Test]
        public void TC_LG1_001_SuccessfulLogin()
        {
            string email = "test@example.com";
            string password = "Password123!";

            _loginPage.Login(email, password);

            Assert.Pass("Ввод данных и нажатие кнопки выполнены успешно");
        }

        [Test]
        public void TC_LG1_002_LoginFormFields()
        {
            bool emailFieldExists = Driver.FindElements(_loginPage.GetEmailFieldLocator()).Count > 0;
            Assert.That(emailFieldExists, Is.True, "Поле Email отсутствует на форме входа");

            By passwordFieldLocator = By.XPath("/html/body/div/div/div/section/div/form/div/div[2]/div/input");
            bool passwordFieldExists = Driver.FindElements(passwordFieldLocator).Count > 0;
            Assert.That(passwordFieldExists, Is.True, "Поле Password отсутствует на форме входа");

            Assert.Pass("Форма входа содержит необходимые поля: Email и Password");
        }

        [Test]
        public void TC_LG1_003_EmptyFieldsValidation()
        {
            _loginPage.ClickLogin();

            Thread.Sleep(1000);

            bool emailEmptyError = Driver.FindElements(_loginPage.GetEmailErrorLocator()).Count > 0;
            bool passwordEmptyError = Driver.FindElements(_loginPage.GetPasswordErrorLocator()).Count > 0;

            Assert.That(emailEmptyError, Is.True, "");
            Assert.That(passwordEmptyError, Is.True, "");

            if (emailEmptyError)
            {
                string emailErrorText = Driver.FindElement(_loginPage.GetEmailErrorLocator()).Text;
                bool correctEmailError = emailErrorText.Contains("required") || emailErrorText.Contains("required");
                Assert.That(correctEmailError, Is.True,
                    "");
            }

            if (passwordEmptyError)
            {
                string passwordErrorText = Driver.FindElement(_loginPage.GetPasswordErrorLocator()).Text;
                bool correctPasswordError = passwordErrorText.Contains("required") || passwordErrorText.Contains("required");
                Assert.That(correctPasswordError, Is.True,
                    "");
            }
        }

        [Test]
        public void TC_LG1_004_InvalidEmailFormatValidation()
        {
            _loginPage.EnterEmail("nnn")
                     .EnterPassword("Password123!");

            _loginPage.ClickLogin();

            Thread.Sleep(1000);

            bool emailFormatError = Driver.FindElements(_loginPage.GetEmailErrorLocator()).Count > 0;
            Assert.That(emailFormatError, Is.True, "");

            if (emailFormatError)
            {
                string emailErrorText = Driver.FindElement(_loginPage.GetEmailErrorLocator()).Text;
                bool correctFormatError = emailErrorText.Contains("Invalid email address") ||
                                         emailErrorText.Contains("Please ensure it follows the format");
                Assert.That(correctFormatError, Is.True,
                    "");
            }
        }

        [Test]
        public void TC_LG1_005_AccountNotFoundError()
        {
            string nonExistentEmail = "nonexistent_user_" + Guid.NewGuid().ToString() + "@example.com";
            string anyPassword = "Password123!";

            _loginPage.Login(nonExistentEmail, anyPassword);

            Thread.Sleep(3000);

            By errorToastLocator = By.XPath("//div[contains(@class, 'Toastify__toast--error')]");

            bool errorToastDisplayed = Driver.FindElements(errorToastLocator).Count > 0;
            Assert.That(errorToastDisplayed, Is.True, "");

            if (errorToastDisplayed)
            {
                string toastText = Driver.FindElement(errorToastLocator).Text;
                bool correctErrorMessage = toastText.Contains("We could not find an account matching the email");
                Assert.That(correctErrorMessage, Is.True, "");
            }

            if (!errorToastDisplayed)
            {
                bool hasErrorInPage = Driver.PageSource.Contains("We could not find an account") ||
                                     Driver.PageSource.Contains("matching the email");
                Assert.That(hasErrorInPage, Is.True, "");
            }
        }

        [Test]
        public void TC_LG1_006_AccountNotLockedAfterMultipleFailedAttempts()
        {
            string email = "existing_user@example.com";
            string password = "WrongPassword123";

            for (int i = 0; i < 5; i++)
            {
                _loginPage.Login(email, password);
                Thread.Sleep(1000);
            }

            Thread.Sleep(2000);

            bool hasLockoutMessage = Driver.PageSource.Contains("temporarily locked") ||
                                    Driver.PageSource.Contains("failed login attempts");

            Assert.That(hasLockoutMessage, Is.False, "");

            bool loginFormVisible = Driver.PageSource.Contains("Sign In") ||
                                   Driver.PageSource.Contains("Login") ||
                                   Driver.PageSource.Contains("Password");

            Assert.That(loginFormVisible, Is.True, "");
        }

        [Test]
        public void TC_LG1_007_CreateAccountLink()
        {
            _loginPage.ClickCreateAccount();
            
            Thread.Sleep(2000);
            
            bool isRegistrationPage = Driver.Url.Contains("/signup");
            
            Assert.That(isRegistrationPage, Is.True, "");

            bool hasRegistrationTitle = Driver.PageSource.Contains("Create an Account");
            
            Assert.That(hasRegistrationTitle, Is.True, "");
        }

        [Test]
        public void TC_LG1_008_Logout()
        {
            TC_LG1_001_SuccessfulLogin();
            
            _navigationBar.Logout();
            
            Thread.Sleep(2000);
            
            bool isLoginPageOrHomePage = Driver.Url.Contains("/signin") || 
                                        Driver.Url.Equals(BaseConfiguration.UiBaseUrl);
            
            Assert.That(isLoginPageOrHomePage, Is.True, "");
            Assert.That(_navigationBar.IsUserLoggedIn(), Is.False, "");
        }

        [Test]
        public void TC_LG1_009_RedirectToDashboardAfterLogin()
        {
            string email = "irishkakhrol@gmail.com";
            string password = "Password123!";

            _loginPage.Login(email, password);

            Thread.Sleep(2000);

            bool redirectedToMainPage = Driver.Url.Contains("localhost:5173") ||
                                      Driver.PageSource.Contains("Green & Tasty");

            Assert.That(redirectedToMainPage, Is.True, "");

            bool userLoggedIn = _navigationBar.IsUserLoggedIn() ||
                               Driver.PageSource.Contains("Book a Table") ||
                               Driver.PageSource.Contains("Reservations");

            Assert.That(userLoggedIn, Is.True, "");
        }

        [Test]
        public void TC_LG1_010_RemainLoggedInAcrossSessions()
        {
            string email = "irishkakhrol@gmail.com";
            string password = "Password123!";

            _loginPage.Login(email, password);
            Thread.Sleep(2000);

            var cookies = Driver.Manage().Cookies.AllCookies;

            Driver.Quit();
            base.Setup(); 

            Driver.Navigate().GoToUrl(BaseConfiguration.UiBaseUrl);

            foreach (var cookie in cookies)
            {
                try
                {
                    Driver.Manage().Cookies.AddCookie(cookie);
                }
                catch (Exception)
                {

                }
            }

            Driver.Navigate().Refresh();
            Thread.Sleep(2000);

            bool isLoggedIn = !Driver.Url.Contains("/signin") &&
                             !Driver.Url.Contains("/login") &&
                             !Driver.PageSource.Contains("Sign In to Your Account");

            Assert.That(isLoggedIn, Is.True, "");
        }

        [Test]
        public void TC_LG1_011_PasswordVisibilityToggle()
        {
            bool eyeIconExists = Driver.FindElements(_loginPage.GetEyeIconButtonLocator()).Count > 0;
            Assert.That(eyeIconExists, Is.True, "");

            _loginPage.EnterPassword("TestPassword123");

            IWebElement passwordField = Driver.FindElement(_loginPage.GetPasswordFieldLocator());

            string initialType = passwordField.GetAttribute("type");
            Assert.That(initialType, Is.EqualTo("password"), "");

            Driver.FindElement(_loginPage.GetEyeIconButtonLocator()).Click();
            Thread.Sleep(500);

            string newType = passwordField.GetAttribute("type");
            Assert.That(newType, Is.EqualTo("text"), "");
        }

        [Test]
        public void TC_LG1_012_CreateAccountLinkFunctionality()
        {
            bool linkExists = Driver.FindElements(_loginPage.GetCreateAccountLinkLocator()).Count > 0;
            Assert.That(linkExists, Is.True, "");

            _loginPage.ClickCreateAccount();

            Thread.Sleep(2000);

            bool isOnSignupPage = Driver.Url.Contains("/signup") ||
                                  Driver.Url.Contains("/register") ||
                                  Driver.PageSource.Contains("Create an Account") ||
                                  Driver.PageSource.Contains("Sign Up");

            Assert.That(isOnSignupPage, Is.True, "");
        }
    }
}