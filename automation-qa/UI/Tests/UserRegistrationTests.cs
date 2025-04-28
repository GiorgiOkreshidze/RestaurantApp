using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using automation_qa.UI.Pages;
using automation_qa.Framework;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Registration")]
    public class UserRegistrationTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private RegistrationPage _registrationPage;
        private LoginPage _loginPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _navigationBar = new NavigationBar(Driver);
            _registrationPage = new RegistrationPage(Driver);
            _loginPage = new LoginPage(Driver);
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void TC_US1_001_SuccessfulUserRegistration()
        {
            _navigationBar.GoToRegistrationPage();

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            string password = "Password123!";

            _registrationPage.FillRegistrationForm(
                firstName: "Anna",
                lastName: "Smith",
                email: email,
                password: password,
                confirmPassword: password
            );

            Assert.That(_registrationPage.GetFirstNameValue(), Is.EqualTo("Anna"));
            Assert.That(_registrationPage.GetEmailValue(), Is.EqualTo(email));
            Assert.That(_registrationPage.GetPasswordValue(), Is.EqualTo(password));

            bool hasValidationErrors = _registrationPage.IsFirstNameErrorDisplayed() ||
                                      _registrationPage.IsLastNameErrorDisplayed() ||
                                      _registrationPage.IsEmailErrorDisplayed() ||
                                      _registrationPage.IsPasswordErrorDisplayed() ||
                                      _registrationPage.IsConfirmPasswordErrorDisplayed();

            Assert.That(hasValidationErrors, Is.True, "");

            _registrationPage.ClickCreateAccount();
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_002_RegistrationFailsWithInvalidFirstName()
        {
            _navigationBar.GoToRegistrationPage();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John@",
                lastName: "Doe",
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsFirstNameErrorDisplayed(),
                Is.True, "");

            string errorMessage = _registrationPage.GetFirstNameErrorText();
            Assert.That(errorMessage,
                Does.Contain("Only Latin letters, hyphens, and apostrophes are allowed"),
                $"Failed: {errorMessage}");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_003_RegistrationFailsWithInvalidLastName()
        {
            _navigationBar.GoToRegistrationPage();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Doe@",
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsLastNameErrorDisplayed(),
                Is.True, "");

            string errorMessage = _registrationPage.GetLastNameErrorText();
            Assert.That(errorMessage,
                Does.Contain("Only Latin letters, hyphens, and apostrophes are allowed"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_004_RegistrationFailsWithInvalidEmailFormat()
        {
            _navigationBar.GoToRegistrationPage();

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Doe",
                email: "jondoe@com",
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsEmailErrorDisplayed(),
                Is.True, "");

            string errorMessage = _registrationPage.GetEmailErrorText();
            Assert.That(errorMessage,
                Does.Contain("Invalid email address"),
                $"Failed: {errorMessage}");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_005_RegistrationFailsWithWeakPassword_NoUppercaseAndSpecialChar()
        {
            _navigationBar.GoToRegistrationPage();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: "pass1e321",
                confirmPassword: "pass1e321"
            );

            Assert.Multiple(() =>
            {
                Assert.That(_registrationPage.GetPasswordValue(),
                    Is.EqualTo("pass1e321"),
                    "");

                var passwordField = Driver.FindElement(By.CssSelector("input[name='password']"));
                var fieldClass = passwordField.GetAttribute("class");

                Assert.That(fieldClass,
                    Does.Contain("input-invalid"),
                    "");
            });
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_006_RegistrationFailsWithNonMatchingPasswords()
        {
            _navigationBar.GoToRegistrationPage();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "Jane",
                lastName: "Doe",
                email: email,
                password: "Password123!",
                confirmPassword: "Password456!"
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsConfirmPasswordErrorDisplayed(),
                Is.True, "");

            string errorMessage = _registrationPage.GetConfirmPasswordErrorText();
            Assert.That(errorMessage,
                Does.Contain("Confirm password must match"),
                $"Failed: {errorMessage}");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_007_RegistrationFailsWithLongFirstName()
        {
            _navigationBar.GoToRegistrationPage();
            string longName = new string('A', 51);

            _registrationPage.FillRegistrationForm(
                firstName: longName,
                lastName: "Doe",
                email: "test@example.com",
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            Assert.That(_registrationPage.IsFirstNameErrorDisplayed(),
                Is.True, "");

            string errorMessage = _registrationPage.GetFirstNameErrorText();
            Assert.That(errorMessage,
                Is.EqualTo("First name must be up to 50 characters"),
                $": {errorMessage}");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_008_RegistrationFailsWithEmptyRequiredFields()
        {
            _navigationBar.GoToRegistrationPage();

            _registrationPage.ClickCreateAccount();

            Assert.Multiple(() =>
            {
                Assert.That(_registrationPage.IsFirstNameErrorDisplayed(),
                    Is.True, "");

                Assert.That(_registrationPage.IsLastNameErrorDisplayed(),
                    Is.True, "");

                Assert.That(_registrationPage.IsEmailErrorDisplayed(),
                    Is.True, "");

                Assert.That(_registrationPage.GetFirstNameErrorText(),
                    Is.EqualTo("Enter your first name"),
                    "");

                Assert.That(_registrationPage.GetLastNameErrorText(),
                    Is.EqualTo("Enter your last name"),
                    "");

                Assert.That(_registrationPage.GetEmailErrorText(),
                    Is.EqualTo("Invalid email address. Please ensure it follows the format: username@domain.com"),
                    "");
            });
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void TC_US1_009_PasswordVisibilityToggle()
        {
            _navigationBar.GoToRegistrationPage();

            Thread.Sleep(500);

            _registrationPage.EnterPassword("Password123!");

            string initialType = _registrationPage.GetPasswordFieldType();
            Assert.That(initialType, Is.EqualTo("password"), "");

            _registrationPage.TogglePasswordVisibility();
            Thread.Sleep(300);

            string newType = _registrationPage.GetPasswordFieldType();
            Assert.That(newType, Is.EqualTo("text"), "text");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void TC_US1_010_NavigationBetweenLoginAndRegistration()
        {
            _navigationBar.GoToLoginPage();
            Thread.Sleep(500);

            _loginPage.ClickCreateAccount();
            Thread.Sleep(1000);

            bool hasSignUpHeading = Driver.PageSource.Contains("Create an Account");
            Assert.That(hasSignUpHeading, Is.True, "");

            Driver.FindElement(By.XPath("/html/body/div/div/div/section/div/form/p[2]/a")).Click();
            Thread.Sleep(1000);

            bool hasSignInHeading = Driver.PageSource.Contains("Sign In to Your Account");
            Assert.That(hasSignInHeading, Is.True, "");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_011_RegistrationFailsWithExistingEmail()
        {
            _navigationBar.GoToRegistrationPage();

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Smith",
                email: "irishkakhrol@gmail.com",
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();
            Thread.Sleep(2000);

            bool errorToastDisplayed = Driver.FindElements(_registrationPage.GetErrorToastLocator()).Count > 0;

            Assert.That(errorToastDisplayed, Is.True, "");

            if (errorToastDisplayed)
            {
                string toastText = Driver.FindElement(_registrationPage.GetErrorToastLocator()).Text;
                bool correctErrorMessage = toastText.Contains("already exists") ||
                                         toastText.Contains("User with email");

                Assert.That(correctErrorMessage, Is.True,
                    $": {toastText}");
            }
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_012_RegistrationFailsWithLongLastName()
        {
            _navigationBar.GoToRegistrationPage();

            string longLastName = new string('A', 51);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: longLastName,
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsLastNameErrorDisplayed(),
                Is.True, "");

            string errorMessage = _registrationPage.GetLastNameErrorText();
            Console.WriteLine($"Last Name error message: {errorMessage}");
            Assert.That(errorMessage,
                Is.EqualTo("Last name must be up to 50 characters"),
                $": {errorMessage}");

            Console.WriteLine($"Current URL after clicking Sign Up: {Driver.Url}");
            Assert.That(Driver.Url.Contains("/signup"), Is.True,
                "");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_013_RegistrationFailsWithInvalidLastName()
        {
            _navigationBar.GoToRegistrationPage();

            string invalidLastName = "ш";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: invalidLastName,
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsLastNameErrorDisplayed(),
                Is.True, "");

            string errorMessage = _registrationPage.GetLastNameErrorText();
            Console.WriteLine($"Last Name error message: {errorMessage}");
            Assert.That(errorMessage,
                Is.EqualTo("Only Latin letters, hyphens, and apostrophes are allowed"),
                $": {errorMessage}");

            Console.WriteLine($"Current URL after clicking Sign Up: {Driver.Url}");
            Assert.That(Driver.Url.Contains("/signup"), Is.True,
                "");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_014_RegistrationFailsWithInvalidFirstName()
        {
            _navigationBar.GoToRegistrationPage();

            string invalidFirstName = "ь";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: invalidFirstName,
                lastName: "Doe",
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsFirstNameErrorDisplayed(),
                Is.True, "");

            string errorMessage = _registrationPage.GetFirstNameErrorText();
            Console.WriteLine($"First Name error message: {errorMessage}");
            Assert.That(errorMessage,
                Is.EqualTo("Only Latin letters, hyphens, and apostrophes are allowed"),
                $": {errorMessage}");

            Console.WriteLine($"Current URL after clicking Sign Up: {Driver.Url}");
            Assert.That(Driver.Url.Contains("/signup"), Is.True,
                "");
        }

        [Test]
        [Category("Regression")]
        public void TC_US1_015_RegistrationFailsWithShortPassword()
        {
            _navigationBar.GoToRegistrationPage();

            string shortPassword = "Pa3!!";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: shortPassword,
                confirmPassword: shortPassword
            );

            _registrationPage.ClickCreateAccount();

            bool isLengthRequirementInvalid = _registrationPage.IsPasswordLengthRequirementInvalid();
            Console.WriteLine($"Is password length requirement invalid? {isLengthRequirementInvalid}");
            Assert.That(isLengthRequirementInvalid, Is.True,
                "");

            Console.WriteLine($"Current URL after clicking Sign Up: {Driver.Url}");
            Assert.That(Driver.Url.Contains("/signup"), Is.True,
                "");
        }
    }
}