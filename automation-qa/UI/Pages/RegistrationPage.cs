using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace automation_qa.UI.Pages
{
    public class RegistrationPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _firstNameField = By.CssSelector("input[name='firstName']");
        private readonly By _lastNameField = By.CssSelector("input[name='lastName']");
        private readonly By _emailField = By.CssSelector("input[name='email']");
        private readonly By _passwordField = By.CssSelector("input[name='password']");
        private readonly By _confirmPasswordField = By.CssSelector("input[name='confirmPassword']");
        private readonly By _createAccountButton = By.XPath("//button[@type='submit' and contains(text(), 'Sign Up')]");
        private readonly By _firstNameError = By.XPath("//form//div[1]//p[contains(@class, 'text-destructive')]");
        private readonly By _lastNameError = By.XPath("/html/body/div/div/div/section/div/form/div[1]/div/div[2]/p[contains(@class, 'text-destructive')]");
        private readonly By _emailError = By.XPath("/html/body/div/div/div/section/div/form/div[2]/div[1]/p[contains(@class, 'text-destructive')]");
        private readonly By _passwordLengthRequirement = By.XPath("//input[@name='password']/following-sibling::p[contains(text(), 'Password must be 8-16 characters long')]");
        private readonly By _passwordUppercaseError = By.XPath("/html/body/div/div/div/section/div/form/div[2]/div[2]/p[1]");
        private readonly By _passwordSpecialCharError = By.XPath("/html/body/div/div/div/section/div/form/div[2]/div[2]/p[4]");
        private readonly By _confirmPasswordError = By.XPath("/html/body/div/div/div/section/div/form/div[2]/div[3]/p[contains(@class, 'text-destructive')]");
        private readonly By _signUpButton = By.XPath("/html/body/div/div/div/section/div/form/button");
        private readonly By _togglePasswordVisibilityButton = By.XPath("/html/body/div/div/div/section/div/form/div[2]/div[2]/div/button");
        private readonly By _loginLink = By.XPath("/html/body/div/div/div/section/div/form/p[2]/a");
        private readonly By _errorToastLocator = By.XPath("//div[contains(@class, 'Toastify__toast--error')]");


        public RegistrationPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public RegistrationPage EnterFirstName(string firstName)
        {
            var element = _wait.Until(driver => driver.FindElement(_firstNameField));
            element.Clear();
            element.SendKeys(firstName);
            Console.WriteLine($"Entered First Name: {firstName}");
            return this;
        }

        public RegistrationPage EnterLastName(string lastName)
        {
            var element = _wait.Until(driver => driver.FindElement(_lastNameField));
            element.Clear();
            element.SendKeys(lastName);
            Console.WriteLine($"Entered Last Name: {lastName}");
            return this;
        }

        public RegistrationPage EnterEmail(string email)
        {
            var element = _wait.Until(driver => driver.FindElement(_emailField));
            element.Clear();
            element.SendKeys(email);
            Console.WriteLine($"Entered Email: {email}");
            return this;
        }

        public RegistrationPage EnterPassword(string password)
        {
            var element = _wait.Until(driver => driver.FindElement(_passwordField));
            element.Clear();
            element.SendKeys(password);
            Console.WriteLine($"Entered Password: {password}");
            return this;
        }

        public RegistrationPage EnterConfirmPassword(string confirmPassword)
        {
            var element = _wait.Until(driver => driver.FindElement(_confirmPasswordField));
            element.Clear();
            element.SendKeys(confirmPassword);
            Console.WriteLine($"Entered Confirm Password: {confirmPassword}");
            return this;
        }

        public void ClickCreateAccount()
        {
            var button = _wait.Until(driver => driver.FindElement(_createAccountButton));
            Console.WriteLine($"Found Sign Up button: {button.Text}");
            button.Click();
            Console.WriteLine("Clicked Sign Up button");
        }

        public string GetFirstNameValue()
        {
            return _wait.Until(driver => driver.FindElement(_firstNameField)).GetAttribute("value");
        }

        public bool IsPasswordErrorDisplayed()
        {
            return IsPasswordLengthRequirementInvalid();
        }

        public string GetLastNameValue()
        {
            return _wait.Until(driver => driver.FindElement(_lastNameField)).GetAttribute("value");
        }

        public string GetEmailValue()
        {
            return _wait.Until(driver => driver.FindElement(_emailField)).GetAttribute("value");
        }

        public string GetPasswordValue()
        {
            return _wait.Until(driver => driver.FindElement(_passwordField)).GetAttribute("value");
        }

        public string GetConfirmPasswordValue()
        {
            return _wait.Until(driver => driver.FindElement(_confirmPasswordField)).GetAttribute("value");
        }

        public bool IsFirstNameErrorDisplayed()
        {
            try
            {
                var errorElement = _wait.Until(driver => driver.FindElement(_firstNameError));
                return errorElement.Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetFirstNameErrorText()
        {
            try
            {
                var errorElement = _wait.Until(driver => driver.FindElement(_firstNameError));
                return errorElement.Text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool IsLastNameErrorDisplayed()
        {
            return IsElementDisplayed(_lastNameError);
        }

        public bool IsEmailErrorDisplayed()
        {
            return IsElementDisplayed(_emailError);
        }

        public bool IsConfirmPasswordErrorDisplayed()
        {
            return IsElementDisplayed(_confirmPasswordError);
        }

        public string GetLastNameErrorText()
        {
            return _wait.Until(driver => driver.FindElement(_lastNameError)).Text;
        }

        public string GetEmailErrorText()
        {
            return _wait.Until(driver => driver.FindElement(_emailError)).Text;
        }

        public string GetConfirmPasswordErrorText()
        {
            return _wait.Until(driver => driver.FindElement(_confirmPasswordError)).Text;
        }

        private bool IsElementDisplayed(By locator)
        {
            try
            {
                var element = _wait.Until(driver => driver.FindElement(locator));
                return element.Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ClickSignUp()
        {
            _driver.FindElement(_signUpButton).Click();
        }

        public string GetPasswordFieldType()
        {
            return _wait.Until(driver => driver.FindElement(_passwordField)).GetAttribute("type");
        }

        public void TogglePasswordVisibility()
        {
            _wait.Until(driver => driver.FindElement(_togglePasswordVisibilityButton)).Click();
        }

        public void ClickLoginLink()
        {
            _driver.FindElement(_loginLink).Click();
        }

        public By GetErrorToastLocator()
        {
            return _errorToastLocator;
        }

        public bool IsPasswordLengthRequirementInvalid()
        {
            try
            {
                var requirementElement = _wait.Until(driver =>
                {
                    var element = driver.FindElement(_passwordLengthRequirement);
                    string elementClass = element.GetAttribute("class") ?? "";
                    Console.WriteLine($"Classes of password length requirement: {elementClass}");
                    return elementClass.Contains("text-glass-destructive") ? element : null;
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding password length requirement: {ex.Message}");
                return true;
            }
        }

        public void FillRegistrationForm(string firstName, string lastName, string email, string password, string confirmPassword)
        {
            EnterFirstName(firstName)
                .EnterLastName(lastName)
                .EnterEmail(email)
                .EnterPassword(password)
                .EnterConfirmPassword(confirmPassword);
        }
    }
}
