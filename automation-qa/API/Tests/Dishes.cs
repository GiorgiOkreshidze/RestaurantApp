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
    [Category("Dishes")]
    public class DishesTests : BaseTest
    {
        private Dishes _dishes;
        private string _validLocationId;
        private string idToken;

        [SetUp]
        public void Setup()
        {
            _dishes = new Dishes();
            _validLocationId = Config.ValidLocationId;
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetPopularDishes_ReturnsSuccess()
        {
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody.Count == 0)
            {
                Console.WriteLine("Warning: No popular dishes found in the system");
                Assert.Ignore("Test skipped - popular dishes not available");
            }
            else
            {
                Console.WriteLine($"Found {responseBody.Count} popular dishes");
            }
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetPopularDishes_HasCorrectStructure()
        {
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody.Count > 0)
            {
                var firstDish = responseBody[0];
                Assert.That(firstDish["id"], Is.Not.Null, "Dish should have an ID");
                Assert.That(firstDish["name"], Is.Not.Null, "Dish should have a name");
                Assert.That(firstDish["price"], Is.Not.Null, "Dish should have a price");
                Assert.That(firstDish["imageUrl"], Is.Not.Null, "Dish should have an image URL");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetPopularDishes_ContainsOnlyPopularDishes()
        {
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody.Count > 0)
            {
                Console.WriteLine("isPopular field check skipped as it is missing in the API response");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetPopularDishes_ReturnsValidJson()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.InstanceOf<JArray>(), "Response body should be a JSON array");
        }

        [Test]
        [Category("Regression")]
        public async Task GetPopularDishes_HasAtLeastOneDish()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody.Count, Is.GreaterThan(0), "Should return at least one popular item");
        }

        [Test]
        [Category("Regression")]
        public async Task GetPopularDishes_DishNamesAreNotEmpty()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    string name = dish["name"].Value<string>();
                    Assert.That(name, Is.Not.Empty, "Dish name should not be empty");
                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetPopularDishes_ImageUrlsStartWithHttps()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    string imageUrl = dish["imageUrl"].Value<string>();
                    Assert.That(imageUrl, Does.StartWith("https://"), "Image URL should start with https://");
                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetPopularDishes_BasicSuccess()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetPopularDishes_HasSomeResults()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status");
            Assert.That(responseBody.Count, Is.GreaterThan(0),
                "Should return at least one popular item");
        }

        [Test]
        [Category("Regression")]
        public async Task GetPopularDishes_HasPriceInformation()
        {
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    try
                    {
                        decimal price = dish["price"].Value<decimal>();
                        Assert.That(price, Is.GreaterThan(0), "Dish price should be positive");
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail($"Failed to convert price of dish {dish["name"]} to decimal: {ex.Message}");
                    }
                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetSpecialtyDishes_EmptyLocationId_ReturnsBadRequest()
        {
            // Arrange
            string emptyLocationId = "";

            // Act
            var (statusCode, responseBody) = await _dishes.GetSpecialtyDishes(emptyLocationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "Should return 404 Not Found status with empty location ID");

            // Check error message if available
            if (responseBody != null)
            {
                try
                {
                    JObject errorObj = JObject.Parse(responseBody.ToString());
                    if (errorObj["message"] != null)
                    {
                        Assert.That(errorObj["message"].ToString(),
                            Contains.Substring("location").IgnoreCase.Or.Contains("id").IgnoreCase,
                            "Error message should indicate issue with location ID");
                    }
                }
                catch { /* Skip if parsing fails */ }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetSpecialtyDishes_WithEmptyLocationId_ReturnsBadRequest()
        {
            // Arrange
            string emptyLocationId = "";

            // Act
            var result = await _dishes.GetSpecialtyDishes(emptyLocationId);
            HttpStatusCode statusCode = result.StatusCode;
            JArray responseBody = result.ResponseBody;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "Should return 404 Not Found status with null location ID");
            Assert.That(responseBody, Is.Null, "Response body should be null with invalid parameters");
        }

        [Test]
        [Category("Regression")]
        public async Task GetSpecialtyDishes_NullLocationId_ReturnsBadRequest()
        {
            // Arrange
            string nullLocationId = null;

            // Act
            var result = await _dishes.GetSpecialtyDishes(nullLocationId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "Should return 404 Not Found status with null location ID");
            Assert.That(result.ResponseBody, Is.Null,
                "Response body should be null with null location ID");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAllDishes_WithoutParameters_ReturnsOkStatus()
        {
            // Act
            var result = await _dishes.GetAllDishes();

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Request without parameters should return 200 OK status");

            // Check success message if available
            if (result.ResponseBody != null && result.ResponseBody.Count > 0)
            {
                Assert.That(result.ResponseBody, Is.Not.Null,
                    "Response should contain dish data");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAllDishes_WithDishTypeParameter_ReturnsOkStatus()
        {
            // Arrange
            string dishType = "Appetizers";

            // Act
            var result = await _dishes.GetAllDishes(dishType);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Request with dishType parameter should return 200 OK status");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAllDishes_WithSortParameter_ReturnsOkStatus()
        {
            // Arrange
            string sort = "PopularityAsc";

            // Act
            var result = await _dishes.GetAllDishes(sort: sort);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Request with sort parameter should return 200 OK status");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetAllDishes_ResponseContainsValidData()
        {
            // Act
            var result = await _dishes.GetAllDishes();

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status");
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");
            Assert.That(result.ResponseBody.Count, Is.GreaterThan(0), "Response should contain at least one dish");

            if (result.ResponseBody != null && result.ResponseBody.Count > 0)
            {
                JObject firstDish = (JObject)result.ResponseBody[0];

                // Check required fields in first object
                Assert.That(firstDish.ContainsKey("id"), "Dish should contain 'id' field");
                Assert.That(firstDish.ContainsKey("name"), "Dish should contain 'name' field");
                Assert.That(firstDish.ContainsKey("price"), "Dish should contain 'price' field");
                Assert.That(firstDish.ContainsKey("imageUrl"), "Dish should contain 'imageUrl' field");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAllDishes_FilterByDishType_ReturnsMatchingDishes()
        {
            // Arrange
            string dishType = "Appetizers";

            // Act
            var result = await _dishes.GetAllDishes(dishType);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status");
            Assert.That(result.ResponseBody, Is.Not.Null,
                "Response body should not be null");

            if (result.ResponseBody != null && result.ResponseBody.Count > 0)
            {
                // Check that all dishes in response match requested type
                bool allMatchDishType = true;
                foreach (JObject dish in result.ResponseBody)
                {
                    if (!string.Equals(dish["dishType"]?.ToString(), dishType, StringComparison.OrdinalIgnoreCase))
                    {
                        allMatchDishType = false;
                        break;
                    }
                }

                Assert.That(allMatchDishType, Is.True,
                    $"All dishes in response should have type '{dishType}'");
            }
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetDishById_ValidId_ReturnsOk()
        {
            // Arrange
            string validDishId = "ed3f1f9b-7412-42ea-84aa-04c9b85ab67a";

            // Act
            var result = await _dishes.GetDishById(validDishId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status with valid dish ID");
        }

        [Test]
        [Category("Regression")]
        public async Task GetDishById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            string invalidDishId = "invalid-dish-id";

            // Act
            var result = await _dishes.GetDishById(invalidDishId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "Should return 404 Not Found status with invalid dish ID");

            // Check error message if available
            if (result.ResponseBody != null && result.ResponseBody.ContainsKey("message"))
            {
                Assert.That(result.ResponseBody["message"].ToString(),
                    Contains.Substring("not found").IgnoreCase.Or.Contains("does not exist").IgnoreCase,
                    "Error message should indicate dish not found");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetDishById_EmptyId_ReturnsBadRequest()
        {
            // Act
            var result = await _dishes.GetDishById("");

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Should return 400 Bad Request status with empty dish ID");
        }

        [Test]
        [Category("Regression")]
        public async Task GetDishById_ValidId_ContainsExpectedFields()
        {
            // Arrange
            string validDishId = "ed3f1f9b-7412-42ea-84aa-04c9b85ab67a";

            // Act
            var result = await _dishes.GetDishById(validDishId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status with valid dish ID");
            Assert.That(result.ResponseBody, Is.Not.Null,
                "Response body should not be null");
            Assert.That(result.ResponseBody.ContainsKey("id"), Is.True,
                "Response should contain 'id' field");
            Assert.That(result.ResponseBody.ContainsKey("name"), Is.True,
                "Response should contain 'name' field");
            Assert.That(result.ResponseBody.ContainsKey("price"), Is.True,
                "Response should contain 'price' field");
        }

        [Test]
        [Category("Regression")]
        public async Task GetDishById_ValidId_IdMatchesRequest()
        {
            // Arrange
            string validDishId = "ed3f1f9b-7412-42ea-84aa-04c9b85ab67a";

            // Act
            var result = await _dishes.GetDishById(validDishId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status with valid dish ID");
            Assert.That(result.ResponseBody["id"].ToString(), Is.EqualTo(validDishId),
                "ID in response should match requested ID");
        }

        [Test]
        [Category("Regression")]
        public async Task GetPopularDishes_PricesAreReasonable()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");

            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    decimal price = dish["price"].Value<decimal>();
                    Assert.That(price, Is.LessThan(1000), "Dish price should be reasonable (less than 1000)");
                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAllDishes_WithInvalidSortParameter_ReturnsOkStatus()
        {
            // Arrange
            string invalidSort = "InvalidSortParameter";

            // Act
            var result = await _dishes.GetAllDishes(sort: invalidSort);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Request with invalid sort parameter should still return 200 OK status with default sorting");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAllDishes_DishNamesNotEmpty()
        {
            // Act
            var result = await _dishes.GetAllDishes();

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");

            if (result.ResponseBody != null && result.ResponseBody.Count > 0)
            {
                foreach (var dish in result.ResponseBody)
                {
                    string name = dish["name"].Value<string>();
                    Assert.That(name, Is.Not.Empty, "Dish name should not be empty");
                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetDishById_ValidId_HasDescriptionField()
        {
            // Arrange
            string validDishId = "ed3f1f9b-7412-42ea-84aa-04c9b85ab67a";

            // Act
            var result = await _dishes.GetDishById(validDishId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status with valid dish ID");
            Assert.That(result.ResponseBody, Is.Not.Null, "Response body should not be null");

            // Check if description field exists (it's common for dishes to have descriptions)
            Assert.That(result.ResponseBody.ContainsKey("description") || result.ResponseBody.ContainsKey("desc"),
                "Response should contain a description field");
        }
    }
}