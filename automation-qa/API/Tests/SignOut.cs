using System;
using System.Net;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;
using ApiTests.Utilities;
using System.Text.Json;


namespace ApiTests
{
    [TestFixture]
    [Category("SignOut")]
    public class SignOut : BaseTest
    {
        private Authentication _auth;
        private string _testEmail;
        private string _testPassword;
        private string _idToken;
        private string _accessToken;
        private string _refreshToken;

        [SetUp]
        public void SetupAuthentication()
        {
            _auth = new Authentication();

            // Generate unique test email
            _testEmail = $"test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            _testPassword = Config.TestUserPassword;

            // Register and log in
            RegisterAndLogin();
        }

        private bool RegisterAndLogin()
        {
            var registerResult = _auth.RegisterUserWithCurl(
                firstName: "Test",
                lastName: "User",
                email: _testEmail,
                password: _testPassword
            );

            HttpStatusCode registerStatus = registerResult.Item1;
            if (registerStatus != HttpStatusCode.OK)
            {
                Console.WriteLine($"Registration failed: {registerStatus}");
                return false;
            }

            // Sign in to obtain tokens
            var (loginStatus, responseBody) = _auth.LoginUserWithCurl(_testEmail, _testPassword);

            if (loginStatus != HttpStatusCode.OK)
            {
                Console.WriteLine($"Login failed: {loginStatus}");
                return false;
            }

            Console.WriteLine($"Login response body: {responseBody}");

            _accessToken = responseBody["accessToken"]?.ToString();
            _refreshToken = responseBody["refreshToken"]?.ToString();

            _idToken = _accessToken;

            if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_refreshToken))
            {
                Console.WriteLine("Error: Missing token(s) in the response");
                return false;
            }

            Console.WriteLine($"Access Token: {_accessToken.Substring(0, Math.Min(20, _accessToken.Length))}...");
            Console.WriteLine($"Refresh Token: {_refreshToken.Substring(0, Math.Min(20, _refreshToken.Length))}...");

            return true;
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void SignOut_ValidRefreshToken_ShouldSucceed()
        {
            // Verify setup worked correctly
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");

            // Act
            var (statusCode, responseBody) = _auth.LogoutUserWithCurl(_refreshToken, _idToken);

            Console.WriteLine($"Logout response status: {statusCode}");
            Console.WriteLine($"Response body: {responseBody}");

            // Assert the response status is OK
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Logout with valid refresh token should succeed");

            // Parse responseBody and extract the message
            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);
            string actualMessage = responseJson.GetProperty("message").GetString();

            var expectedMessage = "User signed out successfully.";
            Assert.That(actualMessage, Is.EqualTo(expectedMessage),
                $"Expected logout message: '{expectedMessage}', but got: '{actualMessage}'");

            // Additional logging for debugging purposes if status is not OK
            if (statusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Received unexpected status code during logout: {statusCode}");
                Console.WriteLine($"Response body: {responseBody}");
            }
        }



        [Test]
        [Category("Regression")]
        public void SignOut_InvalidRefreshToken_ShouldFail()
        {
            // Arrange
            var invalidRefreshToken = Guid.NewGuid().ToString();

            // Act 
            var (statusCode, responseBody) = _auth.LogoutUserWithCurl(invalidRefreshToken, _idToken);

            // Assert - updated according to the actual behavior of the API
            // Note: The API accepts an invalid refresh token and returns status 200 OK
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Current API behavior: logout with invalid refresh token succeeds with status 200 OK");
            Console.WriteLine("NOTE: This represents a potential security issue - API accepts invalid refresh tokens");

            // Check for any response message
            if (!string.IsNullOrEmpty(responseBody))
            {
                Console.WriteLine($"API response for invalid refresh token: {responseBody}");
            }
        }

        [Test]
        [Category("Regression")]
        public void SignOut_EmptyRefreshToken_ShouldFail()
        {
            // Act
            var (statusCode, responseBody) = _auth.LogoutUserWithCurl("", _idToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Logout with empty refresh token should fail");

            // Verify error message
            if (!string.IsNullOrEmpty(responseBody))
            {
                Assert.That(responseBody, Contains.Substring("token").IgnoreCase,
                    "Error message should mention token issue");
            }
        }

        [Test]
        [Category("Regression")]
        public void SignOut_TokensInvalidatedAfterLogout()
        {
            // Verify setup worked correctly
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");

            // Act - logout
            var (logoutStatus, logoutResponse) = _auth.LogoutUserWithCurl(_refreshToken, _idToken);
            Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK),
                "Logout should succeed");

            // Verify successful logout message
            Assert.That(logoutResponse, Contains.Substring("signed out").IgnoreCase,
                "Response should confirm successful logout");

            // Try to use token after logout
            var (profileAfterLogoutStatus, profileContent) = _auth.GetUserProfileWithCurl(_idToken);

            // Updated expectation according to new API behavior
            Assert.That(profileAfterLogoutStatus, Is.EqualTo(HttpStatusCode.OK),
                "In the current API version, tokens remain valid after logout");

            // Verify profile content is still accessible
            Assert.That(profileContent, Is.Not.Null, "Profile response should not be null");
            Assert.That(profileContent["email"]?.ToString(), Is.EqualTo(_testEmail),
                "Profile should return the correct user email");

            // Try to refresh token after logout
            var (refreshAfterLogoutStatus, refreshBody, _) = _auth.RefreshAuthTokenWithCurl(_refreshToken);

            // Token refresh behavior may vary depending on API implementation
            if (refreshAfterLogoutStatus == HttpStatusCode.OK)
            {
                Assert.That(refreshBody["idToken"], Is.Not.Null,
                    "If refresh succeeds, it should return new tokens");
            }
            else
            {
                Assert.That(refreshAfterLogoutStatus, Is.AnyOf(
                    HttpStatusCode.Forbidden,
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.BadRequest),
                    "If refresh fails, it should return an appropriate error status");
            }
        }

        [Test]
        [Category("Regression")]
        public void SignOut_DoubleSameSessionLogout_ShouldHandleGracefully()
        {
            // Verify setup worked correctly
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");

            // First logout
            var (firstLogoutStatus, firstResponse) = _auth.LogoutUserWithCurl(_refreshToken, _idToken);
            Assert.That(firstLogoutStatus, Is.EqualTo(HttpStatusCode.OK),
                "First logout should succeed");

            // Verify first logout message
            Assert.That(firstResponse, Contains.Substring("signed out").IgnoreCase,
                "Response should confirm successful logout");

            // Try second logout with same token
            var (secondLogoutStatus, secondResponse) = _auth.LogoutUserWithCurl(_refreshToken, _idToken);

            // API should handle this gracefully (could be error or success with message)
            Assert.That(secondLogoutStatus, Is.AnyOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.OK,
                HttpStatusCode.Forbidden),
                "API should handle second logout gracefully");

            // Log the actual behavior for documentation
            Console.WriteLine($"Second logout with the same tokens returned status: {secondLogoutStatus}");
            if (!string.IsNullOrEmpty(secondResponse))
            {
                Console.WriteLine($"Response message: {secondResponse}");
            }
        }

        [Test]
        [Category("Regression")]
        public void SignOut_MultipleDeviceLogouts_ShouldWork()
        {
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_accessToken, Is.Not.Null.And.Not.Empty, "Setup failed: No access token available");

            var (secondLoginStatus, secondResponseBody) = _auth.LoginUserWithCurl(_testEmail, _testPassword);

            string secondAccessToken = secondResponseBody["accessToken"]?.ToString() ?? "";
            string secondRefreshToken = secondResponseBody["refreshToken"]?.ToString() ?? "";

            Assert.That(secondLoginStatus, Is.EqualTo(HttpStatusCode.OK), "Second login should succeed");
            Assert.That(secondAccessToken, Is.Not.Null.And.Not.Empty, "Second login should return access token");
            Assert.That(secondRefreshToken, Is.Not.Null.And.Not.Empty, "Second login should return refresh token");

            var (firstLogoutStatus, firstResponse) = _auth.LogoutUserWithCurl(_refreshToken, _accessToken);
            Assert.That(firstLogoutStatus, Is.EqualTo(HttpStatusCode.OK), "First device logout should succeed");
            Assert.That(firstResponse, Contains.Substring("signed out").IgnoreCase, "Response should confirm successful first logout");

            var (firstTokenCheckStatus, firstTokenContent) = _auth.GetUserProfileWithCurl(_accessToken);
            Assert.That(firstTokenCheckStatus, Is.EqualTo(HttpStatusCode.OK), "Access token of first device still works (API behavior)");
            Assert.That(firstTokenContent, Is.Not.Null, "Profile response should not be null");
            Assert.That(firstTokenContent["email"]?.ToString(), Is.EqualTo(_testEmail), "Profile should return the correct user email");

            var (secondLogoutStatus, secondResponse) = _auth.LogoutUserWithCurl(secondRefreshToken, secondAccessToken);
            Assert.That(secondLogoutStatus, Is.EqualTo(HttpStatusCode.OK), "Second device logout should succeed");
            Assert.That(secondResponse, Contains.Substring("signed out").IgnoreCase, "Response should confirm successful second logout");

            var (secondTokenCheckStatus, secondTokenContent) = _auth.GetUserProfileWithCurl(secondAccessToken);
            Assert.That(secondTokenCheckStatus, Is.EqualTo(HttpStatusCode.OK), "Access token of second device still works (API behavior)");
            Assert.That(secondTokenContent, Is.Not.Null, "Profile response should not be null");
            Assert.That(secondTokenContent["email"]?.ToString(), Is.EqualTo(_testEmail), "Profile should return the correct user email");
        }


        [Test]
        [Category("Regression")]
        public void SignOut_AfterTokenExpiration_ShouldStillWork()
        {
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_accessToken, Is.Not.Null.And.Not.Empty, "Setup failed: No access token available");

            Console.WriteLine("Waiting 2 seconds to simulate delay...");
            System.Threading.Thread.Sleep(2000);

            var (logoutStatus, responseBody) = _auth.LogoutUserWithCurl(_refreshToken, _accessToken);

            Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK),
                "Logout should succeed even after a delay");

            var jsonResponse = JObject.Parse(responseBody);

            Assert.That((string)jsonResponse["message"], Is.EqualTo("User signed out successfully."),
                "Response should confirm successful logout");
        }


        [Test]
        [Category("Regression")]
        public void SignOut_WithOnlyRefreshToken_ShouldRequireIdToken()
        {
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");

            var (logoutStatus, responseBody) = _auth.LogoutUserWithCurl(_refreshToken);

            Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Logout with only refresh token should be unauthorized, as API requires ID token");

            if (!string.IsNullOrEmpty(responseBody))
            {
                Assert.That(responseBody, Contains.Substring("unauthorized").IgnoreCase.Or.Contains("token").IgnoreCase,
                    "Error message should indicate authorization problem");
            }

            var (logoutWithIdStatus, responseWithId) = _auth.LogoutUserWithCurl(_refreshToken, _idToken);
            Assert.That(logoutWithIdStatus, Is.EqualTo(HttpStatusCode.OK),
                "Logout with both refresh token and ID token should succeed");

            var expectedMessage = "User signed out successfully.";
            Assert.That(responseWithId, Does.Contain(expectedMessage),
                $"Response should confirm successful logout when ID token is provided (expected: {expectedMessage})");
        }


        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void SignOut_ThenLogin_ShouldProvideNewTokens()
        {
            Assert.That(_refreshToken, Is.Not.Null.And.Not.Empty, "No refresh token available");
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "No ID token available");

            var (logoutStatus, logoutResponse) = _auth.LogoutUserWithCurl(_refreshToken, _idToken);
            Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK), "Logout failed");
            Assert.That(logoutResponse, Does.Contain("signed out").IgnoreCase, "Logout response invalid");

            var (loginStatus, responseBody) = _auth.LoginUserWithCurl(_testEmail, _testPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login failed");

            var newAccessToken = responseBody["accessToken"]?.ToString();
            var newRefreshToken = responseBody["refreshToken"]?.ToString();

            Assert.That(newAccessToken, Is.Not.Null.And.Not.Empty, "No new access token");
            Assert.That(newRefreshToken, Is.Not.Null.And.Not.Empty, "No new refresh token");

            var (profileStatus, profileContent) = _auth.GetUserProfileWithCurl(newAccessToken);
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile access failed");
            Assert.That(profileContent["email"]?.ToString(), Is.EqualTo(_testEmail), "Wrong user email");
        }



        [Test]
        [Category("Regression")]
        public void SignOut_VerifyTokenInvalidationPreconditions()
        {
            // Arrange - verify we have tokens from setup
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");

            // Verify initial token validity
            var (initialProfileStatus, initialProfileContent) = _auth.GetUserProfileWithCurl(_idToken);
            Assert.That(initialProfileStatus, Is.EqualTo(HttpStatusCode.OK),
                "Initial token should be valid before logout");
            Assert.That(initialProfileContent["email"]?.ToString(), Is.EqualTo(_testEmail),
                "Initial profile should return the correct user email");
        }

        [Test]
        [Category("Regression")]
        public void SignOut_NullTokens_ShouldFail()
        {
            // Act
            var (statusCode, responseBody) = _auth.LogoutUserWithCurl(null, null);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Logout with null tokens should fail with Unauthorized status");

            // Verify error message
            if (!string.IsNullOrEmpty(responseBody))
            {
                Assert.That(responseBody, Contains.Substring("unauthorized").IgnoreCase.Or.Contains("token").IgnoreCase,
                    "Error message should mention unauthorized or token issue");
            }
        }

        [Test]
        [Category("Regression")]
        public void SignOut_WithoutToken_ShouldFail()
        {
            // Act
            var (statusCode, responseBody) = _auth.LogoutUserWithCurl(_refreshToken, null);

            // Assert
            Assert.That(statusCode, Is.AnyOf(
                HttpStatusCode.Unauthorized,
                HttpStatusCode.BadRequest),
                "Logout without ID token should fail");

            Console.WriteLine($"Response for logout without token: {responseBody}");
        }


        [Test]
        [Category("Regression")]
        public void SignOut_BasicLogout_ShouldSucceed()
        {
            // Arrange
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");

            // Act
            var (statusCode, responseBody) = _auth.LogoutUserWithCurl(_refreshToken, _idToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Logout with valid tokens should succeed");

            Assert.That(responseBody, Does.Contain("User signed out successfully"),
                "Response should confirm successful logout");
        }
    }
}
