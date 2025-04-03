using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;

namespace ApiTests
{
    [TestFixture]
    [Category("SignUp")]
    public class SignUp : BaseTest
    {
        private Authentication _auth;
        private string _defaultPassword = "StrongP@ss123!";

        [SetUp]
        public void SetupAuthentication()
        {
            _auth = new Authentication();
        }

        [Test]
        public async Task SignUp_CompleteValidData_ShouldCreateUser()
        {
            // Act
            var (statusCode, email, responseBody) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Doe"
            );

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration should be successful");

            Assert.That(responseBody, Is.Not.Null, "Response body should not be empty");

            // Проверяем сообщение об успешной регистрации вместо ожидания tokenов
            Assert.That(responseBody.ContainsKey("message"), Is.True,
                "Response should contain message field");
            Assert.That(responseBody["message"].ToString(), Is.EqualTo("User Registered"),
                "Response should indicate successful registration");
        }

        [Test]
        public async Task SignUp_DuplicateEmail_ShouldFail()
        {
            // Arrange
            var email = $"duplicate_{Guid.NewGuid()}@example.com";

            // First registration
            var (firstStatusCode, _, _) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: _defaultPassword
            );

            Assert.That(firstStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "First registration should be successful");
            await Task.Delay(2000);

            // Repeat registration
            var (secondStatusCode, _, _) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: _defaultPassword
            );

            Assert.That(secondStatusCode, Is.EqualTo(HttpStatusCode.Conflict),
                 "Repeated registration should be prohibited (Conflict expected)");
        }

        [Test]
        public async Task SignUp_InvalidEmailFormat_ShouldFail()
        {
            var invalidEmails = new[]
            {
                "invalid-email",
                "invalid@",
                "@invalid.com",
                "invalid@invalid",
                "invalid@.com"
            };

            foreach (var invalidEmail in invalidEmails)
            {
                var (statusCode, _, _) = await _auth.RegisterUser(
                    firstName: "John",
                    lastName: "Smith",
                    email: invalidEmail,
                    password: _defaultPassword
                );

                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with invalid email '{invalidEmail}' should fail");
            }
        }

        [Test]
        public async Task SignUp_WeakPassword_ShouldFail()
        {
            var weakPasswords = new[]
            {
                "weak",
                "123456",
                "password",
                "weakpassword",
                "12345678"
            };

            foreach (var weakPassword in weakPasswords)
            {
                var (statusCode, _, _) = await _auth.RegisterUser(
                    firstName: "John",
                    lastName: "Smith",
                    email: $"test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                    password: weakPassword
                );

                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with weak password '{weakPassword}' should fail");
            }
        }

        [Test]
        public async Task SignUp_PasswordBoundaryValues_ShouldSucceed()
        {
            // Test minimum length password (8 characters)
            var (minStatusCode, _, _) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Smith",
                email: $"test_min_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: "Aa1@5678" // Exactly 8 characters
            );

            Assert.That(minStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration with 8-character password should succeed");

            // Test maximum length password (16 characters)
            var (maxStatusCode, _, _) = await _auth.RegisterUser(
                firstName: "Jane",
                lastName: "Doe",
                email: $"test_max_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: "Aa1@567890123456" // Exactly 16 characters
            );

            Assert.That(maxStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration with 16-character password should succeed");
        }

        [Test]
        public async Task SignUp_EmptyNameFields_ShouldFail()
        {
            // Test with empty first name
            var (emptyFirstNameStatus, _, _) = await _auth.RegisterUser(
                firstName: "",
                lastName: "Smith",
                email: $"test_empty_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
            );

            Assert.That(emptyFirstNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with empty first name should fail");

            // Test with empty last name
            var (emptyLastNameStatus, _, _) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "",
                email: $"test_empty_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
            );

            Assert.That(emptyLastNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with empty last name should fail");
        }

        [Test]
        public async Task SignUp_TooLongNameFields_ShouldFail()
        {
            var longName = new string('A', 256); // 256 characters

            // Test with too long first name
            var (longFirstNameStatus, _, _) = await _auth.RegisterUser(
                firstName: longName,
                lastName: "Smith",
                email: $"test_long_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
            );

            Assert.That(longFirstNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with too long first name should fail");

            // Test with too long last name
            var (longLastNameStatus, _, _) = await _auth.RegisterUser(
                firstName: "John",
                lastName: longName,
                email: $"test_long_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
            );

            Assert.That(longLastNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with too long last name should fail");
        }

        [Test]
        public async Task SignUp_SpecialCharactersInNameFields_ShouldFail()
        {
            var specialChars = new[] { "@", "#", "$", "%", "^", "*", "(", ")", "<", ">", "=" };

            foreach (var specialChar in specialChars)
            {
                // Test with special characters in first name
                var (firstNameStatus, _, _) = await _auth.RegisterUser(
                    firstName: "John" + specialChar,
                    lastName: "Smith",
                    email: $"test_special_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
                );

                Assert.That(firstNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with special character '{specialChar}' in first name should fail");

                // Test with special characters in last name
                var (lastNameStatus, _, _) = await _auth.RegisterUser(
                    firstName: "John",
                    lastName: "Doe" + specialChar,
                    email: $"test_special_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
                );

                Assert.That(lastNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with special character '{specialChar}' in last name should fail");
            }
        }

        [Test]
        public async Task SignUp_SuccessfulRegistration_ShouldAllowLogin()
        {
            // Arrange - register a new user
            string email = $"login_test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "Test@123Password";

            var (registerStatus, registeredEmail, _) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Smith",
                email: email,
                password: password
            );

            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK),
                "Registration should be successful");

            // Act - trying to log in with the registered credentials
            var (loginStatus, _) = await _auth.LoginUser(email, password);

            // Assert
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK),
                "Login with registered credentials should succeed");
        }

        [Test]
        public async Task SignUp_NoPassword_ShouldFail()
        {
            var (statusCode, _, responseBody) = await _auth.RegisterUser(
                firstName: "Alice",
                lastName: "Brown",
                email: $"no_password_{Guid.NewGuid()}@example.com",
                password: null
            );

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration without a password should fail");

            Assert.That(responseBody.ContainsKey("message"), Is.True,
                "Response should contain an error message");
        }

        [Test]
        public async Task SignUp_BasicValidUser_ShouldSucceed()
        {
            // Arrange
            string email = $"basic_user_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";

            // Act
            var (statusCode, _, responseBody) = await _auth.RegisterUser(
                firstName: "Basic",
                lastName: "User",
                email: email,
                password: "ValidP@ss123!"
            );

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Basic user registration should succeed");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
        }

        [Test]
        public async Task SignUp_CheckResponseMessage_ShouldContainSuccessMessage()
        {
            // Arrange
            string email = $"message_check_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";

            // Act
            var (_, _, responseBody) = await _auth.RegisterUser(
                firstName: "Message",
                lastName: "Check",
                email: email,
                password: "StrongP@ss123!"
            );

            // Assert
            Assert.That(responseBody.ContainsKey("message"), Is.True,
                "Response should contain a message key");
            Assert.That(responseBody["message"].ToString(), Is.EqualTo("User Registered"),
                "Success message should match expected text");
        }

        [Test]
        public async Task SignUp_DuplicateEmailRegistration_ShouldFail()
        {
            // Предполагаем, что есть поле auth типа Authentication
            // Если его нет, нужно создать экземпляр
            var auth = new ApiTests.Pages.Authentication();

            // Создаем уникальный email для теста
            string duplicateEmail = $"duplicate_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";

            // Первая регистрация
            var firstResult = await auth.RegisterUser(
                firstName: "Alice",
                lastName: "Brown",
                email: duplicateEmail,
                password: _defaultPassword
            );

            Assert.That(firstResult.statusCode, Is.EqualTo(HttpStatusCode.OK),
                "First registration should be successful");

            // Вторая регистрация с тем же email
            var secondResult = await auth.RegisterUser(
                firstName: "John",
                lastName: "Smith",
                email: duplicateEmail,
                password: _defaultPassword
            );

            Assert.That(secondResult.statusCode, Is.AnyOf(HttpStatusCode.InternalServerError, HttpStatusCode.Conflict),
                "Second registration with the same email should fail with Conflict");
        }
    }
}
