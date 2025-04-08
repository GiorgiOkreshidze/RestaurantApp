using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;
using ApiTests.Utilities;

namespace ApiTests
{
    [TestFixture]
    [Category("SignUp")]
    public class SignUp : BaseTest
    {
        private Authentication _auth;

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

            // Check success message
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
            var (firstStatusCode, _, firstResponseBody) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: Config.TestUserPassword
            );

            Assert.That(firstStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "First registration should be successful");
            await Task.Delay(2000);

            // Repeat registration
            var (secondStatusCode, _, secondResponseBody) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: Config.TestUserPassword
            );

            Assert.That(secondStatusCode, Is.EqualTo(HttpStatusCode.Conflict),
                 "Repeated registration should be prohibited (Conflict expected)");

            // Verify error message
            if (secondResponseBody != null && secondResponseBody.ContainsKey("message"))
            {
                Assert.That(secondResponseBody["message"].ToString(),
                    Contains.Substring("already exists").IgnoreCase,
                    "Error message should indicate email already exists");
            }
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
                var (statusCode, _, responseBody) = await _auth.RegisterUser(
                    firstName: "John",
                    lastName: "Smith",
                    email: invalidEmail,
                    password: Config.TestUserPassword
                );

                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with invalid email '{invalidEmail}' should fail");

                // Verify error message
                if (responseBody != null && responseBody.ContainsKey("message"))
                {
                    Assert.That(responseBody["message"].ToString(),
                        Contains.Substring("email").IgnoreCase,
                        "Error message should indicate issue with email format");
                }
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
                var (statusCode, _, responseBody) = await _auth.RegisterUser(
                    firstName: "John",
                    lastName: "Smith",
                    email: $"test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                    password: weakPassword
                );

                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with weak password '{weakPassword}' should fail");

                // Verify error message
                if (responseBody != null && responseBody.ContainsKey("message"))
                {
                    Assert.That(responseBody["message"].ToString(),
                        Contains.Substring("password").IgnoreCase,
                        "Error message should indicate issue with password strength");
                }
            }
        }

        [Test]
        public async Task SignUp_PasswordBoundaryValues_ShouldSucceed()
        {
            // Test minimum length password (8 characters)
            var (minStatusCode, _, minResponseBody) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Smith",
                email: $"test_min_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: "Aa1@5678" // Exactly 8 characters
            );

            Assert.That(minStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration with 8-character password should succeed");

            // Verify success message
            if (minResponseBody != null && minResponseBody.ContainsKey("message"))
            {
                Assert.That(minResponseBody["message"].ToString(), Is.EqualTo("User Registered"),
                    "Response should indicate successful registration");
            }

            // Test maximum length password (16 characters)
            var (maxStatusCode, _, maxResponseBody) = await _auth.RegisterUser(
                firstName: "Jane",
                lastName: "Doe",
                email: $"test_max_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: "Aa1@567890123456" // Exactly 16 characters
            );

            Assert.That(maxStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration with 16-character password should succeed");

            // Verify success message
            if (maxResponseBody != null && maxResponseBody.ContainsKey("message"))
            {
                Assert.That(maxResponseBody["message"].ToString(), Is.EqualTo("User Registered"),
                    "Response should indicate successful registration");
            }
        }

        [Test]
        public async Task SignUp_EmptyNameFields_ShouldFail()
        {
            // Test with empty first name
            var (emptyFirstNameStatus, _, firstNameResponseBody) = await _auth.RegisterUser(
                firstName: "",
                lastName: "Smith",
                email: $"test_empty_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
            );

            Assert.That(emptyFirstNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with empty first name should fail");

            // Verify error message
            if (firstNameResponseBody != null && firstNameResponseBody.ContainsKey("message"))
            {
                Assert.That(firstNameResponseBody["message"].ToString(),
                    Contains.Substring("first name").IgnoreCase,
                    "Error message should indicate issue with first name");
            }

            // Test with empty last name
            var (emptyLastNameStatus, _, lastNameResponseBody) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "",
                email: $"test_empty_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
            );

            Assert.That(emptyLastNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with empty last name should fail");

            // Verify error message
            if (lastNameResponseBody != null && lastNameResponseBody.ContainsKey("message"))
            {
                Assert.That(lastNameResponseBody["message"].ToString(),
                    Contains.Substring("last name").IgnoreCase,
                    "Error message should indicate issue with last name");
            }
        }

        [Test]
        public async Task SignUp_TooLongNameFields_ShouldFail()
        {
            var longName = new string('A', 256); // 256 characters

            // Test with too long first name
            var (longFirstNameStatus, _, firstNameResponseBody) = await _auth.RegisterUser(
                firstName: longName,
                lastName: "Smith",
                email: $"test_long_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
            );

            Assert.That(longFirstNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with too long first name should fail");

            // Verify error message
            if (firstNameResponseBody != null && firstNameResponseBody.ContainsKey("message"))
            {
                Assert.That(firstNameResponseBody["message"].ToString(),
                    Contains.Substring("first name").IgnoreCase,
                    "Error message should indicate issue with first name length");
            }

            // Test with too long last name
            var (longLastNameStatus, _, lastNameResponseBody) = await _auth.RegisterUser(
                firstName: "John",
                lastName: longName,
                email: $"test_long_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
            );

            Assert.That(longLastNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with too long last name should fail");

            // Verify error message
            if (lastNameResponseBody != null && lastNameResponseBody.ContainsKey("message"))
            {
                Assert.That(lastNameResponseBody["message"].ToString(),
                    Contains.Substring("last name").IgnoreCase,
                    "Error message should indicate issue with last name length");
            }
        }

        [Test]
        public async Task SignUp_SpecialCharactersInNameFields_ShouldFail()
        {
            var specialChars = new[] { "@", "#", "$", "%", "^", "*", "(", ")", "<", ">", "=" };

            foreach (var specialChar in specialChars)
            {
                // Test with special characters in first name
                var (firstNameStatus, _, firstNameResponseBody) = await _auth.RegisterUser(
                    firstName: "John" + specialChar,
                    lastName: "Smith",
                    email: $"test_special_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
                );

                Assert.That(firstNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with special character '{specialChar}' in first name should fail");

                // Verify error message
                if (firstNameResponseBody != null && firstNameResponseBody.ContainsKey("message"))
                {
                    Assert.That(firstNameResponseBody["message"].ToString(),
                        Contains.Substring("first name").IgnoreCase,
                        "Error message should indicate issue with first name");
                }

                // Test with special characters in last name
                var (lastNameStatus, _, lastNameResponseBody) = await _auth.RegisterUser(
                    firstName: "John",
                    lastName: "Doe" + specialChar,
                    email: $"test_special_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com"
                );

                Assert.That(lastNameStatus, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with special character '{specialChar}' in last name should fail");

                // Verify error message
                if (lastNameResponseBody != null && lastNameResponseBody.ContainsKey("message"))
                {
                    Assert.That(lastNameResponseBody["message"].ToString(),
                        Contains.Substring("last name").IgnoreCase,
                        "Error message should indicate issue with last name");
                }
            }
        }

        [Test]
        public async Task SignUp_SuccessfulRegistration_ShouldAllowLogin()
        {
            // Arrange - register a new user
            string email = $"login_test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "Test@123Password";

            var (registerStatus, registeredEmail, registerResponseBody) = await _auth.RegisterUser(
                firstName: "John",
                lastName: "Smith",
                email: email,
                password: password
            );

            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK),
                "Registration should be successful");

            // Verify registration success message
            if (registerResponseBody != null && registerResponseBody.ContainsKey("message"))
            {
                Assert.That(registerResponseBody["message"].ToString(), Is.EqualTo("User Registered"),
                    "Registration response should indicate successful registration");
            }

            // Act - trying to log in with the registered credentials
            var (loginStatus, loginResponseBody) = await _auth.LoginUser(email, password);

            // Assert
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK),
                "Login with registered credentials should succeed");

            // Verify login contains tokens
            if (loginResponseBody != null)
            {
                Assert.That(loginResponseBody.ContainsKey("idToken") ||
                            loginResponseBody.ContainsKey("accessToken"),
                    "Login response should contain authentication token");
            }
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

            Assert.That(responseBody["message"].ToString(),
                Contains.Substring("password").IgnoreCase,
                "Error message should indicate issue with password");
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

            // Verify success message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(), Is.EqualTo("User Registered"),
                    "Response should indicate successful registration");
            }
        }

        [Test]
        public async Task SignUp_CheckResponseMessage_ShouldContainSuccessMessage()
        {
            // Arrange
            string email = $"message_check_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";

            // Act
            var (statusCode, _, responseBody) = await _auth.RegisterUser(
                firstName: "Message",
                lastName: "Check",
                email: email,
                password: Config.TestUserPassword
            );

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration should be successful");
            Assert.That(responseBody.ContainsKey("message"), Is.True,
                "Response should contain a message key");
            Assert.That(responseBody["message"].ToString(), Is.EqualTo("User Registered"),
                "Success message should match expected text");
        }
    }
}
