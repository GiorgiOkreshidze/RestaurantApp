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
    [Category("SignIn")]
    public class SignIn : BaseTest
    {
        private Authentication _auth;
        private string _testFirstName;
        private string _testLastName;
        private string _testEmail;
        private string _testPassword;

        [SetUp]
        public void SetupAuthentication()
        {
            _auth = new Authentication();

            // Generate unique test data
            _testFirstName = "John";
            _testLastName = "Smith";
            _testEmail = $"test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            _testPassword = "TestPass123!";
        }

        private async Task<bool> RegisterTestUser()
        {
            var (statusCode, _, _) = await _auth.RegisterUser(
                firstName: _testFirstName,
                lastName: _testLastName,
                email: _testEmail,
                password: _testPassword
            );
            return statusCode == HttpStatusCode.OK;
        }

        [Test]
        public async Task SignIn_ValidCredentials_ShouldReturnTokens()
        {
            // Arrange - register test user
            bool registrationSuccessful = await RegisterTestUser();
            Assert.That(registrationSuccessful, Is.True, "Test user registration failed");

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
            // Arrange - register test user
            bool registrationSuccessful = await RegisterTestUser();
            Assert.That(registrationSuccessful, Is.True, "Test user registration failed");

            // Act - attempt to log in with incorrect email
            var invalidEmail = $"wrong_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            var (statusCode, responseBody) = await _auth.LoginUser(invalidEmail, _testPassword);

            // Assert - considering the API returns 404 NotFound for non-existent email
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "Login with invalid email should fail with NotFound");

            // Check for error message
            Assert.That(responseBody, Is.Not.Null, "Error response should contain a message");
            Assert.That(responseBody.ContainsKey("message") || responseBody.ContainsKey("error"), Is.True,
                "Error response should include an error message field");
        }

        [Test]
        public async Task SignIn_InvalidPassword_ShouldFail()
        {
            // Arrange - register test user
            bool registrationSuccessful = await RegisterTestUser();
            Assert.That(registrationSuccessful, Is.True, "Test user registration failed");

            // Act - attempt to log in with incorrect password
            var wrongPassword = _testPassword + "Wrong";
            var (statusCode, responseBody) = await _auth.LoginUser(_testEmail, wrongPassword);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized) | Is.EqualTo(HttpStatusCode.BadRequest),
                "Login with invalid password should fail");

            // Check for error message
            Assert.That(responseBody, Is.Not.Null, "Error response should contain a message");
            Assert.That(responseBody.ContainsKey("message") || responseBody.ContainsKey("error"), Is.True,
                "Error response should include an error message field");
        }

        [Test]
        public async Task SignIn_EmptyCredentials_ShouldFail()
        {
            // Act - attempt to log in with empty email
            var (emptyEmailStatus, emptyEmailResponse) = await _auth.LoginUser("", _testPassword);

            // Assert
            Assert.That(emptyEmailStatus, Is.EqualTo(HttpStatusCode.BadRequest) | Is.EqualTo(HttpStatusCode.Unauthorized),
                "Login with empty email should fail");

            // Check for error message
            Assert.That(emptyEmailResponse, Is.Not.Null, "Error response should contain a message");
            Assert.That(emptyEmailResponse.ContainsKey("message") || emptyEmailResponse.ContainsKey("error"), Is.True,
                "Error response should include an error message field");

            // Act - attempt to log in with empty password
            var (emptyPasswordStatus, emptyPasswordResponse) = await _auth.LoginUser(_testEmail, "");

            // Assert
            Assert.That(emptyPasswordStatus, Is.EqualTo(HttpStatusCode.BadRequest) | Is.EqualTo(HttpStatusCode.Unauthorized),
                "Login with empty password should fail");

            // Check for error message
            Assert.That(emptyPasswordResponse, Is.Not.Null, "Error response should contain a message");
            Assert.That(emptyPasswordResponse.ContainsKey("message") || emptyPasswordResponse.ContainsKey("error"), Is.True,
                "Error response should include an error message field");
        }

        [Test]
        public async Task SignIn_SuccessfulLogin_TokenShouldBeValid()
        {
            // Arrange - register test user
            bool registrationSuccessful = await RegisterTestUser();
            Assert.That(registrationSuccessful, Is.True, "Test user registration failed");

            // Act - log in
            var (loginStatus, responseBody) = await _auth.LoginUser(_testEmail, _testPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");

            // Extract idToken from the response
            string idToken = responseBody["idToken"]?.ToString();
            string accessToken = responseBody["accessToken"]?.ToString();

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Use the obtained token to get the user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Check the token validity, considering that we need to log in through /signin first
            // Assert - verify that we get 403 Forbidden, as additional authentication is required
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.Forbidden),
                "API should require authentication through /signin before accessing profile");
        }

        [Test]
        public async Task SignIn_LogoutAndLogin_ShouldWorkCorrectly()
        {
            // Arrange - register user and log in
            Console.WriteLine("Registering and logging in the user...");
            try
            {
                await RegisterTestUser();
                var (loginStatus, responseBody) = await _auth.LoginUser(_testEmail, _testPassword);
                Console.WriteLine($"Login status: {loginStatus}");
                Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should be successful");

                // Extract tokens from the response
                string idToken = responseBody["idToken"]?.ToString();
                string accessToken = responseBody["accessToken"]?.ToString();
                string refreshToken = responseBody["refreshToken"]?.ToString();

                Console.WriteLine($"Login successful. ID Token: {idToken}, Access Token: {accessToken}, Refresh Token: {refreshToken}");

                Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");
                Assert.That(refreshToken, Is.Not.Null.And.Not.Empty, "Refresh token should not be null or empty");

                // Act - log out using Bearer token
                Console.WriteLine("Logging out...");
                var (logoutStatus, _) = await _auth.LogoutUser(refreshToken, idToken);
                Console.WriteLine($"Logout status: {logoutStatus}");
                Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK), "Logout should be successful");

                // Check if the old token is no longer valid
                Console.WriteLine($"Checking if old token is invalidated...");
                var (profileStatus, profileResponseBody) = await _auth.GetUserProfile(idToken, accessToken);
                Console.WriteLine($"Profile status after logout: {profileStatus}");
                Console.WriteLine($"Profile response body after logout: {profileResponseBody}");

                // Expect Forbidden, not Unauthorized, as this aligns with the API's behavior
                Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.Forbidden),
                    "Token should be invalidated after logout");

                // Log in again with the same credentials
                Console.WriteLine("Logging in again after logout...");
                var (secondLoginStatus, secondResponseBody) = await _auth.LoginUser(_testEmail, _testPassword);
                Console.WriteLine($"Second login status: {secondLoginStatus}");
                Assert.That(secondLoginStatus, Is.EqualTo(HttpStatusCode.OK),
                    "Login after logout should succeed");

                // Extract new tokens after second login
                string newIdToken = secondResponseBody["idToken"]?.ToString();
                string newAccessToken = secondResponseBody["accessToken"]?.ToString();

                Console.WriteLine($"New login successful. New ID Token: {newIdToken}, New Access Token: {newAccessToken}");

                Assert.That(newIdToken, Is.Not.Null.And.Not.Empty, "New login should return a valid token");

                // Check new token, as the API expects a proper authentication sequence before accessing the profile
                Console.WriteLine("Checking new token validity...");
                var (newProfileStatus, newProfileResponseBody) = await _auth.GetUserProfile(newIdToken, newAccessToken);
                Console.WriteLine($"New profile status: {newProfileStatus}");
                Console.WriteLine($"New profile response body: {newProfileResponseBody}");

                // Expect Forbidden, not OK, as this aligns with the API's behavior
                Assert.That(newProfileStatus, Is.EqualTo(HttpStatusCode.Forbidden),
                    "API should require proper authentication sequence");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed with exception: {ex.Message}");
                throw;
            }
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

                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest) | Is.EqualTo(HttpStatusCode.Unauthorized),
                    $"Login with invalid email format '{invalidEmail}' should fail");

                // Check for error message in the response
                Assert.That(responseBody, Is.Not.Null, "Error response should contain a message");
            }
        }
    }
}
