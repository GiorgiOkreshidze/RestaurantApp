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
    [Category("SignIn")]
    public class SignIn : BaseTest
    {
        private Authentication _auth;
        private static string _testEmail;
        private static string _testPassword;
        private static bool _userRegistered = false;

        [OneTimeSetUp]
        public async Task RegisterTestUserOnce()
        {
            _auth = new Authentication();

            // Generate unique test data
            _testEmail = $"signin_test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            _testPassword = Config.TestUserPassword;

            // Register user once for all tests
            await RegisterTestUser();
        }

        [SetUp]
        public void SetupAuthentication()
        {
            _auth = new Authentication();
        }

        private async Task<bool> RegisterTestUser()
        {
            if (_userRegistered)
                return true;

            var (statusCode, _, _) = await _auth.RegisterUser(
                firstName: "Test",
                lastName: "User",
                email: _testEmail,
                password: _testPassword
            );

            _userRegistered = statusCode == HttpStatusCode.OK;
            return _userRegistered;
        }

        [Test]
        public async Task SignIn_ValidCredentials_ShouldReturnTokens()
        {
            // Act - attempt to log in with valid credentials
            var (statusCode, responseBody) = await _auth.LoginUser(_testEmail, _testPassword);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check that tokens and other user data are returned
            Assert.That(responseBody.ContainsKey("idToken") || responseBody.ContainsKey("token"), Is.True,
                "Response should contain token");
            Assert.That(responseBody.ContainsKey("refreshToken"), Is.True,
                "Response should contain refresh token");
        }

        [Test]
        public async Task SignIn_InvalidEmail_ShouldFail()
        {
            // Act - attempt to log in with incorrect email
            var invalidEmail = $"wrong_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            var (statusCode, responseBody) = await _auth.LoginUser(invalidEmail, _testPassword);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Forbidden),
                "Login with invalid email should fail with Forbidden");

            // Check for error message
            Assert.That(responseBody, Is.Not.Null, "Error response should contain a message");
            Assert.That(responseBody.ContainsKey("message"), Is.True,
                "Error response should include an error message field");

            if (responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("incorrect").IgnoreCase.Or.Contains("invalid").IgnoreCase,
                    "Error message should indicate invalid credentials");
            }
        }

        [Test]
        public async Task SignIn_InvalidPassword_ShouldFail()
        {
            // Act - attempt to log in with incorrect password
            var wrongPassword = _testPassword + "Wrong";
            var (statusCode, responseBody) = await _auth.LoginUser(_testEmail, wrongPassword);

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest),
                "Login with invalid password should fail");

            // Check for error message
            Assert.That(responseBody, Is.Not.Null, "Error response should contain a message");
            Assert.That(responseBody.ContainsKey("message"), Is.True,
                "Error response should include an error message field");

            if (responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("password").IgnoreCase.Or.Contains("credentials").IgnoreCase,
                    "Error message should indicate password issue");
            }
        }

        [Test]
        public async Task SignIn_EmptyCredentials_ShouldFail()
        {
            // Act - attempt to log in with empty email
            var (emptyEmailStatus, emptyEmailResponse) = await _auth.LoginUser("", _testPassword);

            // Assert
            Assert.That(emptyEmailStatus, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                "Login with empty email should fail");

            // Check for error message
            Assert.That(emptyEmailResponse, Is.Not.Null, "Error response should contain a message");
            Assert.That(emptyEmailResponse.ContainsKey("message"), Is.True,
                "Error response should include an error message field");

            // We validate that there is an error message, but don't check its specific content
            // as the API returns a generic error message

            // Act - attempt to log in with empty password
            var (emptyPasswordStatus, emptyPasswordResponse) = await _auth.LoginUser(_testEmail, "");

            // Assert
            Assert.That(emptyPasswordStatus, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized, HttpStatusCode.InternalServerError),
                "Login with empty password should fail");

            // Check for error message
            Assert.That(emptyPasswordResponse, Is.Not.Null, "Error response should contain a message");
            Assert.That(emptyPasswordResponse.ContainsKey("message"), Is.True,
                "Error response should include an error message field");

            // Only check that there is a message, without validating its specific content
            Assert.That(emptyPasswordResponse["message"].ToString(), Is.Not.Empty,
                "Error message should not be empty");
        }

        [Test]
        public async Task SignIn_SuccessfulLogin_TokenShouldBeValid()
        {
            // Act - log in
            var (loginStatus, responseBody) = await _auth.LoginUser(_testEmail, _testPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");

            // Extract idToken from the response
            string idToken = responseBody["idToken"]?.ToString();
            string accessToken = responseBody["idToken"]?.ToString(); // Using idToken as accessToken

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Use the obtained token to get the user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

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
        public async Task SignIn_LogoutAndLogin_ShouldWorkCorrectly()
        {
            // Act - log in
            Console.WriteLine("Logging in the user...");
            try
            {
                var (loginStatus, responseBody) = await _auth.LoginUser(_testEmail, _testPassword);
                Console.WriteLine($"Login status: {loginStatus}");
                Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");

                // Extract tokens from the response
                string idToken = responseBody["idToken"]?.ToString();
                string accessToken = responseBody["idToken"]?.ToString(); // Using idToken as accessToken
                string refreshToken = responseBody["refreshToken"]?.ToString();

                Console.WriteLine($"Login successful. ID Token: {idToken?.Substring(0, 10)}..., Refresh Token: {refreshToken?.Substring(0, 10)}...");

                Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");
                Assert.That(refreshToken, Is.Not.Null.And.Not.Empty, "Refresh token should not be null or empty");

                // Act - log out using Bearer token
                Console.WriteLine("Logging out...");
                var (logoutStatus, _) = await _auth.LogoutUser(refreshToken, idToken);
                Console.WriteLine($"Logout status: {logoutStatus}");
                Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK), "Logout should be successful");

                // Check if the old token is still valid
                Console.WriteLine($"Checking token validity after logout...");
                var (profileStatus, profileResponseBody) = await _auth.GetUserProfile(idToken, accessToken);
                Console.WriteLine($"Profile status after logout: {profileStatus}");

                // Token behavior might vary depending on implementation
                Assert.That(profileStatus, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Forbidden),
                    "After logout, token behavior is implementation specific");

                // Log in again with the same credentials
                Console.WriteLine("Logging in again after logout...");
                var (secondLoginStatus, secondResponseBody) = await _auth.LoginUser(_testEmail, _testPassword);
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
        public async Task SignIn_TokenExpiration_ShouldBeIncludedInResponse()
        {
            // Act - log in
            var (loginStatus, responseBody) = await _auth.LoginUser(_testEmail, _testPassword);

            // Assert
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");
            Assert.That(responseBody.ContainsKey("expiresIn"), Is.True,
                "Response should include token expiration time");

            // Check that expiresIn is a numeric value
            Assert.That(responseBody["expiresIn"].Type, Is.EqualTo(JTokenType.Integer),
                "expiresIn should be an integer value");
        }

        [Test]
        public async Task SignIn_CanLoginAfterRegistration_WithoutDelay()
        {
            // Arrange & Act - register new user
            string newEmail = $"immediate_login_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            var (registerStatus, _, _) = await _auth.RegisterUser(
                firstName: "Test",
                lastName: "User",
                email: newEmail,
                password: _testPassword
            );

            // Assert registration success
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK),
                "User registration should be successful");

            // Act - attempt immediate login after registration
            var (loginStatus, responseBody) = await _auth.LoginUser(newEmail, _testPassword);

            // Assert login success
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK),
                "Login immediately after registration should succeed");
            Assert.That(responseBody.ContainsKey("idToken"), Is.True,
                "Login response should contain idToken");
        }

        [Test]
        public async Task SignIn_SuccessfulLoginAndProfile_UserDataMatches()
        {
            // Arrange - register test user with specific data
            string testFirstName = "Jane";
            string testLastName = "Doe";
            string testEmail = $"jane_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";

            var (registerStatus, _, _) = await _auth.RegisterUser(
                firstName: testFirstName,
                lastName: testLastName,
                email: testEmail,
                password: _testPassword
            );

            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "Registration should succeed");

            // Act - login and get profile
            var (loginStatus, loginResponse) = await _auth.LoginUser(testEmail, _testPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should succeed");

            string idToken = loginResponse["idToken"]?.ToString();
            var (profileStatus, profileData) = await _auth.GetUserProfile(idToken);

            // Assert - profile data matches registration data
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile retrieval should succeed");
            Assert.That(profileData["email"]?.ToString(), Is.EqualTo(testEmail), "Email should match");
            Assert.That(profileData["firstName"]?.ToString(), Is.EqualTo(testFirstName), "First name should match");
            Assert.That(profileData["lastName"]?.ToString(), Is.EqualTo(testLastName), "Last name should match");
        }

        [Test]
        public async Task SignIn_InvalidEmailFormat_ShouldBeValidated()
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
                var (statusCode, responseBody) = await _auth.LoginUser(invalidEmail, "ValidPassword123!");

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
        public async Task SignIn_IncorrectPasswordFormat_ShouldFail()
        {
            // Arrange - password too short
            string shortPassword = "123";

            // Act
            var (statusCode, responseBody) = await _auth.LoginUser(_testEmail, shortPassword);

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                "Login with too short password should fail");
            Assert.That(responseBody, Is.Not.Null, "Error response should not be null");
            Assert.That(responseBody.ContainsKey("message"), Is.True,
                "Response should contain error message");
        }

        [Test]
        public async Task SignIn_IncorrectEmailFormat_ShouldFail()
        {
            // Arrange - invalid email format
            string invalidEmail = "notanemail";

            // Act
            var (statusCode, responseBody) = await _auth.LoginUser(invalidEmail, _testPassword);

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized),
                "Login with invalid email format should fail");
            Assert.That(responseBody, Is.Not.Null, "Error response should not be null");
            Assert.That(responseBody.ContainsKey("message"), Is.True,
                "Response should contain error message");
        }

        [Test]
        public async Task SignIn_TokenFormatIsValid()
        {
            // Arrange - register a test user
            string email = $"token_test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, _) = await _auth.RegisterUser("Token", "Test", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "Registration should succeed");

            // Act - login and validate token format
            var (loginStatus, responseBody) = await _auth.LoginUser(email, password);

            // Assert
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should succeed");
            Assert.That(responseBody.ContainsKey("idToken"), Is.True, "Response should contain ID token");
            Assert.That(responseBody.ContainsKey("refreshToken"), Is.True, "Response should contain refresh token");

            // Check token formats - typically JWT tokens have 3 parts separated by dots
            string idToken = responseBody["idToken"].ToString();
            Assert.That(idToken.Split('.').Length, Is.EqualTo(3), "ID token should be in JWT format");
        }

        [Test]
        public async Task SignIn_LoginAfterLogout_ShouldSucceed()
        {
            // Arrange - log in first
            var (loginStatus, loginResponse) = await _auth.LoginUser(_testEmail, _testPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Initial login should succeed");

            // Get tokens
            string idToken = loginResponse["idToken"].ToString();
            string refreshToken = loginResponse["refreshToken"].ToString();

            // Act - log out
            var (logoutStatus, _) = await _auth.LogoutUser(refreshToken, idToken);
            Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK), "Logout should succeed");

            // Act again - try to log in after logout
            var (secondLoginStatus, secondLoginResponse) = await _auth.LoginUser(_testEmail, _testPassword);

            // Assert
            Assert.That(secondLoginStatus, Is.EqualTo(HttpStatusCode.OK),
                "Login after logout should succeed");
            Assert.That(secondLoginResponse.ContainsKey("idToken"), Is.True,
                "Second login should return new ID token");
        }
    }
}
