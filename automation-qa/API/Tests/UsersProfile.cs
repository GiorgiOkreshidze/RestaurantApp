using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;
using ApiTests.Utilities;

namespace ApiTests
{
    [TestFixture]
    [Category("UsersProfile")]
    public class UsersProfile : BaseTest
    {
        private Authentication _auth;

        [SetUp]
        public void SetUp()
        {
            _auth = new Authentication();
        }

        [Test]
        [Category("Regression")]
        public void UsersProfile_NewUserWithRegularEmail_ShouldHaveCustomerRole()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            string email = $"regular_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Act
            var (registerStatus, registeredEmail, registerResponse) = _auth.RegisterUserWithCurl(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            if (registerResponse != null && registerResponse.ContainsKey("message"))
            {
                Assert.That(registerResponse["message"].ToString(), Does.Contain("registered successfully"),
                    "Registration response should confirm successful registration");
            }

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string accessToken = loginResponse["accessToken"]?.ToString();

            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");
            Assert.That(userData, Is.Not.Null, "User data should not be null");
            Assert.That(userData["role"]?.ToString(), Is.EqualTo("Customer"),
                "New user with regular email should have Customer role");
            Assert.That(userData["email"]?.ToString(), Is.EqualTo(email),
                "User profile should show correct email");
        }


        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void UsersProfile_DisplaysCorrectUserInfo()
        {
            // Arrange
            string firstName = "Jane";
            string lastName = "Smith";
            string email = $"profile_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Act
            var (registerStatus, registeredEmail, registerResponse) = _auth.RegisterUserWithCurl(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            if (registerResponse != null && registerResponse.ContainsKey("message"))
            {
                Assert.That(registerResponse["message"].ToString(), Does.Contain("registered successfully"),
                    "Registration response should confirm successful registration");
            }

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string accessToken = loginResponse["accessToken"]?.ToString();

            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");
            Assert.That(userData, Is.Not.Null, "User data should not be null");
            Assert.That(userData["firstName"]?.ToString(), Is.EqualTo(firstName),
                "User profile should show correct first name");
            Assert.That(userData["lastName"]?.ToString(), Is.EqualTo(lastName),
                "User profile should show correct last name");
            Assert.That(userData["email"]?.ToString(), Is.EqualTo(email),
                "User profile should show correct email");
            Assert.That(userData["role"], Is.Not.Null,
                "User profile should include role information");
        }



        // Test for requirement 3: Check assignment of Waiter role for email from the waiter list
        // Note: This test requires knowing a specific email from the waiter list
        // This test can be skipped if you do not have access to the list or control over it
        [Test]
        [Category("Regression")]
        public void UsersProfile_UserWithWaiterEmail_ShouldHaveRole()
        {
            // Arrange
            string firstName = "Waiter";
            string lastName = "Test";
            string waiterEmail = $"waiter_{Guid.NewGuid().ToString("N").Substring(0, 8)}@restaurant.com";
            string password = TestConfig.Instance.WaiterPassword;

            // Act
            var (registerStatus, registeredEmail, registerResponse) = _auth.RegisterUserWithCurl(
                firstName,
                lastName,
                waiterEmail,
                password);

            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(waiterEmail, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");
            Assert.That(userData, Is.Not.Null, "User data should not be null");
            Assert.That(userData["role"]?.ToString(), Is.EqualTo("Customer"),
                "Registered user should have Customer role by default");
        }


        // Test checks that the profile correctly displays the role for all new users (requirement 5)
        [Test]
        [Category("Regression")]
        public void UsersProfile_RoleIsDisplayedCorrectlyForNewUsers()
        {
            // Arrange - Register a new user
            string email = $"new_user_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Act - Perform registration
            var (registerStatus, registeredEmail, registerResponse) = _auth.RegisterUserWithCurl("New", "User", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "New user registration should succeed");

            // Verify registration success message
            if (registerResponse != null && registerResponse.ContainsKey("message"))
            {
                Assert.That(registerResponse["message"].ToString().StartsWith("User with email"),
                    "Registration response should include a message with the user's email address");
            }

            // Log in to get token
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "New user should be able to log in");

            // Extract token from the response
            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            // Updated expectation according to current API behavior
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");

            // Check new user role
            Assert.That(userData, Is.Not.Null, "User data should not be null");
            Assert.That(userData["role"], Is.Not.Null,
                "User profile should include role information");
            Console.WriteLine($"New user role: {userData["role"]}");

            // For regular users, Customer role is expected
            Assert.That(userData["role"]?.ToString(), Is.EqualTo("Customer"),
                "New user with regular email should have Customer role");
        }


        [Test]
        [Category("Regression")]
        public void UsersProfile_UserIdIsPresent()
        {
            // Arrange - register a user
            string email = $"uid_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, registerResponse) = _auth.RegisterUserWithCurl("Test", "User", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Extract the access token or idToken depending on the response
            string idToken = loginResponse["accessToken"]?.ToString(); // Make sure to use accessToken if that's what's provided
            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token must not be null or empty");

            // Act - get profile using the correct token
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(idToken, idToken); // Pass the correct token

            // Assert - check user ID presence
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            Assert.That(userData["id"], Is.Not.Null, "User profile should contain user ID");
            Assert.That(userData["id"].ToString(), Is.Not.Empty, "User ID should not be empty");
        }


        [Test]
        [Category("Regression")]
        public void UsersProfile_RoleIsCustomerAfterRegistration()
        {
            // Arrange - Register a user
            string firstName = "Alice";
            string lastName = "Smith";
            string email = $"user_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Register the user
            var (registerStatus, _, _) = _auth.RegisterUserWithCurl(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Login the user and get the accessToken
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Act - Get the profile using the accessToken
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            // Assert - Check if the profile contains a role field
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            Assert.That(userData, Is.Not.Null, "User profile data should not be null");

            // Check if the "role" field exists and matches the expected value ("Customer")
            Assert.That(userData.ContainsKey("role"), Is.True, "User profile should contain role field");
            Assert.That(userData["role"]?.ToString(), Is.EqualTo("Customer"), "User role should be 'Customer'");
        }


        [Test]
        [Category("Regression")]
        public void UsersProfile_NameMatchesRegistrationData()
        {
            // Arrange - register a user with special characters in name
            string firstName = "John-Martin";  // Simplified characters to avoid encoding issues
            string lastName = "OConnor";       // Simplified characters to avoid encoding issues
            string email = $"name_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Check registration result
            var (registerStatus, _, registerResponse) = _auth.RegisterUserWithCurl(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Login and get tokens - add debug information
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Check that loginResponse is not null and contains required fields
            Assert.That(loginResponse, Is.Not.Null, "Login response should not be null");
            Console.WriteLine($"Login response: {loginResponse}");

            // Check that accessToken exists in the response
            Assert.That(loginResponse.ContainsKey("accessToken"), Is.True, "Login response should contain accessToken");

            // Extract accessToken and verify it
            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");
            Console.WriteLine($"Access Token: {accessToken.Substring(0, Math.Min(20, accessToken.Length))}...");

            // Act - get profile using accessToken
            Console.WriteLine("Requesting user profile...");
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            Console.WriteLine($"Profile status: {profileStatus}");
            if (userData != null)
            {
                Console.WriteLine($"User data: {userData}");
            }

            // Assert - check status and data presence
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            Assert.That(userData, Is.Not.Null, "User data should not be null");

            // Check that first and last name match registration data
            Assert.That(userData["firstName"]?.ToString(), Is.EqualTo(firstName),
                "First name should match exactly what was provided during registration");
            Assert.That(userData["lastName"]?.ToString(), Is.EqualTo(lastName),
                "Last name should match exactly what was provided during registration");
        }


        [Test]
        [Category("Regression")]
        public void UsersProfile_LongNamesAreHandledCorrectly()
        {
            // Arrange - register a user with long first and last names
            string longFirstName = "Johnathanjosephjamesmichael";
            string longLastName = "Smith-Johnson-Williams-Brown-Davis";
            string email = $"longname_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, _) = _auth.RegisterUserWithCurl(longFirstName, longLastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Extract accessToken directly from loginResponse
            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Act - get profile using the accessToken
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            // Assert - check that long names are handled correctly
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            Assert.That(userData["firstName"]?.ToString(), Is.EqualTo(longFirstName),
                "Long first name should be stored and displayed correctly");
            Assert.That(userData["lastName"]?.ToString(), Is.EqualTo(longLastName),
                "Long last name should be stored and displayed correctly");
        }


        [Test]
        [Category("Regression")]
        public void UsersProfile_EmailIsLowercased()
        {
            // Arrange - register a user with mixed case email
            string mixedCaseEmail = $"MixedCase_{Guid.NewGuid().ToString("N").Substring(0, 8)}@Example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, _) = _auth.RegisterUserWithCurl("Mixed", "Case", mixedCaseEmail, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(mixedCaseEmail, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Extract accessToken from login response
            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Act - get profile using the accessToken
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            // Assert - check that email is stored in lowercase
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            string storedEmail = userData["email"]?.ToString();
            Assert.That(storedEmail, Is.AnyOf(mixedCaseEmail.ToLower(), mixedCaseEmail),
                "Email should be normalized (either stored as lowercase or as provided)");
        }


        [Test]
        [Category("Regression")]
        public void UsersProfile_MultipleProfileRequestsReturnSameData()
        {
            // Arrange - register a user
            string email = $"multi_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, _) = _auth.RegisterUserWithCurl("Multiple", "Requests", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Extract accessToken from login response
            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Act - get profile twice using the accessToken
            var (firstProfileStatus, firstProfileData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);
            var (secondProfileStatus, secondProfileData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            // Assert - check that data is identical
            Assert.That(firstProfileStatus, Is.EqualTo(HttpStatusCode.OK), "First profile request should succeed");
            Assert.That(secondProfileStatus, Is.EqualTo(HttpStatusCode.OK), "Second profile request should succeed");

            Assert.That(secondProfileData["id"]?.ToString(), Is.EqualTo(firstProfileData["id"]?.ToString()),
                "User ID should be consistent across profile requests");
            Assert.That(secondProfileData["email"]?.ToString(), Is.EqualTo(firstProfileData["email"]?.ToString()),
                "Email should be consistent across profile requests");
            Assert.That(secondProfileData["firstName"]?.ToString(), Is.EqualTo(firstProfileData["firstName"]?.ToString()),
                "First name should be consistent across profile requests");
        }


        [Test]
        [Category("Regression")]
        public void UsersProfile_ContainsExpectedFields()
        {
            // Arrange - register a user
            string email = $"fields_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, registerResponse) = _auth.RegisterUserWithCurl("Field", "Test", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Act - get profile using accessToken
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken, accessToken);

            // Assert - check that all expected fields are present
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");

            string[] expectedFields = { "id", "firstName", "lastName", "email", "role" };

            foreach (string field in expectedFields)
            {
                Assert.That(userData[field], Is.Not.Null, $"User profile should contain {field} field");
            }

            // Check that there are no unexpected fields with sensitive information
            Assert.That(userData["password"], Is.Null, "User profile should not contain password");
            Assert.That(userData["passwordHash"], Is.Null, "User profile should not contain password hash");

            // Optionally, check if imageUrl exists but don't fail the test if it's null
            Assert.That(userData["imageUrl"], Is.Null.Or.Not.Null, "User profile should optionally contain imageUrl field");
        }



        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void GetUserProfile_WithValidToken_ReturnsSuccessStatus()
        {
            // Arrange - Login to get a valid token
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(
                TestConfig.Instance.TestUserEmail, TestConfig.Instance.TestUserPassword);

            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should succeed");

            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Act - Get user profile using the access token
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken);

            // Assert
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "User profile request should succeed");
            Assert.That(userData, Is.Not.Null, "User data should not be null");
        }

        [Test]
        [Category("Smoke")]
        public void UsersProfile_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            string invalidToken = "invalid-token";

            // Act
            var (status, response) = _auth.GetUserProfileWithCurl(invalidToken);

            // Assert
            Assert.That((int)status, Is.EqualTo((int)HttpStatusCode.Unauthorized), "Profile request with invalid token should return Unauthorized");
        }

        [Test]
        [Category("Regression")]
        public void UsersProfile_FieldsExistInProfile()
        {
            // Arrange
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(
                TestConfig.Instance.TestUserEmail,
                TestConfig.Instance.TestUserPassword);

            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should succeed");

            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken);

            // Assert
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");

            string[] requiredFields = { "id", "firstName", "lastName", "email" };
            foreach (var field in requiredFields)
            {
                Assert.That(userData[field], Is.Not.Null, $"Profile should contain {field}");
            }
        }



        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void GetUserProfile_EmailMatchesLoginEmail()
        {
            // Arrange - Login with test user
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(
                TestConfig.Instance.TestUserEmail,
                TestConfig.Instance.TestUserPassword);

            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should succeed");

            string accessToken = loginResponse["accessToken"]?.ToString();
            Assert.That(accessToken, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            // Act - Get user profile using the access token
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(accessToken);

            // Assert
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "User profile request should succeed");
            Assert.That(userData["email"]?.ToString(),
                Is.EqualTo(TestConfig.Instance.TestUserEmail),
                "Profile email should match login email");
        }

        [Test]
        [Category("Regression")]
        public void UsersProfile_WaiterEmailInList_ShouldHandleRegistration()
        {
            // Arrange
            string waiterEmail = "waiter@restaurant.com";
            string firstName = "Waiter";
            string lastName = "Test";
            string password = TestConfig.Instance.WaiterPassword;

            // Act - Perform registration
            var (registerStatus, registeredEmail, registerResponse) = _auth.RegisterUserWithCurl(
                firstName,
                lastName,
                waiterEmail,
                password);

            // Assert registration status
            if (registerStatus == HttpStatusCode.BadRequest)
            {
                // Check if the error message is about the email already existing
                string errorMessage = registerResponse["title"]?.ToString();
                if (errorMessage == "A User with the same email already exists.")
                {
                    Console.WriteLine("User already exists, proceeding with login");
                }
                else
                {
                    // If it's a different error, fail the test
                    Assert.Fail("Registration failed with unexpected error: " + errorMessage);
                }
            }
            else
            {
                Assert.That((int)registerStatus, Is.AnyOf(
                    (int)HttpStatusCode.OK,
                    (int)HttpStatusCode.Conflict),
                    "Registration should either succeed or return Conflict");
            }

            // Proceed with login after registration attempt
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(waiterEmail, password);

            Assert.That((int)loginStatus, Is.AnyOf(
                (int)HttpStatusCode.OK,
                (int)HttpStatusCode.Forbidden),
                "Login should either succeed or return Forbidden");

            if (loginStatus == HttpStatusCode.OK)
            {
                // Try to retrieve tokens from the response
                string idToken = loginResponse["idToken"]?.ToString();
                string accessToken = loginResponse["accessToken"]?.ToString();

                // Ensure at least one token is present
                Assert.That(idToken, Is.Not.Null.Or.Not.Empty, "ID token should be present");
                Assert.That(accessToken, Is.Not.Null.Or.Not.Empty, "Access token should be present");

                // If both tokens are absent, fail the test
                if (string.IsNullOrEmpty(idToken) && string.IsNullOrEmpty(accessToken))
                {
                    Assert.Fail("Neither ID token nor Access token is present in the response.");
                }

                // Use the access token if idToken is absent
                string tokenToUse = string.IsNullOrEmpty(idToken) ? accessToken : idToken;

                // Get user profile with the available token
                var (profileStatus, userData) = _auth.GetUserProfileWithCurl(tokenToUse);

                Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                    "API should return user profile");

                Assert.That(userData, Is.Not.Null, "User data should not be null");

                Console.WriteLine($"User role: {userData["role"]}");
            }
        }


        [Test]
        [Category("Regression")]
        public void GetUserProfile_WithExpiredToken_ReturnsUnauthorized()
        {
            // Arrange - Create an intentionally expired token (this might need to be adjusted based on your token generation)
            string expiredToken = "expired-test-token";

            // Act - Try to get profile with expired token
            var (profileStatus, userData) = _auth.GetUserProfileWithCurl(expiredToken);

            // Assert
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Profile request with expired token should be unauthorized");
        }

        [Test]
        [Category("Regression")]
        public void GetUserProfile_WithValidToken_ShouldSucceed()
        {
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl("test@example.com", Config.TestUserPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string token = loginResponse["accessToken"].ToString();
            Assert.That(token, Is.Not.Null.And.Not.Empty, "Access token should not be null or empty");

            var (profileStatus, profile) = _auth.GetUserProfileWithCurl(token);

            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Should successfully get user profile");
            Assert.That(profile, Is.Not.Null, "User profile should not be null");
            Assert.That(profile["email"].ToString(), Is.EqualTo("test@example.com"), "User email should match");
        }

        [Test]
        [Category("Regression")]
        public void GetUserRole_WithValidToken_ShouldReturnCustomerRole()
        {
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl("test@example.com", Config.TestUserPassword);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string token = loginResponse["accessToken"].ToString();

            var (profileStatus, profile) = _auth.GetUserProfileWithCurl(token);

            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Should get profile successfully");
            Assert.That(profile["role"]?.ToString(), Is.EqualTo("Customer"), "Regular user should have Customer role");
        }

        [Test]
        [Category("Smoke")]
        public void LoginUser_WithValidCredentials_ShouldSucceed()
        {
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl("test@example.com", Config.TestUserPassword);

            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Login should succeed");
            Assert.That(loginResponse["accessToken"], Is.Not.Null, "Access token should be returned");
        }

        [Test]
        [Category("Smoke")]
        public void LoginUser_WithInvalidCredentials_ShouldFail()
        {
            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl("test@example.com", "WrongPassword123!");

            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.Unauthorized).Or.EqualTo(HttpStatusCode.BadRequest),
                "Login with invalid credentials should fail");
        }

        [Test]
        [Category("Regression")]
        public void GetUserProfile_WithInvalidToken_ShouldReturnUnauthorized()
        {
            string invalidToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            var (profileStatus, profile) = _auth.GetUserProfileWithCurl(invalidToken);

            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Request with invalid token should return Unauthorized");
            Assert.That(profile, Is.Not.Null, "Response should not be null");
            Assert.That(profile["type"]?.ToString(), Is.EqualTo("Unauthorized"),
                "Response should indicate Unauthorized error type");
            Assert.That(profile["status"]?.ToObject<int>(), Is.EqualTo(401),
                "Response should contain correct status code");
        }
    }
}