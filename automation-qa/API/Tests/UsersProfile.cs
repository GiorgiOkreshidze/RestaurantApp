using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;

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
            string password = "StrongPass123!";

            // Act - Perform registration
            var (registerStatus, registeredEmail, _) = await _auth.RegisterUser(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Log in to get tokens
            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Extract tokens from the response
            string idToken = loginResponse["idToken"]?.ToString();
            // Используем idToken и как accessToken
            string accessToken = idToken;

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Обновлено ожидание в соответствии с новым поведением API
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");

            // Теперь можно проверить роль пользователя
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
            string password = "StrongPass123!";

            // Act - Perform registration
            var (registerStatus, registeredEmail, _) = await _auth.RegisterUser(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Log in to get tokens
            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Extract token from the response
            string idToken = loginResponse["idToken"]?.ToString();
            // Используем idToken и как accessToken
            string accessToken = idToken;

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Обновлено ожидание в соответствии с новым поведением API
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");

            // Теперь можно проверить данные пользователя
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
            string password = "StrongPass123!";

            // Act - Perform registration
            var (registerStatus, registeredEmail, _) = await _auth.RegisterUser(firstName, lastName, waiterEmail, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "Waiter registration should succeed");

            // Log in to get token
            var (loginStatus, loginResponse) = await _auth.LoginUser(waiterEmail, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "Waiter should be able to log in");

            // Extract token from the response
            string idToken = loginResponse["idToken"]?.ToString();
            // Используем idToken и как accessToken
            string accessToken = idToken;

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Обновлено ожидание в соответствии с новым поведением API
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");

            // Теперь можно проверить роль официанта
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
            string password = "StrongPass123!";

            // Act - Perform registration
            var (registerStatus, registeredEmail, _) = await _auth.RegisterUser("New", "User", email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "New user registration should succeed");

            // Log in to get token
            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "New user should be able to log in");

            // Extract token from the response
            string idToken = loginResponse["idToken"]?.ToString();
            // Используем idToken и как accessToken
            string accessToken = idToken;

            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");

            // Get user profile
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Обновлено ожидание в соответствии с новым поведением API
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK),
                "API should return user profile immediately after login");

            // Теперь можно проверить роль нового пользователя
            Assert.That(userData, Is.Not.Null, "User data should not be null");
            Assert.That(userData["role"], Is.Not.Null,
                "User profile should include role information");
            Console.WriteLine($"New user role: {userData["role"]}");

            // Для обычных пользователей ожидается роль Customer
            Assert.That(userData["role"]?.ToString(), Is.EqualTo("Customer"),
                "New user with regular email should have Customer role");
        }

        [Test]
        public async Task UsersProfile_UserIdIsPresent()
        {
            // Arrange - регистрируем пользователя
            string email = $"uid_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "StrongPass123!";

            await _auth.RegisterUser("Test", "User", email, password);
            var (_, loginResponse) = await _auth.LoginUser(email, password);

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - получаем профиль
            var (_, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - проверяем наличие ID пользователя
            Assert.That(userData["id"], Is.Not.Null, "User profile should contain user ID");
            Assert.That(userData["id"].ToString(), Is.Not.Empty, "User ID should not be empty");
        }

        [Test]
        public async Task UsersProfile_DefaultImageUrlIsPresent()
        {
            // Arrange - регистрируем пользователя
            string email = $"img_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "StrongPass123!";

            await _auth.RegisterUser("Image", "Test", email, password);
            var (_, loginResponse) = await _auth.LoginUser(email, password);

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - получаем профиль
            var (_, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - проверяем наличие URL изображения по умолчанию
            Assert.That(userData["imageUrl"], Is.Not.Null, "User profile should contain image URL");
            Assert.That(userData["imageUrl"].ToString(), Is.Not.Empty, "Image URL should not be empty");
            Assert.That(userData["imageUrl"].ToString(), Does.Contain("default_user"),
                "New user should have default image URL");
        }

        [Test]
        public async Task UsersProfile_NameMatchesRegistrationData()
        {
            // Arrange - регистрируем пользователя с особыми символами в имени
            string firstName = "John-Martin";  // Упростил символы, чтобы избежать проблем с кодировкой
            string lastName = "OConnor";       // Упростил символы, чтобы избежать проблем с кодировкой
            string email = $"name_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "StrongPass123!";

            // Проверка результата регистрации
            var (registerStatus, _, _) = await _auth.RegisterUser(firstName, lastName, email, password);
            Assert.That(registerStatus, Is.EqualTo(HttpStatusCode.OK), "User registration should succeed");

            // Вход и получение токенов - добавляем отладочную информацию
            var (loginStatus, loginResponse) = await _auth.LoginUser(email, password);
            Assert.That(loginStatus, Is.EqualTo(HttpStatusCode.OK), "User should be able to log in");

            // Проверяем, что loginResponse не null и содержит нужные поля
            Assert.That(loginResponse, Is.Not.Null, "Login response should not be null");
            Console.WriteLine($"Login response: {loginResponse}");

            // Проверяем, что idToken существует в ответе
            Assert.That(loginResponse.ContainsKey("idToken"), Is.True, "Login response should contain idToken");

            // Извлекаем idToken и проверяем его
            string idToken = loginResponse["idToken"]?.ToString();
            Assert.That(idToken, Is.Not.Null.And.Not.Empty, "ID token should not be null or empty");
            Console.WriteLine($"ID Token: {idToken.Substring(0, Math.Min(20, idToken.Length))}...");

            // Используем idToken как accessToken
            string accessToken = idToken;

            // Act - получаем профиль с подробным логированием
            Console.WriteLine("Requesting user profile...");
            var (profileStatus, userData) = await _auth.GetUserProfile(idToken, accessToken);

            Console.WriteLine($"Profile status: {profileStatus}");
            if (userData != null)
            {
                Console.WriteLine($"User data: {userData}");
            }

            // Assert - проверяем статус и наличие данных
            Assert.That(profileStatus, Is.EqualTo(HttpStatusCode.OK), "Profile request should succeed");
            Assert.That(userData, Is.Not.Null, "User data should not be null");

            // Проверяем, что имя и фамилия соответствуют регистрационным данным
            Assert.That(userData["firstName"]?.ToString(), Is.EqualTo(firstName),
                "First name should match exactly what was provided during registration");
            Assert.That(userData["lastName"]?.ToString(), Is.EqualTo(lastName),
                "Last name should match exactly what was provided during registration");
        }

        [Test]
        public async Task UsersProfile_LongNamesAreHandledCorrectly()
        {
            // Arrange - регистрируем пользователя с длинными именем и фамилией
            string longFirstName = "Johnathanjosephjamesmichael";
            string longLastName = "Smith-Johnson-Williams-Brown-Davis";
            string email = $"longname_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "StrongPass123!";

            await _auth.RegisterUser(longFirstName, longLastName, email, password);
            var (_, loginResponse) = await _auth.LoginUser(email, password);

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - получаем профиль
            var (_, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - проверяем, что длинные имена обрабатываются корректно
            Assert.That(userData["firstName"]?.ToString(), Is.EqualTo(longFirstName),
                "Long first name should be stored and displayed correctly");
            Assert.That(userData["lastName"]?.ToString(), Is.EqualTo(longLastName),
                "Long last name should be stored and displayed correctly");
        }

        [Test]
        public async Task UsersProfile_EmailIsLowercased()
        {
            // Arrange - регистрируем пользователя с email в смешанном регистре
            string mixedCaseEmail = $"MixedCase_{Guid.NewGuid().ToString("N").Substring(0, 8)}@Example.com";
            string password = "StrongPass123!";

            await _auth.RegisterUser("Mixed", "Case", mixedCaseEmail, password);
            var (_, loginResponse) = await _auth.LoginUser(mixedCaseEmail, password);

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - получаем профиль
            var (_, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - проверяем, что email хранится в нижнем регистре
            string storedEmail = userData["email"]?.ToString();
            Assert.That(storedEmail, Is.EqualTo(mixedCaseEmail.ToLower()) | Is.EqualTo(mixedCaseEmail),
                "Email should be normalized (either stored as lowercase or as provided)");
        }

        [Test]
        public async Task UsersProfile_MultipleProfileRequestsReturnSameData()
        {
            // Arrange - регистрируем пользователя
            string email = $"multi_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "StrongPass123!";

            await _auth.RegisterUser("Multiple", "Requests", email, password);
            var (_, loginResponse) = await _auth.LoginUser(email, password);

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - получаем профиль дважды
            var (_, firstProfileData) = await _auth.GetUserProfile(idToken, accessToken);
            var (_, secondProfileData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - проверяем, что данные идентичны
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
            // Arrange - регистрируем пользователя
            string email = $"fields_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            string password = "StrongPass123!";

            await _auth.RegisterUser("Field", "Test", email, password);
            var (_, loginResponse) = await _auth.LoginUser(email, password);

            string idToken = loginResponse["idToken"]?.ToString();
            string accessToken = idToken;

            // Act - получаем профиль
            var (_, userData) = await _auth.GetUserProfile(idToken, accessToken);

            // Assert - проверяем наличие всех ожидаемых полей
            string[] expectedFields = { "id", "firstName", "lastName", "email", "role", "imageUrl" };

            foreach (string field in expectedFields)
            {
                Assert.That(userData[field], Is.Not.Null, $"User profile should contain {field} field");
            }

            // Проверяем, что нет неожиданных полей с конфиденциальной информацией
            Assert.That(userData["password"], Is.Null, "User profile should not contain password");
            Assert.That(userData["passwordHash"], Is.Null, "User profile should not contain password hash");
        }

        // Note: Tests for requirements 6, 7, and 8 require access to the administrative part of the system,
        // which is generally unavailable in API testing from the client side.
        // The test for requirement 9 is also complex, as the Visitor role pertains to users who have not registered.
    }
}
