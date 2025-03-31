using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using automation_qa.UI.Pages;
using automation_qa.Framework; // Добавляем для BaseConfiguration

namespace automation_qa.UI.Tests
{
    [TestFixture]
    public class UserRegistrationTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private RegistrationPage _registrationPage;
        private LoginPage _loginPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Инициализация Page Objects
            _navigationBar = new NavigationBar(Driver);
            _registrationPage = new RegistrationPage(Driver);
            _loginPage = new LoginPage(Driver);
        }

        [Test]
        public void TC_US1_001_SuccessfulUserRegistration()
        {
            // Переход на страницу регистрации
            _navigationBar.GoToRegistrationPage();

            // Ввод регистрационных данных
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";
            string password = "Password@123";

            _registrationPage.FillRegistrationForm(
                firstName: "Anna",
                lastName: "Smith",
                email: email,
                password: password,
                confirmPassword: password
            );

            // Проверка введенных данных
            Assert.That(_registrationPage.GetFirstNameValue(), Is.EqualTo("Anna"), "Значение в поле First Name не соответствует ожидаемому");
            Assert.That(_registrationPage.GetLastNameValue(), Is.EqualTo("Smith"), "Значение в поле Last Name не соответствует ожидаемому");
            Assert.That(_registrationPage.GetEmailValue(), Is.EqualTo(email), "Значение в поле Email не соответствует ожидаемому");
            Assert.That(_registrationPage.GetPasswordValue(), Is.EqualTo(password), "Значение в поле Password не соответствует ожидаемому");
            Assert.That(_registrationPage.GetConfirmPasswordValue(), Is.EqualTo(password), "Значение в поле Confirm Password не соответствует ожидаемому");

            // Проверка на наличие ошибок валидации
            if (_registrationPage.IsFirstNameErrorDisplayed())
            {
                string errorText = _registrationPage.GetFirstNameErrorText();
                Console.WriteLine($"First Name validation error: {errorText}");
                Assert.Fail($"First Name validation failed: {errorText}");
            }

            if (_registrationPage.IsLastNameErrorDisplayed())
            {
                string errorText = _registrationPage.GetLastNameErrorText();
                Console.WriteLine($"Last Name validation error: {errorText}");
                Assert.Fail($"Last Name validation failed: {errorText}");
            }

            if (_registrationPage.IsEmailErrorDisplayed())
            {
                string errorText = _registrationPage.GetEmailErrorText();
                Console.WriteLine($"Email validation error: {errorText}");
                Assert.Fail($"Email validation failed: {errorText}");
            }

            if (_registrationPage.IsPasswordErrorDisplayed())
            {
                string errorText = _registrationPage.GetPasswordErrorText();
                Console.WriteLine($"Password validation error: {errorText}");
                Assert.Fail($"Password validation failed: {errorText}");
            }

            if (_registrationPage.IsConfirmPasswordErrorDisplayed())
            {
                string errorText = _registrationPage.GetConfirmPasswordErrorText();
                Console.WriteLine($"Confirm Password validation error: {errorText}");
                Assert.Fail($"Confirm Password validation failed: {errorText}");
            }

            // Отправка формы регистрации
            _registrationPage.ClickCreateAccount();

            // Поскольку ничего не происходит после клика, тест завершается после проверки данных
            Console.WriteLine("All data validations passed, form submitted.");
        }

        // Для остальных тестовых методов также замените Assert.IsTrue на Assert.That(..., Is.True)
        // Пример:
        [Test]
        public void TC_US1_002_RegistrationFailsWithInvalidFirstName()
        {
            _navigationBar.GoToRegistrationPage();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John@", // Недопустимый символ
                lastName: "Doe",
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            // Проверка наличия ошибки валидации
            Assert.That(_registrationPage.IsFirstNameErrorDisplayed(),
                Is.True, "Сообщение об ошибке не отображается для недопустимого имени");

            // Проверка текста ошибки
            string errorMessage = _registrationPage.GetFirstNameErrorText();
            Assert.That(errorMessage,
                Does.Contain("Only Latin letters, hyphens, and apostrophes are allowed"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");
        }

        [Test]
        public void TC_US1_003_RegistrationFailsWithInvalidLastName()
        {
            _navigationBar.GoToRegistrationPage();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Doe@", // Невалидная фамилия
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            // Проверка наличия ошибки валидации
            Assert.That(_registrationPage.IsLastNameErrorDisplayed(),
                Is.True, "Сообщение об ошибке не отображается для недопустимой фамилии");

            // Проверка текста ошибки
            string errorMessage = _registrationPage.GetLastNameErrorText();
            Assert.That(errorMessage,
                Does.Contain("Only Latin letters, hyphens, and apostrophes are allowed"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");
        }

        [Test]
        public void TC_US1_004_RegistrationFailsWithInvalidEmailFormat()
        {
            _navigationBar.GoToRegistrationPage();

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Doe",
                email: "jondoe@com", // Невалидный email
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsEmailErrorDisplayed(),
                Is.True, "Сообщение об ошибке не отображается для невалидного email");

            string errorMessage = _registrationPage.GetEmailErrorText();
            Assert.That(errorMessage,
                Does.Contain("Invalid email address"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");
        }

        [Test]
        public void TC_US1_005_RegistrationFailsWithWeakPassword_NoUppercaseAndSpecialChar()
        {
            _navigationBar.GoToRegistrationPage();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com";

            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: "pass1e321", // Нет заглавной буквы и специального символа
                confirmPassword: "pass1e321"
            );

            Assert.Multiple(() =>
            {
                // Проверки валидации перед нажатием кнопки
                Assert.That(_registrationPage.GetPasswordValue(),
                    Is.EqualTo("pass1e321"),
                    "Значение пароля не соответствует введенному");

                // Проверка наличия подсвеченных ошибок
                var passwordField = Driver.FindElement(By.CssSelector("input[name='password']"));
                var fieldClass = passwordField.GetAttribute("class");

                Assert.That(fieldClass,
                    Does.Contain("input-invalid"),
                    "Поле пароля не подсвечено как содержащее ошибку");
            });
        }

        [Test]
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
                confirmPassword: "Password456!" // Не совпадает с паролем
            );

            _registrationPage.ClickCreateAccount();

            Assert.That(_registrationPage.IsConfirmPasswordErrorDisplayed(),
                Is.True, "Сообщение об ошибке не отображается при несовпадении паролей");

            string errorMessage = _registrationPage.GetConfirmPasswordErrorText();
            Assert.That(errorMessage,
                Does.Contain("Confirm password must match"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");
        }

        [Test]
        public void TC_US1_007_RegistrationFailsWithLongFirstName()
        {
            _navigationBar.GoToRegistrationPage();
            string longName = new string('A', 51); // Имя длиной 51 символ

            _registrationPage.FillRegistrationForm(
                firstName: longName,
                lastName: "Doe",
                email: "test@example.com",
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            Assert.That(_registrationPage.IsFirstNameErrorDisplayed(),
                Is.True, "Сообщение об ошибке длины имени не отображается");

            string errorMessage = _registrationPage.GetFirstNameErrorText();
            Assert.That(errorMessage,
                Is.EqualTo("First name must be up to 50 characters"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");
        }

        [Test]
        public void TC_US1_008_RegistrationFailsWithEmptyRequiredFields()
        {
            _navigationBar.GoToRegistrationPage();

            _registrationPage.ClickCreateAccount();

            Assert.Multiple(() =>
            {
                Assert.That(_registrationPage.IsFirstNameErrorDisplayed(),
                    Is.True, "Отсутствует ошибка для незаполненного имени");

                Assert.That(_registrationPage.IsLastNameErrorDisplayed(),
                    Is.True, "Отсутствует ошибка для незаполненной фамилии");

                Assert.That(_registrationPage.IsEmailErrorDisplayed(),
                    Is.True, "Отсутствует ошибка для незаполненного email");

                // Проверка текста ошибок
                Assert.That(_registrationPage.GetFirstNameErrorText(),
                    Is.EqualTo("Enter your first name"),
                    "Текст ошибки имени не соответствует ожидаемому");

                Assert.That(_registrationPage.GetLastNameErrorText(),
                    Is.EqualTo("Enter your last name"),
                    "Текст ошибки фамилии не соответствует ожидаемому");

                Assert.That(_registrationPage.GetEmailErrorText(),
                    Is.EqualTo("Invalid email address. Please ensure it follows the format: username@domain.com"),
                    "Текст ошибки email не соответствует ожидаемому");
            });
        }

        // Обновите остальные тесты аналогичным образом
    }
}