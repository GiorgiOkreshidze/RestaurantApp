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
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody.Count == 0)
            {
                Console.WriteLine("Warning: No popular dishes found in the system");
                Assert.Ignore("Test skipped - no popular dishes available");
            }
            else
            {
                Console.WriteLine($"Found {responseBody.Count} popular dishes");
            }
        }

        [Test]
        public async Task GetPopularDishes_HasCorrectStructure()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody.Count > 0)
            {
                var firstDish = responseBody[0];

                Assert.That(firstDish["id"], Is.Not.Null, "Dish should have an ID");
                Assert.That(firstDish["name"], Is.Not.Null, "Dish should have a name");
                Assert.That(firstDish["price"], Is.Not.Null, "Dish should have a price");
                Assert.That(firstDish["imageUrl"], Is.Not.Null, "Dish should have an image URL");
                Assert.That(firstDish["isPopular"], Is.Not.Null, "Dish should have isPopular flag");
                Assert.That(firstDish["locationId"], Is.Not.Null, "Dish should have a location ID");
            }
        }

        [Test]
        public async Task GetPopularDishes_ContainsOnlyPopularDishes()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    bool isPopular = dish["isPopular"].Value<bool>();
                    Assert.That(isPopular, Is.True, "All dishes in popular dishes endpoint should be marked as popular");
                }
            }
        }

        [Test]
        public async Task GetPopularDishes_HasPriceInformation()
        {
            // Act
            var (statusCode, responseBody) = await _dishes.GetPopularDishes();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody.Count > 0)
            {
                foreach (var dish in responseBody)
                {
                    // Check that the price is specified and is a positive number
                    decimal price = dish["price"].Value<decimal>();
                    Assert.That(price, Is.GreaterThan(0), "Dish price should be a positive value");
                }
            }
        }
    }
}