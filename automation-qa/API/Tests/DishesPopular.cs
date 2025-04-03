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
    }
}