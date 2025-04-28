using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using ApiTests.Pages;
using ApiTests.Utilities;
using Newtonsoft.Json.Linq;

namespace ApiTests
{
    [TestFixture]
    [Category("Cart")]
    public class CartTests : BaseTest
    {
        private Cart _cart;
        private Authentication _auth;
        private string _testDishId;
        private string _userToken;

        [SetUp]
        public async Task Setup()
        {
            _cart = new Cart();
            _auth = new Authentication();
            _testDishId = "1234abcd-5678-90ef-ghij-klmnopqrstuv";

            var (loginStatus, loginResponse) = _auth.LoginUserWithCurl(
                Config.TestUserEmail,
                Config.TestUserPassword);

            if (loginStatus == HttpStatusCode.OK && loginResponse != null)
            {
                _userToken = loginResponse["accessToken"].ToString();
            }
            else
            {
                Assert.Fail("Could not obtain authentication token for tests");
            }
        }

        [Test]
        [Category("Smoke")]
        public async Task GetCart_WithValidToken_ShouldReturnSuccess()
        {
            // Act
            var (statusCode, responseBody) = await _cart.GetCart(_userToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Getting cart with valid token should return OK status");
            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
            Assert.That(responseBody["content"], Is.Not.Null,
                "Response should contain 'content' field");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_WithoutToken_ShouldReturnUnauthorized()
        {
            // Act & Assert
            Assert.ThrowsAsync<System.ArgumentNullException>(async () =>
                await _cart.GetCart(null),
                "Getting cart without token should throw ArgumentNullException");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_ShouldSucceed()
        {
            // Act
            var (statusCode, responseBody) = await _cart.GetCart(_userToken);

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody?.ToString() ?? "No response"}");
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Getting cart should succeed");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_ShouldReturnIsEmptyField()
        {
            // Act
            var (statusCode, responseBody) = await _cart.GetCart(_userToken);

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody?.ToString() ?? "No response"}");
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Getting cart should succeed");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody["isEmpty"], Is.Not.Null, "Response should contain 'isEmpty' field");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_ShouldReturnContentArray()
        {
            // Act
            var (statusCode, responseBody) = await _cart.GetCart(_userToken);

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody?.ToString() ?? "No response"}");
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Getting cart should succeed");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody["content"], Is.Not.Null, "Response should contain 'content' field");
            Assert.That(responseBody["content"].Type, Is.EqualTo(JTokenType.Array), "'content' should be an array");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_ShouldReturnStatus200()
        {
            // Act
            var (statusCode, _) = await _cart.GetCart(_userToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Getting cart should return 200 OK");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_ShouldReturnNonEmptyResponse()
        {
            // Act
            var (statusCode, responseBody) = await _cart.GetCart(_userToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Getting cart should succeed");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_ShouldContainContentField()
        {
            // Act
            var (statusCode, responseBody) = await _cart.GetCart(_userToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Getting cart should succeed");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody["content"], Is.Not.Null, "Response should contain 'content' field");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_IsEmptyShouldBeBoolean()
        {
            // Act
            var (statusCode, responseBody) = await _cart.GetCart(_userToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Getting cart should succeed");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody["isEmpty"].Type, Is.EqualTo(JTokenType.Boolean), "'isEmpty' should be a boolean");
        }

        [Test]
        [Category("Regression")]
        public async Task GetCart_ShouldContainExpectedKeys()
        {
            // Act
            var (statusCode, responseBody) = await _cart.GetCart(_userToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Getting cart should succeed");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody["content"], Is.Not.Null, "Response should contain 'content' field");
            Assert.That(responseBody["isEmpty"], Is.Not.Null, "Response should contain 'isEmpty' field");
        }
    }
}
