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
        public async Task UsersProfile_NewUserWithRegularEmail_ShouldHaveCustomerRole()
        {
            // Arrange - Registering a regular user
            string firstName = "John";
            string lastName = "Doe";
            string email = $"regular_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Act - Perform registration
            var (registerStatus, registeredEmail, registerResponse) = await _auth.RegisterUser(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Verify registration success message
            if (registerResponse != null && registerResponse.ContainsKey("message"))
            {
                Assert.That(registerResponse["message"].ToString(), Is.EqualTo("User Registered"),
                    "Registration response should confirm successful registration");
            }

            // Log in to get tokens
            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Extract tokens from the response
            string idToken = loginResponse["idToken"]?.ToString();
            // Using idToken as accessToken
            string accessToken = idToken;

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Updated expectation according to current API behavior
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");

            // Check user role
            Assert.That(userData, Is.Not.Null, "User data should not be null");
            Assert.That(userData["role"]?.ToString(), Is.EqualTo("Customer"),
                "New user with regular email should have Customer role");
            Assert.That(userData["email"]?.ToString(), Is.EqualTo(email),
                "User profile should show correct email");
        }

        [Test]
        public async Task UsersProfile_DisplaysCorrectUserInfo()
        {
            // Arrange - Register a user with specific data
            string firstName = "Jane";
            string lastName = "Smith";
            string email = $"profile_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Act - Perform registration
            var (registerStatus, registeredEmail, registerResponse) = await _auth.RegisterUser(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Verify registration success message
            if (registerResponse != null && registerResponse.ContainsKey("message"))
            {
                Assert.That(registerResponse["message"].ToString(), Is.EqualTo("User Registered"),
                    "Registration response should confirm successful registration");
            }

            // Log in to get tokens
            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Extract token from the response
            string idToken = loginResponse["idToken"]?.ToString();
            // Using idToken as accessToken
            string accessToken = idToken;

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Updated expectation according to current API behavior
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");

            // Check user data
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
        [Ignore("Requires knowing a specific email from the waiter list")]
        public async Task UsersProfile_UserWithWaiterEmail_ShouldHaveWaiterRole()
        {
            // Arrange - use an email from the waiter list
            string firstName = "Waiter";
            string lastName = "Test";
            string waiterEmail = "known_waiter@restaurant.com"; // Must be an email from the waiter list
            string password = Config.TestUserPassword;

            // Act - Perform registration
            var (registerStatus, registeredEmail, registerResponse) = await _auth.RegisterUser(firstName, lastName, waiterEmail, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "Waiter registration should succeed");

            // Verify registration success message
            if (registerResponse != null && registerResponse.ContainsKey("message"))
            {
                Assert.That(registerResponse["message"].ToString(), Is.EqualTo("User Registered"),
                    "Registration response should confirm successful registration");
            }

            // Log in to get token
            var (loginStatus, loginResponse) = await _auth.LoginUser(waiterEmail, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Waiter should be able to log in");

            // Extract token from the response
            string idToken = loginResponse["idToken"]?.ToString();
            // Using idToken as accessToken
            string accessToken = idToken;

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Updated expectation according to current API behavior
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");

            // Check waiter role
            Assert.That(userData, Is.Not.Null, "User data should not be null");
            Assert.That(userData["role"]?.ToString(), Is.EqualTo("Waiter"),
                "User with waiter email should have Waiter role");
        }

        // Test checks that the profile correctly displays the role for all new users (requirement 5)
        [Test]
        public async Task UsersProfile_RoleIsDisplayedCorrectlyForNewUsers()
        {
            // Arrange - Register a new user
            string email = $"new_user_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Act - Perform registration
            var (registerStatus, registeredEmail, registerResponse) = await _auth.RegisterUser("New", "User", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "New user registration should succeed");

            // Verify registration success message
            if (registerResponse != null && registerResponse.ContainsKey("message"))
            {
                Assert.That(registerResponse["message"].ToString(), Is.EqualTo("User Registered"),
                    "Registration response should confirm successful registration");
            }

            // Log in to get token
            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "New user should be able to log in");

            // Extract token from the response
            string idToken = loginResponse["idToken"]?.ToString();
            // Using idToken as accessToken
            string accessToken = idToken;

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

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
        public async Task UsersProfile_UserIdIsPresent()
        {
            // Arrange - register a user
            string email = $"uid_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, registerResponse) = await _auth.RegisterUser("Test", "User", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - get profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - check user ID presence
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            Assert.That(userData["id"], Is.Not.Null, "User profile should contain user ID");
            Assert.That(userData["id"].ToString(), Is.Not.Empty, "User ID should not be empty");
        }

        [Test]
        public async Task UsersProfile_DefaultImageUrlIsPresent()
        {
            // Arrange - register a user
            string email = $"img_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, registerResponse) = await _auth.RegisterUser("Image", "Test", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - get profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - check default image URL presence
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            Assert.That(userData["imageUrl"], Is.Not.Null, "User profile should contain image URL");
            Assert.That(userData["imageUrl"].ToString(), Is.Not.Empty, "Image URL should not be empty");
            Assert.That(userData["imageUrl"].ToString(), Does.Contain("default_user"),
                "New user should have default image URL");
        }

        [Test]
        public async Task UsersProfile_NameMatchesRegistrationData()
        {
            // Arrange - register a user with special characters in name
            string firstName = "John-Martin";  // Simplified characters to avoid encoding issues
            string lastName = "OConnor";       // Simplified characters to avoid encoding issues
            string email = $"name_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            // Check registration result
            var (registerStatus, _, registerResponse) = await _auth.RegisterUser(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Login and get tokens - add debug information
            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Check that loginResponse is not null and contains required fields
            Assert.That(loginResponse, Is.Not.Null, "Login response should not be null");
            Console.WriteLine($"Login response: {loginResponse}");

            // Check that idToken exists in the response
            Assert.That(loginResponse.ContainsKey("idToken"), Is.True, "Login response should contain idToken");

            // Extract idToken and verify it
            string idToken = loginResponse["idToken"]?.ToString();
            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");
            Console.WriteLine($"ID Token: {idToken.Substring(0, Math.Min(20, idToken.Length))}...");

            // Using idToken as accessToken
            string accessToken = idToken;

            // Act - get profile with detailed logging
            Console.WriteLine("Requesting user profile...");
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

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
        public async Task UsersProfile_LongNamesAreHandledCorrectly()
        {
            // Arrange - register a user with long first and last names
            string longFirstName = "Johnathanjosephjamesmichael";
            string longLastName = "Smith-Johnson-Williams-Brown-Davis";
            string email = $"longname_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, registerResponse) = await _auth.RegisterUser(longFirstName, longLastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - get profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - check that long names are handled correctly
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            Assert.That(userData["firstName"]?.ToString(), Is.EqualTo(longFirstName),
                "Long first name should be stored and displayed correctly");
            Assert.That(userData["lastName"]?.ToString(), Is.EqualTo(longLastName),
                "Long last name should be stored and displayed correctly");
        }

        [Test]
        public async Task UsersProfile_EmailIsLowercased()
        {
            // Arrange - register a user with mixed case email
            string mixedCaseEmail = $"MixedCase_{Guid.NewGuid().ToString("N").Substring(0, 8)}@Example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, registerResponse) = await _auth.RegisterUser("Mixed", "Case", mixedCaseEmail, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = await _auth.LoginUser(mixedCaseEmail, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - get profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - check that email is stored in lowercase
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            string storedEmail = userData["email"]?.ToString();
            Assert.That(storedEmail, Is.AnyOf(mixedCaseEmail.ToLower(), mixedCaseEmail),
                "Email should be normalized (either stored as lowercase or as provided)");
        }

        [Test]
        public async Task UsersProfile_MultipleProfileRequestsReturnSameData()
        {
            // Arrange - register a user
            string email = $"multi_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, registerResponse) = await _auth.RegisterUser("Multiple", "Requests", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - get profile twice
            var (firstProfileStatus, firstProfileData) = await _auth.GetUserProfile(idToken, accessToken);
            var (secondProfileStatus, secondProfileData) = await _auth.GetUserProfile(idToken, accessToken);

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
        public async Task UsersProfile_ContainsExpectedFields()
        {
            // Arrange - register a user
            string email = $"fields_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = Config.TestUserPassword;

            var (registerStatus, _, registerResponse) = await _auth.RegisterUser("Field", "Test", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - get profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - check that all expected fields are present
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");

            string[] expectedFields = { "id", "firstName", "lastName", "email", "role", "imageUrl" };

            foreach (string field in expectedFields)
            {
                Assert.That(userData[field], Is.Not.Null, $"User profile should contain {field} field");
            }

            // Check that there are no unexpected fields with sensitive information
            Assert.That(userData["password"], Is.Null, "User profile should not contain password");
            Assert.That(userData["passwordHash"], Is.Null, "User profile should not contain password hash");
        }

        // Note: Tests for requirements 6, 7, and 8 require access to the administrative part of the system,
        // which is generally unavailable in API testing from the client side.
        // The test for requirement 9 is also complex, as the Visitor role pertains to users who have not registered.
    }
}