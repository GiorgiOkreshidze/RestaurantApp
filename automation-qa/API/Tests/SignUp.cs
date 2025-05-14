using System;
using System.Net;
using NUnit.Framework;
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
        public void SignUp_CompleteValidData_ShouldCreateUser()
        {
            // Act
            var (statusCode, email, responseBody) = _auth.RegisterUserWithCurl(
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
            Assert.That(responseBody["message"].ToString(),
                Contains.Substring("registered").IgnoreCase,
                "Response should indicate successful registration");
        }

        [Test]
        public void SignUp_DuplicateEmail_ShouldFail()
        {
            // Arrange
            var email = $"duplicate_{Guid.NewGuid()}@example.com";

            // First registration
            var (firstStatusCode, _, firstResponseBody) = _auth.RegisterUserWithCurl(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: Config.TestUserPassword
            );

            Assert.That(firstStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "First registration should be successful");
            System.Threading.Thread.Sleep(2000);

            // Repeat registration
            var (secondStatusCode, _, secondResponseBody) = _auth.RegisterUserWithCurl(
                firstName: "John",
                lastName: "Doe",
                email: email,
                password: Config.TestUserPassword
            );

            Assert.That(secondStatusCode, Is.EqualTo(HttpStatusCode.BadRequest)
                .Or.EqualTo(HttpStatusCode.Conflict),
                "Repeated registration should return BadRequest or Conflict");

            // Verify error message
            if (secondResponseBody != null && secondResponseBody.ContainsKey("title"))
            {
                Assert.That(secondResponseBody["title"].ToString(),
                    Contains.Substring("already exists").IgnoreCase,
                    "Error message should indicate email already exists");
            }
        }

        [Test]
        public void SignUp_InvalidEmailFormat_ShouldFail()
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
                var (statusCode, _, responseBody) = _auth.RegisterUserWithCurl(
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
        public void SignUp_WeakPassword_ShouldFail()
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
                var (statusCode, _, responseBody) = _auth.RegisterUserWithCurl(
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
        public void SignUp_PasswordBoundaryValues_ShouldSucceed()
        {
            // Test minimum length password (8 characters)
            var (minStatusCode, _, minResponseBody) = _auth.RegisterUserWithCurl(
                firstName: "John",
                lastName: "Smith",
                email: $"test_min_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: "Aa1@5678" // Exactly 8 characters
            );

            Assert.That(minStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration with 8-character password should succeed");

            if (minResponseBody != null && minResponseBody.ContainsKey("message"))
            {
                Assert.That(minResponseBody["message"].ToString(),
                    Does.Contain("registered successfully").IgnoreCase,
                    "Response should indicate successful registration");
            }

            // Test maximum length password (16 characters)
            var (maxStatusCode, _, maxResponseBody) = _auth.RegisterUserWithCurl(
                firstName: "Jane",
                lastName: "Doe",
                email: $"test_max_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: "Aa1@567890123456" // Exactly 16 characters
            );

            Assert.That(maxStatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration with 16-character password should succeed");

            if (maxResponseBody != null && maxResponseBody.ContainsKey("message"))
            {
                Assert.That(maxResponseBody["message"].ToString(),
                    Does.Contain("registered successfully").IgnoreCase,
                    "Response should indicate successful registration");
            }
        }


        [Test]
        public void SignUp_EmptyNameFields_ShouldFail()
        {
            // Test with empty first name
            var (emptyFirstNameStatus, _, firstNameResponseBody) = _auth.RegisterUserWithCurl(
                firstName: "",
                lastName: "Smith",
                email: $"test_empty_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: Config.TestUserPassword
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
            var (emptyLastNameStatus, _, lastNameResponseBody) = _auth.RegisterUserWithCurl(
                firstName: "John",
                lastName: "",
                email: $"test_empty_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: Config.TestUserPassword
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
        public void SignUp_TooLongNameFields_ShouldFail()
        {
            var longName = new string('A', 256); // 256 characters

            // Test with too long first name
            var (longFirstNameStatus, _, firstNameResponseBody) = _auth.RegisterUserWithCurl(
                firstName: longName,
                lastName: "Smith",
                email: $"test_long_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: Config.TestUserPassword
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
            var (longLastNameStatus, _, lastNameResponseBody) = _auth.RegisterUserWithCurl(
                firstName: "John",
                lastName: longName,
                email: $"test_long_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                password: Config.TestUserPassword
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
        public void SignUp_SpecialCharactersInNameFields_ShouldFail()
        {
            var specialChars = new[] { "@", "#", "$", "%", "^", "*", "(", ")", "<", ">", "=" };

            foreach (var specialChar in specialChars)
            {
                // Test with special characters in first name
                var (firstNameStatus, _, firstNameResponseBody) = _auth.RegisterUserWithCurl(
                    firstName: "John" + specialChar,
                    lastName: "Smith",
                    email: $"test_special_first_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                    password: Config.TestUserPassword
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
                var (lastNameStatus, _, lastNameResponseBody) = _auth.RegisterUserWithCurl(
                    firstName: "John",
                    lastName: "Doe" + specialChar,
                    email: $"test_special_last_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                    password: Config.TestUserPassword
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
        public void SignUp_SuccessfulRegistration_ShouldAllowLogin()
        {
            // Arrange - register a new user
            string email = $"login_test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "Test@123Password";

            var (registerStatus, registeredEmail, registerResponseBody) = _auth.RegisterUserWithCurl(
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
                Assert.That(registerResponseBody["message"].ToString(),
                    Contains.Substring("registered").IgnoreCase,
                    "Registration response should indicate successful registration");
            }

            // Act - trying to log in with the registered credentials
            var (loginStatus, loginResponseBody) = _auth.LoginUserWithCurl(email, password);

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
        public void SignUp_NoPassword_ShouldFail()
        {
            var (statusCode, _, responseBody) = _auth.RegisterUserWithCurl(
                firstName: "Alice",
                lastName: "Brown",
                email: $"no_password_{Guid.NewGuid()}@example.com",
                password: null
            );

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration without a password should fail");

            Assert.That(responseBody.ContainsKey("errors"), Is.True,
                "Response should contain a validation errors object");

            var errors = responseBody["errors"] as JObject;
            Assert.That(errors, Is.Not.Null, "Errors should be a valid JSON object");

            Assert.That(errors.ContainsKey("Password"), Is.True,
                "Errors should include 'Password' field");

            var passwordErrors = errors["Password"]?.ToString();
            Assert.That(passwordErrors, Does.Contain("required").IgnoreCase,
                "Password error message should mention it is required");
        }


        [Test]
        public void SignUp_BasicValidUser_ShouldSucceed()
        {
            // Arrange
            string email = $"basic_user_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";

            // Act
            var (statusCode, _, responseBody) = _auth.RegisterUserWithCurl(
                firstName: "Basic",
                lastName: "User",
                email: email,
                password: "ValidP@ss123!"
            );

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Basic user registration should succeed");

            if (statusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error response body: {responseBody}");
            }

            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Verify success message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("registered").IgnoreCase,
                    "Response should indicate successful registration");
            }
        }


        [Test]
        public void SignUp_WeakPasswords_ComplexTest_ShouldFail()
        {
            string[] weakPasswords = new[]
            {
        "weak",           // too short
        "123456",         // only digits
        "password",       // common password
        "weakpassword",   // no complexity
        "12345678",       // only digits
        new string('a', 257) // exceeds max length
    };

            foreach (var weakPassword in weakPasswords)
            {
                var (statusCode, _, responseBody) = _auth.RegisterUserWithCurl(
                    firstName: "Weak",
                    lastName: "Password",
                    email: $"weak_{Guid.NewGuid()}@example.com",
                    password: weakPassword
                );

                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration should fail for weak password: {weakPassword}");

                Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

                Assert.That(responseBody.ContainsKey("errors"), Is.True, "Response should contain 'errors'");
                var errors = responseBody["errors"] as Newtonsoft.Json.Linq.JObject;

                Assert.That(errors.ContainsKey("Password"), Is.True,
                    $"Response should include a Password error for password: {weakPassword}");
            }
        }

        [Test]
        [Category("Smoke")]
        public void SignIn_ValidCredentials_ShouldSucceed()
        {
            // Act
            var (statusCode, responseBody) = _auth.LoginUserWithCurl(
                "irishkakhrol@gmail.com",
                Config.TestUserPassword
            );

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Login with valid credentials should succeed");
            Assert.That(responseBody.ContainsKey("accessToken"), Is.True,
                "Response should contain access token");
        }

        [Test]
        [Category("Regression")]
        public void SignIn_InvalidEmail_ShouldFail()
        {
            // Act
            var invalidEmail = "notanemail";
            var (statusCode, responseBody) = _auth.LoginUserWithCurl(
                invalidEmail,
                Config.TestUserPassword
            );

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                "Login with invalid email format should fail");
            Assert.That(responseBody, Is.Not.Null,
                "Error response should not be null");

            if (responseBody.ContainsKey("errors"))
            {
                Assert.That(responseBody["errors"].ToString().ToLower(),
                    Contains.Substring("email"),
                    "Error message should mention email issue");
            }
        }


        [Test]
        public void SignUp_CheckResponseMessage_ShouldContainSuccessMessage()
        {
            // Arrange
            string email = $"message_check_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";

            // Act
            var (statusCode, _, responseBody) = _auth.RegisterUserWithCurl(
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
            Assert.That(responseBody["message"].ToString(),
                Contains.Substring("registered").IgnoreCase,
                "Response should indicate successful registration");
        }
    }
}
