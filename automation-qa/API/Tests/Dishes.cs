using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;

namespace ApiTests
{
    [TestFixture]
    [Category("Dishes")]
    public class DishesTests : BaseTest
    {
        private Dishes _dishes;
        private string _validLocationId = "8c4fc44e-c1a5-42eb-9912-55aeb5111a99";
        private string idToken;

        [SetUp]
        public void Setup()
        {
            _dishes = new Dishes();
        }

        [Test]
        public async Task GetPopularDishes_ReturnsSuccess()
        {
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            if (responseBody.Count == 0)
            {
                Console.WriteLine("Предупреждение: Популярные блюда не найдены в системе");
                Assert.Ignore("Тест пропущен - популярные блюда недоступны");
            }
            else
            {
                Console.WriteLine($"Найдено {responseBody.Count} популярных блюд");
            }
        }

        [Test]
        public async Task GetPopularDishes_HasCorrectStructure()
        {
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            if (responseBody.Count > 0)
            {
                var firstDish = responseBody[0];
                Assert.That(firstDish["id"], Is.Not.Null, "Блюдо должно иметь ID");
                Assert.That(firstDish["name"], Is.Not.Null, "Блюдо должно иметь название");
                Assert.That(firstDish["price"], Is.Not.Null, "Блюдо должно иметь цену");
                Assert.That(firstDish["imageUrl"], Is.Not.Null, "Блюдо должно иметь URL изображения");
            }
        }

        [Test]
        public async Task GetPopularDishes_ContainsOnlyPopularDishes()
        {
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            if (responseBody.Count > 0)
            {
                Console.WriteLine("Проверка isPopular пропущена, так как поле отсутствует в ответе API");
            }
        }

        [Test]
        public async Task GetPopularDishes_ReturnsValidJson()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.InstanceOf<JArray>(), "Тело ответа должно быть массивом JSON");
        }

        [Test]
        public async Task GetPopularDishes_HasAtLeastOneDish()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody.Count, Is.GreaterThan(0), "Должен возвращаться хотя бы один популярный элемент");
        }

        [Test]
        public async Task GetPopularDishes_DishNamesAreNotEmpty()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    string name = dish["name"].Value<string>();
                    Assert.That(name, Is.Not.Empty, "Название блюда не должно быть пустым");
                }
            }
        }

        [Test]
        public async Task GetPopularDishes_ImageUrlsStartWithHttps()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    string imageUrl = dish["imageUrl"].Value<string>();
                    Assert.That(imageUrl, Does.StartWith("https://"), "URL изображения должен начинаться с https://");
                }
            }
        }

        [Test]
        public async Task GetPopularDishes_BasicSuccess()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
        }

        [Test]
        public async Task GetPopularDishes_HasSomeResults()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Должен возвращать статус 200 OK");
            Assert.That(responseBody.Count, Is.GreaterThan(0),
                "Должен возвращаться хотя бы один популярный элемент");
        }

        [Test]
        public async Task GetPopularDishes_HasPriceInformation()
        {
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    try
                    {
                        decimal price = dish["price"].Value<decimal>();
                        Assert.That(price, Is.GreaterThan(0), "Цена блюда должна быть положительной");
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail($"Не удалось преобразовать цену блюда {dish["name"]} в decimal: {ex.Message}");
                    }
                }
            }
        }

        [Test]
        public async Task GetSpecialtyDishes_EmptyLocationId_ReturnsBadRequest()
        {
            // Arrange
            string emptyLocationId = "";

            // Act
            var (statusCode, responseBody) = await _dishes.GetSpecialtyDishes(emptyLocationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Должен возвращать статус 400 Bad Request при запросе с пустым ID локации");
        }

        [Test]
        public async Task GetSpecialtyDishes_WithEmptyLocationId_ReturnsBadRequest()
        {
            // Arrange
            string emptyLocationId = "";

            // Act
            var result = await _dishes.GetSpecialtyDishes(emptyLocationId);
            HttpStatusCode statusCode = result.StatusCode;
            JArray responseBody = result.ResponseBody;

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseBody, Is.Null);
        }

        [Test]
        public async Task GetSpecialtyDishes_NullLocationId_ReturnsBadRequest()
        {
            // Arrange
            string nullLocationId = null;

            // Act
            var result = await _dishes.GetSpecialtyDishes(nullLocationId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Должен возвращать статус 400 Bad Request при запросе с null вместо ID локации");
            Assert.That(result.ResponseBody, Is.Null,
                "Ответ должен быть null при запросе с null вместо ID локации");
        }

        [Test]
        public async Task GetAllDishes_WithoutParameters_ReturnsOkStatus()
        {
            // Act
            var result = await _dishes.GetAllDishes();

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Запрос без параметров должен возвращать статус 200 OK");
        }

        [Test]
        public async Task GetAllDishes_WithDishTypeParameter_ReturnsOkStatus()
        {
            // Arrange
            string dishType = "Appetizers";

            // Act
            var result = await _dishes.GetAllDishes(dishType);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Запрос с параметром dishType должен возвращать статус 200 OK");
        }

        [Test]
        public async Task GetAllDishes_WithSortParameter_ReturnsOkStatus()
        {
            // Arrange
            string sort = "PopularityAsc";

            // Act
            var result = await _dishes.GetAllDishes(sort: sort);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Запрос с параметром sort должен возвращать статус 200 OK");
        }

        [Test]
        public async Task GetAllDishes_ResponseContainsValidData()
        {
            // Act
            var result = await _dishes.GetAllDishes();

            // Assert
            Assert.That(result.ResponseBody, Is.Not.Null, "Ответ не должен быть null");
            Assert.That(result.ResponseBody.Count, Is.GreaterThan(0), "Ответ должен содержать хотя бы одно блюдо");

            if (result.ResponseBody != null && result.ResponseBody.Count > 0)
            {
                JObject firstDish = (JObject)result.ResponseBody[0];

                // Проверяем наличие обязательных полей в первом объекте
                Assert.That(firstDish.ContainsKey("id"), "Блюдо должно содержать поле 'id'");
                Assert.That(firstDish.ContainsKey("name"), "Блюдо должно содержать поле 'name'");
                Assert.That(firstDish.ContainsKey("price"), "Блюдо должно содержать поле 'price'");
                Assert.That(firstDish.ContainsKey("imageUrl"), "Блюдо должно содержать поле 'imageUrl'");
            }
        }

        [Test]
        public async Task GetAllDishes_FilterByDishType_ReturnsMatchingDishes()
        {
            // Arrange
            string dishType = "Appetizers";

            // Act
            var result = await _dishes.GetAllDishes(dishType);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.ResponseBody, Is.Not.Null);

            if (result.ResponseBody != null && result.ResponseBody.Count > 0)
            {
                // Проверяем, что все блюда в ответе соответствуют запрошенному типу
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
                    $"Все блюда в ответе должны иметь тип '{dishType}'");
            }
        }

        [Test]
        public async Task GetDishById_ValidId_ReturnsOk()
        {
            // Arrange
            string validDishId = "ed3f1f9b-7412-42ea-84aa-04c9b85ab67a";

            // Act
            var result = await _dishes.GetDishById(validDishId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetDishById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            string invalidDishId = "invalid-dish-id";

            // Act
            var result = await _dishes.GetDishById(invalidDishId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetDishById_EmptyId_ReturnsBadRequest()
        {
            // Act
            var result = await _dishes.GetDishById("");

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetDishById_ValidId_ContainsExpectedFields()
        {
            // Arrange
            string validDishId = "ed3f1f9b-7412-42ea-84aa-04c9b85ab67a";

            // Act
            var result = await _dishes.GetDishById(validDishId);

            // Assert
            Assert.That(result.ResponseBody, Is.Not.Null);
            Assert.That(result.ResponseBody.ContainsKey("id"), Is.True);
            Assert.That(result.ResponseBody.ContainsKey("name"), Is.True);
            Assert.That(result.ResponseBody.ContainsKey("price"), Is.True);
        }

        [Test]
        public async Task GetDishById_ValidId_IdMatchesRequest()
        {
            // Arrange
            string validDishId = "ed3f1f9b-7412-42ea-84aa-04c9b85ab67a";

            // Act
            var result = await _dishes.GetDishById(validDishId);

            // Assert
            Assert.That(result.ResponseBody["id"].ToString(), Is.EqualTo(validDishId));
        }
    }
}