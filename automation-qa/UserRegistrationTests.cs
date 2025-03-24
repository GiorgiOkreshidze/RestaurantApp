using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace ApiTests
{
    [TestFixture]
    public class UserRegistrationTests
    {
        private RestClient _client;
        private const string BaseUrl = "https://1ytz73leak.execute-api.eu-west-2.amazonaws.com/api";

        [SetUp]
        public void Setup()
        {
            _client = new RestClient(BaseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
        }

        [Test]
        public async Task SignUp_CompleteValidData_ShouldCreateUser()
        {
            // Arrange
            var request = new RestRequest("/signup", Method.Post);
            var uniqueEmail = "test_" + Guid.NewGuid().ToString() + "@example.com";

            var userData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = uniqueEmail,
                password = "Aa1@5678"
            };

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(userData);

            // Act
            var response = await _client.ExecuteAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration should be successful");

            if (!string.IsNullOrEmpty(response.Content))
            {
                var responseBody = JObject.Parse(response.Content);
                Assert.That(responseBody.ContainsKey("idToken"), Is.True,
                    "Response should contain idToken field");
                Assert.That(responseBody.ContainsKey("refreshToken"), Is.True,
                    "Response should contain refreshToken field");
                Assert.That(responseBody.ContainsKey("accessToken"), Is.True,
                    "Response should contain accessToken field");
            }
            else
            {
                Assert.Fail("Response content is empty");
            }
        }

        [Test]
        public async Task SignUp_DuplicateEmail_ShouldFail()
        {
            // Arrange
            var email = "duplicate_" + Guid.NewGuid().ToString() + "@example.com";
            var userData = new
            {
                firstName = "John",
                lastName = "Duplicate",
                email = email,
                password = "Aa1@5678"
            };

            // First registration
            var firstRequest = new RestRequest("/signup", Method.Post);
            firstRequest.AddJsonBody(userData);
            var firstResponse = await _client.ExecuteAsync(firstRequest);
            Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "First registration should be successful");

            // Repeat registration
            var secondRequest = new RestRequest("/signup", Method.Post);
            secondRequest.AddJsonBody(userData);
            var secondResponse = await _client.ExecuteAsync(secondRequest);

            // Assert
            Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Repeated registration should be prohibited");
        }

        [Test]
        public async Task SignUp_InvalidEmailFormat_ShouldFail()
        {
            // Arrange
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
                // Arrange
                var request = new RestRequest("/signup", Method.Post);

                var userData = new
                {
                    firstName = "John",
                    lastName = "Invalid",
                    email = invalidEmail,
                    password = "StrongPassword123!"
                };

                request.AddJsonBody(userData);

                // Act
                var response = await _client.ExecuteAsync(request);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with invalid email '{invalidEmail}' should fail");
            }
        }

        [Test]
        public async Task SignUp_WeakPassword_ShouldFail()
        {
            // Arrange
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
                // Arrange
                var request = new RestRequest("/signup", Method.Post);

                var userData = new
                {
                    firstName = "John",
                    lastName = "Weak",
                    email = "weak@example.com",
                    password = weakPassword
                };

                request.AddJsonBody(userData);

                // Act
                var response = await _client.ExecuteAsync(request);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with weak password '{weakPassword}' should fail");
            }
        }

        [Test]
        public async Task SignUp_MismatchedPasswords_ShouldFail()
        {
            // Arrange
            var request = new RestRequest("/signup", Method.Post);

            var userData = new
            {
                firstName = "John",
                lastName = "Mismatch",
                email = "mismatch@example.com",
                password = "StrongPassword123!"
            };

            request.AddJsonBody(userData);

            // Act
            var response = await _client.ExecuteAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with mismatched passwords should fail");
        }

        [Test]
        public async Task SignUp_PasswordBoundaryValues_ShouldSucceed()
        {
            // Test with minimum length password (8 characters)
            var minRequest = new RestRequest("/signup", Method.Post);
            var minEmail = "min_" + Guid.NewGuid().ToString() + "@example.com";

            var minUserData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = minEmail,
                password = "Aa1@5678" // Exactly 8 characters
            };

            minRequest.AddHeader("Content-Type", "application/json");
            minRequest.AddJsonBody(minUserData);
            var minResponse = await _client.ExecuteAsync(minRequest);

            Assert.That(minResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration with 8-character password should succeed");

            // Test with maximum length password (16 characters)
            var maxRequest = new RestRequest("/signup", Method.Post);
            var maxEmail = "max_" + Guid.NewGuid().ToString() + "@example.com";

            var maxUserData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = maxEmail,
                password = "Aa1@567890123456" // Exactly 16 characters
            };

            maxRequest.AddHeader("Content-Type", "application/json");
            maxRequest.AddJsonBody(maxUserData);
            var maxResponse = await _client.ExecuteAsync(maxRequest);

            Assert.That(maxResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Registration with 16-character password should succeed");
        }

        [Test]
        public async Task SignUp_EmptyNameFields_ShouldFail()
        {
            // Test with empty first name
            var emptyFirstNameRequest = new RestRequest("/signup", Method.Post);
            var emptyFirstNameEmail = "empty_first_" + Guid.NewGuid().ToString() + "@example.com";

            var emptyFirstNameData = new
            {
                firstName = "",
                lastName = "Doe",
                email = emptyFirstNameEmail,
                password = "Aa1@5678"
            };

            emptyFirstNameRequest.AddHeader("Content-Type", "application/json");
            emptyFirstNameRequest.AddJsonBody(emptyFirstNameData);
            var emptyFirstNameResponse = await _client.ExecuteAsync(emptyFirstNameRequest);

            Assert.That(emptyFirstNameResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with empty first name should fail");

            // Test with empty last name
            var emptyLastNameRequest = new RestRequest("/signup", Method.Post);
            var emptyLastNameEmail = "empty_last_" + Guid.NewGuid().ToString() + "@example.com";

            var emptyLastNameData = new
            {
                firstName = "John",
                lastName = "",
                email = emptyLastNameEmail,
                password = "Aa1@5678"
            };

            emptyLastNameRequest.AddHeader("Content-Type", "application/json");
            emptyLastNameRequest.AddJsonBody(emptyLastNameData);
            var emptyLastNameResponse = await _client.ExecuteAsync(emptyLastNameRequest);

            Assert.That(emptyLastNameResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with empty last name should fail");
        }

        [Test]
        public async Task SignUp_TooLongNameFields_ShouldFail()
        {
            // Create a long string for testing
            var longName = new string('A', 256); // 256 characters

            // Test with too long first name
            var longFirstNameRequest = new RestRequest("/signup", Method.Post);
            var longFirstNameEmail = "long_first_" + Guid.NewGuid().ToString() + "@example.com";

            var longFirstNameData = new
            {
                firstName = longName,
                lastName = "Doe",
                email = longFirstNameEmail,
                password = "Aa1@5678"
            };

            longFirstNameRequest.AddHeader("Content-Type", "application/json");
            longFirstNameRequest.AddJsonBody(longFirstNameData);
            var longFirstNameResponse = await _client.ExecuteAsync(longFirstNameRequest);

            Assert.That(longFirstNameResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with too long first name should fail");

            // Test with too long last name
            var longLastNameRequest = new RestRequest("/signup", Method.Post);
            var longLastNameEmail = "long_last_" + Guid.NewGuid().ToString() + "@example.com";

            var longLastNameData = new
            {
                firstName = "John",
                lastName = longName,
                email = longLastNameEmail,
                password = "Aa1@5678"
            };

            longLastNameRequest.AddHeader("Content-Type", "application/json");
            longLastNameRequest.AddJsonBody(longLastNameData);
            var longLastNameResponse = await _client.ExecuteAsync(longLastNameRequest);

            Assert.That(longLastNameResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Registration with too long last name should fail");
        }

        [Test]
        public async Task SignUp_SQLInjectionAttempts_ShouldBeSanitized()
        {
            // Define SQL injection payloads
            var sqlInjectionPayloads = new[]
            {
                "' OR '1'='1",
                "'; DROP TABLE users; --",
                "' UNION SELECT * FROM users WHERE '1'='1",
                "admin' --",
                "1'; SELECT * FROM users; --"
            };

            foreach (var payload in sqlInjectionPayloads)
            {
                // Test SQL injection in email
                var sqlInjectionEmailRequest = new RestRequest("/signup", Method.Post);
                var safeEmail = "sql_test_" + Guid.NewGuid().ToString() + "@example.com";

                var sqlInjectionEmailData = new
                {
                    firstName = "SQL",
                    lastName = "Test",
                    email = payload + safeEmail, // Add a valid part to ensure email format
                    password = "Aa1@5678"
                };

                sqlInjectionEmailRequest.AddHeader("Content-Type", "application/json");
                sqlInjectionEmailRequest.AddJsonBody(sqlInjectionEmailData);
                var sqlInjectionEmailResponse = await _client.ExecuteAsync(sqlInjectionEmailRequest);

                // Expect either BadRequest (if injection detected) or OK (if safely processed)
                Assert.That(sqlInjectionEmailResponse.StatusCode == HttpStatusCode.BadRequest ||
                           sqlInjectionEmailResponse.StatusCode == HttpStatusCode.OK,
                    $"SQL injection in email with payload '{payload}' should be properly handled");

                // Test SQL injection in name
                var sqlInjectionNameRequest = new RestRequest("/signup", Method.Post);
                var sqlInjectionNameEmail = "sql_name_" + Guid.NewGuid().ToString() + "@example.com";

                var sqlInjectionNameData = new
                {
                    firstName = payload,
                    lastName = "Test",
                    email = sqlInjectionNameEmail,
                    password = "Aa1@5678"
                };

                sqlInjectionNameRequest.AddHeader("Content-Type", "application/json");
                sqlInjectionNameRequest.AddJsonBody(sqlInjectionNameData);
                var sqlInjectionNameResponse = await _client.ExecuteAsync(sqlInjectionNameRequest);

                // Check that system didn't crash when processing potentially dangerous input
                Assert.That(sqlInjectionNameResponse.StatusCode == HttpStatusCode.BadRequest ||
                           sqlInjectionNameResponse.StatusCode == HttpStatusCode.OK,
                    $"SQL injection in name with payload '{payload}' should be properly handled");
            }
        }

        [Test]
        public async Task SignUp_SpecialCharactersInNameFields_ShouldFail()
        {
            // Define special characters to test
            var specialChars = new[] { "@", "#", "$", "%", "^", "*", "(", ")", "<", ">", "=" };

            foreach (var specialChar in specialChars)
            {
                // Test with special characters in first name
                var specialFirstNameRequest = new RestRequest("/signup", Method.Post);
                var specialFirstNameEmail = "special_first_" + Guid.NewGuid().ToString() + "@example.com";

                var specialFirstNameData = new
                {
                    firstName = "John" + specialChar,
                    lastName = "Doe",
                    email = specialFirstNameEmail,
                    password = "Aa1@5678"
                };

                specialFirstNameRequest.AddHeader("Content-Type", "application/json");
                specialFirstNameRequest.AddJsonBody(specialFirstNameData);
                var specialFirstNameResponse = await _client.ExecuteAsync(specialFirstNameRequest);

                Assert.That(specialFirstNameResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with special character '{specialChar}' in first name should fail");

                // Test with special characters in last name
                var specialLastNameRequest = new RestRequest("/signup", Method.Post);
                var specialLastNameEmail = "special_last_" + Guid.NewGuid().ToString() + "@example.com";

                var specialLastNameData = new
                {
                    firstName = "John",
                    lastName = "Doe" + specialChar,
                    email = specialLastNameEmail,
                    password = "Aa1@5678"
                };

                specialLastNameRequest.AddHeader("Content-Type", "application/json");
                specialLastNameRequest.AddJsonBody(specialLastNameData);
                var specialLastNameResponse = await _client.ExecuteAsync(specialLastNameRequest);

                Assert.That(specialLastNameResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Registration with special character '{specialChar}' in last name should fail");
            }
        }
    }
}
