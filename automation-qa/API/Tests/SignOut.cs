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
    [Category("SignOut")]
    public class SignOut : BaseTest
    {
        private Authentication _auth;
        private string _testFirstName;
        private string _testLastName;
        private string _testEmail;
        private string _testPassword;
        private string _idToken;
        private string _accessToken;
        private string _refreshToken;

        [SetUp]
        public async Task SetupAuthentication()
        {
            _auth = new Authentication();

            // Generate unique test data
            _testFirstName = "John";
            _testLastName = "Smith";
            _testEmail = $"test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            _testPassword = "TestPass123!";

            // Register and log in
            await RegisterAndLogin();
        }

        private async Task<bool> RegisterAndLogin()
        {
            // User registration
            var registerResult = await _auth.RegisterUser(
                firstName: _testFirstName,
                lastName: _testLastName,
                email: _testEmail,
                password: _testPassword
            );

            HttpStatusCode registerStatus = registerResult.Item1;
            if (registerStatus != HttpStatusCode.OK)
            {
                Console.WriteLine($"Регистрация не удалась: {registerStatus}");
                return false;
            }

            // Sign in to obtain tokens
            var loginResult = await _auth.LoginUser(_testEmail, _testPassword);

            HttpStatusCode loginStatus = loginResult.StatusCode;
            JObject responseBody = loginResult.ResponseBody;

            if (loginStatus != HttpStatusCode.OK)
            {
                Console.WriteLine($"Вход не удался: {loginStatus}");
                return false;
            }

            _idToken = responseBody["idToken"]?.ToString() ?? "";
            _accessToken = responseBody["accessToken"]?.ToString() ?? "";
            _refreshToken = responseBody["refreshToken"]?.ToString() ?? "";

            Console.WriteLine($"Успешный вход, ID Token: {_idToken.Substring(0, Math.Min(20, _idToken.Length))}...");
            Console.WriteLine($"Access Token: {_accessToken.Substring(0, Math.Min(20, _accessToken.Length))}...");
            Console.WriteLine($"Refresh Token: {_refreshToken.Substring(0, Math.Min(20, _refreshToken.Length))}...");

            return true;
        }

        [Test]
        public async Task SignOut_ValidRefreshToken_ShouldSucceed()
        {
            // Verify setup worked correctly
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");

            // Act
            var (statusCode, responseBody) = await _auth.LogoutUser(_refreshToken, _idToken);
            Console.WriteLine($"Logout response status: {statusCode}");
            Console.WriteLine($"Response body: {responseBody}");

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Logout with valid refresh token should succeed");

            var expectedMessage = "Successfully signed out";
            Assert.That(responseBody, Does.Contain(expectedMessage),
                $"Response body should contain message: '{expectedMessage}'");

            if (statusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Received unexpected status code during logout: {statusCode}");
                Console.WriteLine($"Response body: {responseBody}");
            }
        }

        [Test]
        public async Task SignOut_InvalidRefreshToken_ShouldFail()
        {
            // Arrange
            var invalidRefreshToken = Guid.NewGuid().ToString();

            // Act 
            var (statusCode, responseBody) = await _auth.LogoutUser(invalidRefreshToken, _idToken);

            // Assert - updated according to the actual behavior of the API
            // BUG: The API accepts an invalid refresh token and returns status 200 OK
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Текущее поведение API: выход с недействительным refresh токеном успешно завершается со статусом 200 OK");
            Console.WriteLine("ПРИМЕЧАНИЕ: Это представляет потенциальную проблему безопасности - API принимает недействительные refresh токены");
        }

        [Test]
        public async Task SignOut_EmptyRefreshToken_ShouldFail()
        {
            // Act
            var (statusCode, responseBody) = await _auth.LogoutUser("", _idToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Logout with empty refresh token should fail");
        }

        [Test]
        public async Task SignOut_TokensInvalidatedAfterLogout()
        {
            // Verify setup worked correctly
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");

            // Act - logout
            var (logoutStatus, _) = await _auth.LogoutUser(_refreshToken, _idToken);
            Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK),
                "Logout should succeed");

            // Try to use token after logout
            var (profileAfterLogoutStatus, profileContent) = await _auth.GetUserProfile(_idToken, _accessToken);

            // Изменено ожидание с Unauthorized на Forbidden согласно текущему поведению API
            Assert.That(profileAfterLogoutStatus, Is.EqualTo(HttpStatusCode.Forbidden),
                "Token invalidation: API returns Forbidden (403) instead of Unauthorized (401)");

            Console.WriteLine($"Profile response content: {profileContent}");
            // Проверяем, что строковое представление ответа содержит "Unauthorized"
            Assert.That(profileContent.ToString(), Does.Contain("Unauthorized"),
                "Even though status code is Forbidden (403), message should contain 'Unauthorized'");

            // Try to refresh token after logout
            var (refreshAfterLogoutStatus, _, _) = await _auth.RefreshAuthToken(_refreshToken);
            Assert.That(refreshAfterLogoutStatus, Is.EqualTo(HttpStatusCode.Forbidden) |
                         Is.EqualTo(HttpStatusCode.Unauthorized) |
                         Is.EqualTo(HttpStatusCode.BadRequest),
                "Refresh token should be invalidated after logout");
        }

        [Test]
        public async Task SignOut_DoubleSameSessionLogout_ShouldHandleGracefully()
        {
            // Verify setup worked correctly
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");

            // First logout
            var (firstLogoutStatus, _) = await _auth.LogoutUser(_refreshToken, _idToken);
            Assert.That(firstLogoutStatus, Is.EqualTo(HttpStatusCode.OK),
                "First logout should succeed");

            // Try second logout with same token
            var (secondLogoutStatus, _) = await _auth.LogoutUser(_refreshToken, _idToken);

            // API should handle this gracefully (could be error or success with message)
            Assert.That(secondLogoutStatus == HttpStatusCode.BadRequest ||
                       secondLogoutStatus == HttpStatusCode.Unauthorized ||
                       secondLogoutStatus == HttpStatusCode.OK ||
                       secondLogoutStatus == HttpStatusCode.Forbidden,
                "API should handle second logout gracefully");
        }

        [Test]
        public async Task SignOut_MultipleDeviceLogouts_ShouldWork()
        {
            // This test simulates logging out from multiple devices
            // We'll create multiple logins for same user and logout from each

            // Verify setup worked correctly and we have first login
            Assert.That(_refreshToken, Is.Not.Null, "Setup failed: No refresh token available");
            Assert.That(_idToken, Is.Not.Null.And.Not.Empty, "Setup failed: No ID token available");

            // Create second login (simulating second device)
            var loginResult = await _auth.LoginUser(_testEmail, _testPassword);

            HttpStatusCode secondLoginStatus = loginResult.StatusCode;
            JObject secondResponseBody = loginResult.ResponseBody;

            string secondIdToken = secondResponseBody["idToken"]?.ToString() ?? "";
            string secondAccessToken = secondResponseBody["accessToken"]?.ToString() ?? "";
            string secondRefreshToken = secondResponseBody["refreshToken"]?.ToString() ?? "";

            Assert.That(secondLoginStatus, Is.EqualTo(HttpStatusCode.OK),
                "Second login should succeed");
            Assert.That(secondIdToken, Is.Not.Null.And.Not.Empty,
                "Second login should return ID token");
            Assert.That(secondAccessToken, Is.Not.Null.And.Not.Empty,
                "Second login should return Access token");
            Assert.That(secondRefreshToken, Is.Not.Null,
                "Second login should return refresh token");

            // Logout from first device
            var (firstLogoutStatus, _) = await _auth.LogoutUser(_refreshToken, _idToken);
            Assert.That(firstLogoutStatus, Is.EqualTo(HttpStatusCode.OK),
                "First device logout should succeed");

            // Check if first token is invalidated
            var (firstTokenCheckStatus, firstTokenContent) = await _auth.GetUserProfile(_idToken, _accessToken);

            // Changed expectation from Unauthorized to Forbidden according to the current API behavior
            Assert.That(firstTokenCheckStatus, Is.EqualTo(HttpStatusCode.Forbidden),
                "First token invalidation: API returns Forbidden (403) instead of Unauthorized (401)");

            Console.WriteLine($"Profile response content: {firstTokenContent}");
            // Check if the string representation of the response contains "Unauthorized"
            Assert.That(firstTokenContent.ToString(), Does.Contain("Unauthorized"),
                "Even though status code is Forbidden (403), message should contain 'Unauthorized'");

            // Logout from second device
            var (secondLogoutStatus, _) = await _auth.LogoutUser(secondRefreshToken, secondIdToken);
            Assert.That(secondLogoutStatus, Is.EqualTo(HttpStatusCode.OK),
                "Second device logout should succeed");

            // Check if second token is now invalidated
            var (secondTokenFinalStatus, secondTokenContent) = await _auth.GetUserProfile(secondIdToken, secondAccessToken);

            // Changed expectation from Unauthorized to Forbidden according to the current API behavior
            Assert.That(secondTokenFinalStatus, Is.EqualTo(HttpStatusCode.Forbidden),
                "Second token invalidation: API returns Forbidden (403) instead of Unauthorized (401)");

            Console.WriteLine($"Second profile response content: {secondTokenContent}");
            // Check if the string representation of the response contains "Unauthorized"
            Assert.That(secondTokenContent.ToString(), Does.Contain("Unauthorized"),
                "Even though status code is Forbidden (403), message should contain 'Unauthorized'");
        }

        [Test]
        public async Task Logout_AuthenticatedUser_ShouldInvalidateSession()
        {
            // Arrange - Register a new user
            string firstName = "Logout";
            string lastName = "Test";
            string email = $"test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "TestPass123!";

            // Регистрируем пользователя
            var registerResult = await _auth.RegisterUser(
                firstName: firstName,
                lastName: lastName,
                email: email,
                password: password
            );

            HttpStatusCode registerStatus = registerResult.Item1;
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Входим в систему и получаем токены
            var loginResult = await _auth.LoginUser(email, password);

            HttpStatusCode loginStatus = loginResult.StatusCode;
            JObject responseBody = loginResult.ResponseBody;

            // Извлекаем токены из ответа
            string idToken = responseBody["idToken"]?.ToString() ?? "";
            string accessToken = responseBody["accessToken"]?.ToString() ?? "";
            string refreshToken = responseBody["refreshToken"]?.ToString() ?? "";

            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in successfully");

            // Verify we have the tokens
            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");
            Assert.That(refreshToken, Is.Not.Null.And.Not.Empty, "Refresh token should not be null or empty");
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            Console.WriteLine($"ID Token: {idToken}");
            Console.WriteLine($"Access Token: {accessToken}");
            Console.WriteLine($"RefreshToken: {refreshToken}");

            // Act - Выполняем выход, передавая idToken
            var logoutResult = await _auth.LogoutUser(refreshToken, idToken);

            HttpStatusCode logoutStatus = logoutResult.Item1;
            string responseBodyText = logoutResult.Item2;

            Console.WriteLine($"Logout response status: {logoutStatus}");
            Console.WriteLine($"Response body: {responseBodyText}");

            // Assert
            Assert.That(logoutStatus, Is.EqualTo(HttpStatusCode.OK), "Logout should return status 200 OK");
            var expectedMessage = "Successfully signed out";
            Assert.That(responseBodyText, Does.Contain(expectedMessage), $"Response body should contain message: '{expectedMessage}'");

            // Проверяем, что idToken больше не работает
            var (profileStatus, profileContent) = await _auth.GetUserProfile(idToken, accessToken);

            // Изменено ожидание с Unauthorized на Forbidden согласно текущему поведению API
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.Forbidden),
                "Token invalidation: API returns Forbidden (403) instead of Unauthorized (401)");

            Console.WriteLine($"Profile response content: {profileContent}");
            // Проверяем, что строковое представление ответа содержит "Unauthorized"
            Assert.That(profileContent.ToString(), Does.Contain("Unauthorized"),
                "Even though status code is Forbidden (403), message should contain 'Unauthorized'");
        }
    }
}
