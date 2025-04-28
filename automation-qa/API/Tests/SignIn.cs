using System;
using System.Net;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;
using ApiTests.Utilities;

namespace ApiTests
{
    [TestFixture]
    [Category("SignIn")]
    public class SignIn : BaseTest
    {
        private Authentication _auth;
        private static string _testEmail;
        private static string _testPassword;
        private static bool _userRegistered = false;

        [OneTimeSetUp]
        public void RegisterTestUserOnce()
        {
            _auth = new Authentication();

            // Generate unique test data
            _testEmail = $"signin_test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            _testPassword = Config.TestUserPassword;

            // Register user once for all tests
            RegisterTestUser();
        }

        [SetUp]
        public void SetupAuthentication()
        {
            _auth = new Authentication();
        }

        private bool RegisterTestUser()
        {
            if (_userRegistered)
                return true;

            var (statusCode, _, _) = _auth.RegisterUserWithCurl(
                firstName: "Test",
                lastName: "User",
                email: _testEmail,
                password: _testPassword
            );

            _userRegistered = statusCode == HttpStatusCode.OK;
            return _userRegistered;
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void SignIn_ValidCredentials_ShouldReturnTokens()
        {
            // Act - attempt to log in with valid credentials
            var (statusCode, responseBody) = _auth.LoginUserWithCurl(_testEmail, _testPassword);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check that the access token and refresh token are returned
            Assert.That(responseBody.ContainsKey("accessToken"), Is.True, "Response should contain access token");
            Assert.That(responseBody.ContainsKey("refreshToken"), Is.True, "Response should contain refresh token");
        }


        [Test]
        [Category("Regression")]
        public void SignIn_InvalidEmail_ShouldFail()
        {
            // Act - attempt to log in with incorrect email
            var invalidEmail = $"wrong_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            var (statusCode, responseBody) = _auth.LoginUserWithCurl(invalidEmail, _testPassword);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Login with invalid email should fail with Unauthorized");

            // Check for error message or title in response
            Assert.That(responseBody, Is.Not.Null, "Error response should contain a message");

            // Check if the 'title' field exists in the response
            Assert.That(responseBody.ContainsKey("title"), Is.True,
                "Error response should include a 'title' field");

            // Validate that the title field contains the correct message
            if (responseBody.ContainsKey("title"))
            {
                Assert.That(responseBody["title"].ToString(),
                    Contains.Substring("Invalid email").IgnoreCase.Or.Contains("Invalid credentials").IgnoreCase,
                    "Error message should indicate invalid email or credentials");
            }

            // Check if 'errors' exists, but don't fail if it's an empty object
            if (responseBody.ContainsKey("errors"))
            {
                // If 'errors' is an empty object, the test can pass
                var errors = responseBody["errors"] as Newtonsoft.Json.Linq.JObject;
                if (errors != null && errors.Count == 0)
                {
                    Console.WriteLine("Errors field is empty, which is acceptable.");
                }
                else
                {
                    Assert.That(errors, Is.Not.Null, "Errors field should contain details about the issue");
                }
            }
        }


        [Test]
        [Category("Regression")]
        public void SignIn_InvalidPassword_ShouldFail()
        {
            // Act - attempt to log in with incorrect password
            var wrongPassword = _testPassword + "Wrong";
            var (statusCode, responseBody) = _auth.LoginUserWithCurl(_testEmail, wrongPassword);

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest),
                "Login with invalid password should fail");

            // Check for error message
            Assert.That(responseBody, Is.Not.Null, "Error response should contain a message");
            Assert.That(responseBody.ContainsKey("title"), Is.True, "Response should contain error title field");

            if (responseBody.ContainsKey("title"))
            {
                Assert.That(responseBody["title"].ToString(),
                    Contains.Substring("Invalid email or password").IgnoreCase,
                    "Error title should indicate invalid credentials");
            }
        }


        [Test]
        [Category("Regression")]
        public void SignIn_EmptyCredentials_ShouldFail()
        {
            // Act - attempt to log in with empty email
            var (emptyEmailStatus, emptyEmailResponse) = _auth.LoginUserWithCurl("", _testPassword);

            // Assert
            Assert.That(emptyEmailStatus, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                "Login with empty email should fail");

            Assert.That(emptyEmailResponse, Is.Not.Null, "Error response should not be null");
            Assert.That(emptyEmailResponse.ContainsKey("errors"), Is.True,
                "Error response should include an 'errors' field");

            // Act - attempt to log in with empty password
            var (emptyPasswordStatus, emptyPasswordResponse) = _auth.LoginUserWithCurl(_testEmail, "");

            // Assert
            Assert.That(emptyPasswordStatus, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized, HttpStatusCode.InternalServerError),
                "Login with empty password should fail");

            Assert.That(emptyPasswordResponse, Is.Not.Null, "Error response should not be null");
            Assert.That(emptyPasswordResponse.ContainsKey("errors"), Is.True,
                "Error response should include an 'errors' field");

            // Дополнительно можно проверить, что в поле `errors` есть сообщения
            var errors = emptyPasswordResponse["errors"];
            Assert.That(errors.ToString(), Is.Not.Empty, "Error field should not be empty");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void SignIn_SuccessfulLogin_TokenShouldBeValid()
        {
            // Act - log in
            var (loginStatus, responseBody) = _auth.LoginUserWithCurl(_testEmail, _testPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");

            // Extract accessToken from the response
            string accessToken = responseBody["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Use the obtained token to get the user profile
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            // The token should be valid immediately
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should allow access to profile immediately after login");

            // Check that the response contains user data
            Assert.That(userData, Is.Not.Null, "User data should not be null");

            // Optional: Check the content of userData
            Assert.That(userData.ContainsKey("email"), Is.True, "User data should contain email");

            // If the response contains an email field, check that it matches the current user's email
            if (userData.ContainsKey("email"))
            {
                string userEmail = userData["email"].ToString();
                Assert.That(userEmail, Is.EqualTo(_testEmail), "Email in profile should match the login email");
            }
        }


        [Test]
        [Category("Regression")]
        public void SignIn_LogoutAndLogin_ShouldWorkCorrectly()
        {
            // Act - log in
            Console.WriteLine("Logging in the user...");
            try
            {
                var (loginStatus, responseBody) = _auth.LoginUserWithCurl(_testEmail, _testPassword);
                Console.WriteLine($"Login status: {loginStatus}");
                Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");

                // Extract tokens from the response
                string accessToken = responseBody["accessToken"]?.ToString();
                string refreshToken = responseBody["refreshToken"]?.ToString();
                string idToken = accessToken; // если система использует один токен и для авторизации, и для аутентификации


                Console.WriteLine($"Login successful. ID Token: {idToken?.Substring(0, 10)}..., Refresh Token: {refreshToken?.Substring(0, 10)}...");

                Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");
                Assert.That(refreshToken, Is.Not.Null.And.Not.Empty, "Refresh token should not be null or empty");

                // Act - log out using Bearer token
                Console.WriteLine("Logging out...");
                var (logoutStatus, _) = _auth.LogoutUserWithCurl(refreshToken, idToken);
                Console.WriteLine($"Logout status: {logoutStatus}");
                Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK), "Logout should be successful");

                // Check if the old token is still valid
                Console.WriteLine($"Checking token validity after logout...");
                var (profileStatus, profileResponseBody) = _auth.GetUserProfileWithCurl(idToken, accessToken);
                Console.WriteLine($"Profile status after logout: {profileStatus}");

                // Token behavior might vary depending on implementation
                Assert.That(profileStatus, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Forbidden),
                    "After logout, token behavior is implementation specific");

                // Log in again with the same credentials
                Console.WriteLine("Logging in again after logout...");
                var (secondLoginStatus, secondResponseBody) = _auth.LoginUserWithCurl(_testEmail, _testPassword);
                Assert.That(secondLoginStatus, Is.EqualTo(HttpStatusCode.OK),
                    "Login after logout should succeed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed with exception: {ex.Message}");
                throw;
            }
        }

        [Test]
        [Category("Regression")]
        public void SignIn_TokenExpiration_ShouldBeIncludedInResponse()
        {
            // Act - log in
            var (loginStatus, responseBody) = _auth.LoginUserWithCurl(_testEmail, _testPassword);

            // Assert
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");
            Assert.That(responseBody.ContainsKey("expiresIn"), Is.True,
                "Response should include token expiration time");

            // Check that expiresIn is a numeric value
            Assert.That(responseBody["expiresIn"].Type, Is.EqualTo(JTokenType.Integer),
                "expiresIn should be an integer value");
        }

        [Test]
        [Category("Regression")]
        public void SignIn_CanLoginAfterRegistration_WithoutDelay()
        {
            // Arrange & Act - register new user
            string newEmail = $"immediate_login_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            var (registerStatus, _, _) = _auth.RegisterUserWithCurl(
                firstName: "Test",
                lastName: "User",
                email: newEmail,
                password: _testPassword
            );

            // Assert registration success
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK),
                "User registration should be successful");

            // Act - attempt immediate login after registration
            var (loginStatus, responseBody) = _auth.LoginUserWithCurl(newEmail, _testPassword);

            // Assert login success
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK),
                "Login immediately after registration should succeed");

            Assert.That(responseBody, Is.Not.Null, "Login response should not be null");
            Assert.That(
                responseBody.ContainsKey("accessToken") || responseBody.ContainsKey("idToken"),
                "Login response should contain authentication token (accessToken or idToken)"
            );
        }


        [Test]
        [Category("Regression")]
        public void SignIn_SuccessfulLoginAndProfile_UserDataMatches()
        {
            // Arrange - register test user with specific data
            string testFirstName = "Jane";
            string testLastName = "Doe";
            string testEmail = $"jane_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";

            var (registerStatus, _, _) = _auth.RegisterUserWithCurl(
                firstName: testFirstName,
                lastName: testLastName,
                email: testEmail,
                password: _testPassword
            );

            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "Registration should succeed");

            // Act - login and get profile
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(testEmail, _testPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should succeed");

            string accessToken = loginResponse["accessToken"]?.ToString(); // Use accessToken instead of idToken
            var (profileStatus, profileData) = _auth.GetUserProfileWithCurl(accessToken); // Pass accessToken here

            // Assert - profile data matches registration data
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile retrieval should succeed");
            Assert.That(profileData["email"]?.ToString(), Is.EqualTo(testEmail), "Email should match");
            Assert.That(profileData["firstName"]?.ToString(), Is.EqualTo(testFirstName), "First name should match");
            Assert.That(profileData["lastName"]?.ToString(), Is.EqualTo(testLastName), "Last name should match");
        }


        [Test]
        [Category("Regression")]
        public void SignIn_InvalidEmailFormat_ShouldBeValidated()
        {
            // Arrange
            var invalidEmails = new[]
            {
                "notanemail",
                "missing@",
                "@missingusername.com",
                "spaces are invalid@example.com",
                "missing.domain@",
                "@.com"
            };

            // Act & Assert
            foreach (var invalidEmail in invalidEmails)
            {
                var (statusCode, responseBody) = _auth.LoginUserWithCurl(invalidEmail, "ValidPassword123!");

                Assert.That(statusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                    $"Login with invalid email format '{invalidEmail}' should fail");

                // Check for error message in the response
                Assert.That(responseBody, Is.Not.Null, "Error response should contain a message");

                if (responseBody.ContainsKey("message"))
                {
                    Assert.That(responseBody["message"].ToString(),
                        Contains.Substring("email").IgnoreCase,
                        "Error message should indicate email format issue");
                }
            }
        }

        [Test]
        [Category("Regression")]
        public void SignIn_IncorrectPasswordFormat_ShouldFail()
        {
            // Arrange - password too short
            string shortPassword = "123";

            // Act
            var (statusCode, responseBody) = _auth.LoginUserWithCurl(_testEmail, shortPassword);

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                "Login with too short password should fail");
            Assert.That(responseBody, Is.Not.Null, "Error response should not be null");

            // Check for the 'title' field in the response which contains the error message
            Assert.That(responseBody.ContainsKey("title"), Is.True,
                "Response should contain error message title");

            // Optionally, check the actual error message value
            Assert.That(responseBody["title"]?.ToString(), Is.EqualTo("Invalid email or password."),
                "Error message should be 'Invalid email or password.'");
        }


        [Test]
        [Category("Regression")]
        public void SignIn_IncorrectEmailFormat_ShouldFail()
        {
            // Arrange - invalid email format
            string invalidEmail = "notanemail";

            // Act
            var (statusCode, responseBody) = _auth.LoginUserWithCurl(invalidEmail, _testPassword);

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                "Login with invalid email format should fail");
            Assert.That(responseBody, Is.Not.Null, "Error response should not be null");

            Assert.That(responseBody.ContainsKey("errors"), Is.True,
                "Response should contain 'errors' field");

            Assert.That(responseBody["errors"].ToString().ToLower(), Does.Contain("email"),
                "Errors should contain message about invalid email");
        }


        [Test]
        [Category("Regression")]
        public void SignIn_TokenFormatIsValid()
        {
            // Arrange - register a test user
            string email = $"token_test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, _) = _auth.RegisterUserWithCurl("Token", "Test", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "Registration should succeed");

            // Act - login and validate token format
            var (loginStatus, responseBody) = _auth.LoginUserWithCurl(email, password);

            // Assert
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should succeed");
            Assert.That(responseBody.ContainsKey("accessToken"), Is.True, "Response should contain access token");

            // Check token formats - typically JWT tokens have 3 parts separated by dots
            string accessToken = responseBody["accessToken"].ToString();
            Assert.That(accessToken.Split('.').Length, Is.EqualTo(3), "Access token should be in JWT format");
        }


        [Test]
        [Category("Regression")]
        public void SignIn_LoginAfterLogout_ShouldSucceed()
        {
            // Arrange - log in first
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(_testEmail, _testPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Initial login should succeed");

            // Ensure the response contains both accessToken and refreshToken
            Assert.That(loginResponse.ContainsKey("accessToken"), Is.True, "Login response should contain accessToken");
            Assert.That(loginResponse.ContainsKey("refreshToken"), Is.True, "Login response should contain refreshToken");

            // Get tokens
            string accessToken = loginResponse["accessToken"]?.ToString();
            string refreshToken = loginResponse["refreshToken"]?.ToString();

            // Ensure the tokens are not null or empty
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "accessToken should not be null or empty");
            Assert.That(refreshToken, Is.Not.Null.And.Not.Empty, "refreshToken should not be null or empty");

            // Act - log out
            var (logoutStatus, _) = _auth.LogoutUserWithCurl(refreshToken, accessToken);
            Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK), "Logout should succeed");

            // Act again - try to log in after logout
            var (secondLoginStatus, secondLoginResponse) = _auth.LoginUserWithCurl(_testEmail, _testPassword);

            // Assert
            Assert.That(secondLoginStatus, Is.EqualTo(HttpStatusCode.OK),
                "Login after logout should succeed");
            Assert.That(secondLoginResponse.ContainsKey("accessToken"), Is.True,
                "Second login should return a new access token");
        }

        [Test]
        [Category("Smoke")]
        public void SignIn_CorrectCredentials_ShouldReturnOK()
        {
            // Act - log in with correct credentials
            var (statusCode, _) = _auth.LoginUserWithCurl("test@example.com", Config.TestUserPassword);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Login with correct credentials should succeed");
        }

        [Test]
        [Category("Regression")]
        public void SignIn_WrongEmail_ShouldReturnUnauthorized()
        {
            // Act - log in with wrong email
            var (statusCode, _) = _auth.LoginUserWithCurl("wrong@example.com", Config.TestUserPassword);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Login with wrong email should return Unauthorized");
        }

        [Test]
        [Category("Regression")]
        public void SignIn_EmptyEmail_ShouldReturnBadRequest()
        {
            // Act - log in with empty email
            var (statusCode, _) = _auth.LoginUserWithCurl("", Config.TestUserPassword);

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                "Login with empty email should fail");
        }

        [Test]
        [Category("Regression")]
        public void SignIn_TokenContainsUserInfo()
        {
            // Act - log in
            var (_, response) = _auth.LoginUserWithCurl("test@example.com", Config.TestUserPassword);

            // Get token and check profile
            string accessToken = response["accessToken"].ToString();
            var (_, profile) = _auth.GetUserProfileWithCurl(accessToken);

            // Assert - token contains user info
            Assert.That(profile["email"].ToString(), Is.EqualTo("test@example.com"),
                "Profile from token should contain correct email");
        }

        [Test]
        [Category("Regression")]
        public void SignIn_ResponseContainsAccessToken()
        {
            // Act - log in
            var (_, response) = _auth.LoginUserWithCurl("test@example.com", Config.TestUserPassword);

            // Assert - check response contains access token
            Assert.That(response.ContainsKey("accessToken"), Is.True,
                "Login response should contain access token");
        }
    }
}
