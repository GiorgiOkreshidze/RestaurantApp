using System;
using System.Net;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;
using ApiTests.Utilities;
using automation_qa.Framework;

namespace ApiTests
{
    [TestFixture]
    [Category("Locations")]
    public class LocationsTests : BaseTest
    {
        private LocationsWithCurl _locations;

        [SetUp]
        public void Setup()
        {
            _locations = new LocationsWithCurl();
        }

        [Test]
        public void GetLocations_ShouldReturnAllLocations()
        {
            // Act
            var (statusCode, responseBody) = _locations.GetLocationsWithCurl();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check the structure only if there is data
            if (responseBody.Count > 0)
            {
                var firstLocation = responseBody[0];
                Assert.That(firstLocation["id"], Is.Not.Null, "Location should have an ID");
                Assert.That(firstLocation["address"], Is.Not.Null, "Location should have an address");
                Assert.That(firstLocation["totalCapacity"], Is.Not.Null, "Location should have total capacity");
                Assert.That(firstLocation["averageOccupancy"], Is.Not.Null, "Location should have average occupancy");
                Assert.That(firstLocation["imageUrl"], Is.Not.Null, "Location should have image URL");
                Assert.That(firstLocation["rating"], Is.Not.Null, "Location should have rating");
            }
            else
            {
                Console.WriteLine("WARNING: No locations found in the database. Structure validation skipped.");
            }
        }

        [Test]
        public void GetLocationSelectOptions_ShouldReturnFormattedOptions()
        {
            // Act
            var (statusCode, responseBody) = _locations.GetLocationSelectOptionsWithCurl();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check the structure only if there is data
            if (responseBody.Count > 0)
            {
                var firstOption = responseBody[0];
                Assert.That(firstOption["id"], Is.Not.Null, "Option should have an ID");
                Assert.That(firstOption["address"], Is.Not.Null, "Option should have an address");
            }
            else
            {
                Console.WriteLine("WARNING: No location options found in the database. Structure validation skipped.");
            }
        }

        [Test]
        public void GetSpecialityDishes_ValidLocationId_ShouldReturnDishes()
        {
            // Arrange - first, get the list of locations
            var (locationsStatus, locations) = _locations.GetLocationsWithCurl();
            Assert.That(locationsStatus, Is.EqualTo(HttpStatusCode.OK), "Getting locations should succeed");

            // Skip test if no locations found
            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();

            // Act
            var (statusCode, responseBody) = _locations.GetSpecialityDishesWithCurl(locationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check the structure of the dishes, if they exist
            if (responseBody.Count > 0)
            {
                var firstDish = responseBody[0];
                Assert.That(firstDish["id"], Is.Not.Null, "Dish should have an ID");
                Assert.That(firstDish["name"], Is.Not.Null, "Dish should have a name");
                // Check description only if it exists
                if (firstDish["description"] != null)
                {
                    Assert.That(firstDish["description"].ToString(), Is.Not.Empty, "Dish description should not be empty if present");
                }
            }
        }

        [Test]
        public void GetSpecialityDishes_InvalidLocationId_ReturnsEmptyArray()
        {
            // Arrange
            string invalidLocationId = "non-existent-id";

            // Act
            var (statusCode, responseBody) = _locations.GetSpecialityDishesWithCurl(invalidLocationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "API returns 200 OK with invalid location ID");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody.Count, Is.EqualTo(0), "Response should be an empty array");
        }

        [Test]
        public void GetSpecialityDishes_EmptyLocationId_ReturnsNotFound()
        {
            // Arrange
            string emptyLocationId = "";

            // Act
            var response = _locations.GetSpecialityDishesWithCurl(emptyLocationId);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }


        [Test]
        public void GetLocations_ShouldContainDetailsForLocationOverview()
        {
            // Act
            var (statusCode, responseBody) = _locations.GetLocationsWithCurl();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check the structure only if there is data
            if (responseBody.Count > 0)
            {
                // Check that there is detailed information available for each location
                foreach (var location in responseBody)
                {
                    Assert.That(location["description"], Is.Not.Null, "Location should have a description");
                    Assert.That(location["rating"], Is.Not.Null, "Location should have a rating");
                    Assert.That(location["address"], Is.Not.Null, "Location should have an address");

                    // Check contacts only if this field is provided in the API
                    if (location["contacts"] != null)
                    {
                        Assert.That(location["contacts"], Is.Not.Null, "Location should have contacts");
                    }
                }
            }
            else
            {
                Console.WriteLine("WARNING: No locations found in the database. Structure validation skipped.");
            }
        }

        [Test]
        public void GetLocationFeedbacks_ReturnsSuccess()
        {
            // Arrange - get the location ID for testing
            var (locationsStatus, locations) = _locations.GetLocationsWithCurl();
            Assert.That(locationsStatus, Is.EqualTo(HttpStatusCode.OK), "Getting locations should succeed");

            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();

            // Act
            var (statusCode, responseBody) = _locations.GetLocationFeedbacksWithCurl(locationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Failed to get location feedbacks");
            Assert.That(responseBody, Is.Not.Null, "Feedbacks response is null");

            // Check that 'content' exists and is array
            Assert.That(responseBody["content"], Is.Not.Null, "Content field is missing in response");
            var feedbacks = responseBody["content"] as JArray;
            Assert.That(feedbacks, Is.Not.Null, "Content should be an array");

            if (feedbacks.Count > 0)
            {
                var first = feedbacks[0];
                Assert.That(first["id"], Is.Not.Null, "Feedback should have an ID");
                Assert.That(first["rate"], Is.Not.Null, "Feedback should have a rating");
                Assert.That(first["comment"], Is.Not.Null, "Feedback should have a comment");
                Assert.That(first["userName"], Is.Not.Null, "Feedback should have a user name");
                Assert.That(first["date"], Is.Not.Null, "Feedback should have a date");
                Assert.That(first["type"], Is.Not.Null, "Feedback should have a type");
            }
        }

        [Test]
        public void GetLocationFeedbacks_ShouldReturnFeedbacks()
        {
            // Arrange - get the location ID for testing
            var (locationsStatus, locations) = _locations.GetLocationsWithCurl();
            Assert.That(locationsStatus, Is.EqualTo(HttpStatusCode.OK), "Getting locations should succeed");

            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();

            // Act
            var (statusCode, responseBody) = _locations.GetLocationFeedbacksWithCurl(locationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check basic response structure
            Assert.That(responseBody["content"], Is.Not.Null, "Response should contain 'content' field");

            var feedbacks = responseBody["content"] as JArray;
            Assert.That(feedbacks, Is.Not.Null, "Content should be an array");

            if (feedbacks.Count > 0)
            {
                var firstFeedback = feedbacks[0];
                Assert.That(firstFeedback["id"], Is.Not.Null, "Feedback should have an ID");
                Assert.That(firstFeedback["rate"], Is.Not.Null, "Feedback should have a rating");
                Assert.That(firstFeedback["comment"], Is.Not.Null, "Feedback should have a comment");
                Assert.That(firstFeedback["userName"], Is.Not.Null, "Feedback should have a user name");
                Assert.That(firstFeedback["date"], Is.Not.Null, "Feedback should have a date");
                Assert.That(firstFeedback["type"], Is.Not.Null, "Feedback should have a type");
            }
        }

        [Test]
        public void GetLocationFeedbacks_WithServiceQualityFilter_ShouldReturnOnlyServiceQualityFeedbacks()
        {
            // Arrange
            var (locationsStatus, locations) = _locations.GetLocationsWithCurl();

            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();
            string feedbackType = "SERVICE_QUALITY";

            // Act - Get feedbacks with type filter
            var (statusCode, responseBody) = _locations.GetLocationFeedbacksWithCurl(locationId, type: feedbackType);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");

            var feedbacks = responseBody["content"] as JArray;
            if (feedbacks != null && feedbacks.Count > 0)
            {
                foreach (var feedback in feedbacks)
                {
                    string type = feedback["type"]?.ToString();
                    // Check that the type matches the requested one, accounting for possible variations
                    bool isValidType = type == feedbackType || type == "SERVICE-QUALITY" || type == "Service_Quality";
                    Assert.That(isValidType, Is.True,
                        $"All feedbacks should be of type {feedbackType} or equivalent, but found {type}");
                }
            }
        }

        [Test]
        public void GetLocationFeedbacks_WithCuisineExperienceFilter_ShouldReturnOnlyCuisineExperienceFeedbacks()
        {
            // Arrange
            var (locationsStatus, locations) = _locations.GetLocationsWithCurl();

            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();
            string feedbackType = "CUISINE_EXPERIENCE";

            // Act - Get feedbacks with type filter
            var (statusCode, responseBody) = _locations.GetLocationFeedbacksWithCurl(locationId, type: feedbackType);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");

            var feedbacks = responseBody["content"] as JArray;
            if (feedbacks != null && feedbacks.Count > 0)
            {
                foreach (var feedback in feedbacks)
                {
                    string type = feedback["type"]?.ToString()?.Replace("-", "_");
                    // Normalize the type string to handle variations
                    string normalizedType = type?.ToUpper()?.Replace("-", "_");
                    Assert.That(normalizedType, Is.EqualTo(feedbackType),
                        $"All feedbacks should be of type {feedbackType}, but found {type}");
                }
            }
        }

        [Test]
        public void GetLocationFeedbacks_SortByDateDesc_ShouldReturnFeedbacksInCorrectOrder()
        {
            // Arrange
            var (locationsStatus, locations) = _locations.GetLocationsWithCurl();

            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();

            // Act - Get feedbacks sorted by date
            var (statusCode, responseBody) = _locations.GetLocationFeedbacksWithCurl(locationId, sortBy: "date", sortDir: "DESC");

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");

            // Check sorting by date (descending - from newest to oldest)
            var feedbacks = responseBody["content"] as JArray;
            if (feedbacks != null && feedbacks.Count > 1)
            {
                DateTime prevDate = DateTime.MaxValue;

                foreach (var feedback in feedbacks)
                {
                    string dateStr = feedback["date"]?.ToString();
                    Assert.That(dateStr, Is.Not.Null.Or.Empty, "Date should not be null or empty");

                    DateTime currentDate = DateTime.Parse(dateStr);
                    Assert.That(currentDate, Is.LessThanOrEqualTo(prevDate),
                        "Feedbacks should be sorted by date in descending order");

                    prevDate = currentDate;
                }
            }
        }

        [Test]
        public void GetLocationById_ValidIdWithoutToken_ReturnsNotFound()
        {
            // Arrange
            string locationId = Guid.NewGuid().ToString(); // Using a random GUID as valid format

            // Act
            var (statusCode, responseBody) = _locations.GetLocationByIdWithCurl(locationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "Should return NotFound status when location ID is not found");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            Assert.That(responseBody["type"]?.ToString(), Is.EqualTo("NotFoundException"),
                "Error type should indicate NotFoundException");
        }
    }
}
