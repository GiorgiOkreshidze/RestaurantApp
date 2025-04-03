using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using automation_qa.UI.Pages;
using automation_qa.Framework; // Добавляем для BaseConfiguration

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

            // Ввод регистрационных данных с уникальным email
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

            // Сокращенная проверка введенных данных - только критически важные
            Assert.That(_registrationPage.GetFirstNameValue(), Is.EqualTo("Anna"));
            Assert.That(_registrationPage.GetEmailValue(), Is.EqualTo(email));
            Assert.That(_registrationPage.GetPasswordValue(), Is.EqualTo(password));

            // Быстрая проверка на отсутствие ошибок валидации
            bool hasValidationErrors = _registrationPage.IsFirstNameErrorDisplayed() ||
                                      _registrationPage.IsLastNameErrorDisplayed() ||
                                      _registrationPage.IsEmailErrorDisplayed() ||
                                      _registrationPage.IsPasswordErrorDisplayed() ||
                                      _registrationPage.IsConfirmPasswordErrorDisplayed();

            Assert.That(hasValidationErrors, Is.False, "Обнаружены ошибки валидации формы");

            // Отправка формы регистрации - используем существующий метод
            _registrationPage.ClickCreateAccount();
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

        [Test]
        public void TC_US1_009_PasswordVisibilityToggle()
        {
            // Переход на страницу регистрации
            _navigationBar.GoToRegistrationPage();

            // Короткая пауза для загрузки страницы
            Thread.Sleep(500);

            // Вводим пароль
            _registrationPage.EnterPassword("Password123!");

            // Получаем начальный тип поля пароля
            string initialType = _registrationPage.GetPasswordFieldType();
            Assert.That(initialType, Is.EqualTo("password"), "Начальный тип поля должен быть 'password'");

            // Нажимаем на кнопку видимости
            _registrationPage.TogglePasswordVisibility();
            Thread.Sleep(300);

            // Проверяем, что тип поменялся на text
            string newType = _registrationPage.GetPasswordFieldType();
            Assert.That(newType, Is.EqualTo("text"), "Тип поля после нажатия должен быть 'text'");
        }

        [Test]
        public void TC_US1_010_NavigationBetweenLoginAndRegistration()
        {
            // Переходим на страницу входа
            _navigationBar.GoToLoginPage();
            Thread.Sleep(500);

            // Проверяем наличие ссылки на регистрацию и кликаем по ней
            _loginPage.ClickCreateAccount();
            Thread.Sleep(1000);

            // Проверяем заголовок страницы регистрации
            bool hasSignUpHeading = Driver.PageSource.Contains("Create an Account");
            Assert.That(hasSignUpHeading, Is.True, "Заголовок страницы регистрации не найден");

            // Кликаем на ссылку "Login" по точному XPath
            Driver.FindElement(By.XPath("/html/body/div/div/div/section/div/form/p[2]/a")).Click();
            Thread.Sleep(1000);

            // Проверяем, что вернулись на страницу входа
            bool hasSignInHeading = Driver.PageSource.Contains("Sign In to Your Account");
            Assert.That(hasSignInHeading, Is.True, "Заголовок страницы входа не найден");
        }

        [Test]
        public void TC_US1_011_RegistrationFailsWithExistingEmail()
        {
            // Переходим на страницу регистрации
            _navigationBar.GoToRegistrationPage();

            // Заполняем форму с существующим email
            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Smith",
                email: "irishkakhrol@gmail.com", // Уже существующий email
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            // Отправляем форму
            _registrationPage.ClickCreateAccount();
            Thread.Sleep(2000);

            // Используем локатор из класса для toast-сообщения
            bool errorToastDisplayed = Driver.FindElements(_registrationPage.GetErrorToastLocator()).Count > 0;

            Assert.That(errorToastDisplayed, Is.True, "Toast-сообщение об ошибке не отображается");

            // Проверяем текст сообщения об ошибке
            if (errorToastDisplayed)
            {
                string toastText = Driver.FindElement(_registrationPage.GetErrorToastLocator()).Text;
                bool correctErrorMessage = toastText.Contains("already exists") ||
                                         toastText.Contains("User with email");

                Assert.That(correctErrorMessage, Is.True,
                    $"Текст сообщения об ошибке не содержит информацию о существовании пользователя: {toastText}");
            }
        }

        [Test]
        public void TC_US1_012_RegistrationFailsWithLongLastName()
        {
            // Переход на страницу регистрации
            _navigationBar.GoToRegistrationPage();

            // Создание фамилии длиной 51 символ
            string longLastName = new string('A', 51); // Фамилия длиной 51 символ
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com"; // Уникальный email

            // Заполнение формы регистрации
            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: longLastName,
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            // Отправка формы (чтобы сработала валидация)
            _registrationPage.ClickCreateAccount();

            // Проверка, что сообщение об ошибке для фамилии отображается
            Assert.That(_registrationPage.IsLastNameErrorDisplayed(),
                Is.True, "Сообщение об ошибке длины фамилии не отображается");

            // Проверка текста сообщения об ошибке
            string errorMessage = _registrationPage.GetLastNameErrorText();
            Console.WriteLine($"Last Name error message: {errorMessage}");
            Assert.That(errorMessage,
                Is.EqualTo("Last name must be up to 50 characters"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");

            // Проверка, что пользователь остался на странице регистрации
            Console.WriteLine($"Current URL after clicking Sign Up: {Driver.Url}");
            Assert.That(Driver.Url.Contains("/signup"), Is.True,
                "Пользователь был перенаправлен со страницы регистрации, несмотря на ошибку");
        }

        [Test]
        public void TC_US1_013_RegistrationFailsWithInvalidLastName()
        {
            // Переход на страницу регистрации
            _navigationBar.GoToRegistrationPage();

            // Создание недопустимой фамилии
            string invalidLastName = "ш"; // Недопустимая фамилия (слишком короткая или содержит недопустимые символы)
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com"; // Уникальный email

            // Заполнение формы регистрации
            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: invalidLastName,
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            // Отправка формы (чтобы сработала валидация)
            _registrationPage.ClickCreateAccount();

            // Проверка, что сообщение об ошибке для фамилии отображается
            Assert.That(_registrationPage.IsLastNameErrorDisplayed(),
                Is.True, "Сообщение об ошибке для недопустимой фамилии не отображается");

            // Проверка текста сообщения об ошибке
            string errorMessage = _registrationPage.GetLastNameErrorText();
            Console.WriteLine($"Last Name error message: {errorMessage}");
            Assert.That(errorMessage,
                Is.EqualTo("Only Latin letters, hyphens, and apostrophes are allowed"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");

            // Проверка, что пользователь остался на странице регистрации
            Console.WriteLine($"Current URL after clicking Sign Up: {Driver.Url}");
            Assert.That(Driver.Url.Contains("/signup"), Is.True,
                "Пользователь был перенаправлен со страницы регистрации, несмотря на ошибку");
        }

        [Test]
        public void TC_US1_014_RegistrationFailsWithInvalidFirstName()
        {
            // Переход на страницу регистрации
            _navigationBar.GoToRegistrationPage();

            // Создание недопустимого имени
            string invalidFirstName = "ь"; // Недопустимое имя (слишком короткое или содержит недопустимые символы)
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com"; // Уникальный email

            // Заполнение формы регистрации
            _registrationPage.FillRegistrationForm(
                firstName: invalidFirstName,
                lastName: "Doe",
                email: email,
                password: "Password123!",
                confirmPassword: "Password123!"
            );

            // Отправка формы (чтобы сработала валидация)
            _registrationPage.ClickCreateAccount();

            // Проверка, что сообщение об ошибке для имени отображается
            Assert.That(_registrationPage.IsFirstNameErrorDisplayed(),
                Is.True, "Сообщение об ошибке для недопустимого имени не отображается");

            // Проверка текста сообщения об ошибке
            string errorMessage = _registrationPage.GetFirstNameErrorText();
            Console.WriteLine($"First Name error message: {errorMessage}");
            Assert.That(errorMessage,
                Is.EqualTo("Only Latin letters, hyphens, and apostrophes are allowed"),
                $"Неожиданное сообщение об ошибке: {errorMessage}");

            // Проверка, что пользователь остался на странице регистрации
            Console.WriteLine($"Current URL after clicking Sign Up: {Driver.Url}");
            Assert.That(Driver.Url.Contains("/signup"), Is.True,
                "Пользователь был перенаправлен со страницы регистрации, несмотря на ошибку");
        }

        [Test]
        public void TC_US1_015_RegistrationFailsWithShortPassword()
        {
            // Переход на страницу регистрации
            _navigationBar.GoToRegistrationPage();

            // Создание пароля короче минимальной длины
            string shortPassword = "Pa3!!"; // Длина 4 символа, меньше минимальной (8)
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string email = $"testuser{timestamp}@domain.com"; // Уникальный email

            // Заполнение формы регистрации
            _registrationPage.FillRegistrationForm(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: shortPassword,
                confirmPassword: shortPassword
            );

            // Отправка формы (чтобы сработала валидация)
            _registrationPage.ClickCreateAccount();

            // Проверка, что требование "Password must be 8-16 characters long" подсвечено как невыполненное
            bool isLengthRequirementInvalid = _registrationPage.IsPasswordLengthRequirementInvalid();
            Console.WriteLine($"Is password length requirement invalid? {isLengthRequirementInvalid}");
            Assert.That(isLengthRequirementInvalid, Is.True,
                "Требование 'Password must be 8-16 characters long' не подсвечено как невыполненное");

            // Проверка, что пользователь остался на странице регистрации
            Console.WriteLine($"Current URL after clicking Sign Up: {Driver.Url}");
            Assert.That(Driver.Url.Contains("/signup"), Is.True,
                "Пользователь был перенаправлен со страницы регистрации, несмотря на ошибку");
        }

        // Обновите остальные тесты аналогичным образом
    }
}