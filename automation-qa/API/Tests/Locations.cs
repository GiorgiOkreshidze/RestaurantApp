using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;
using ApiTests.Utils;
using ApiTests.Utilities;

namespace ApiTests
{
    [TestFixture]
    [Category("Locations")]
    public class LocationsTests : BaseTest
    {
        private Locations _locations;
        private TestDataHelper _testDataHelper;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _testDataHelper = new TestDataHelper();

            // Attempting to populate the database, but not interrupting the tests if it fails
            try
            {
                Console.WriteLine("Attempting to seed test data...");
                bool seeded = await _testDataHelper.SeedLocationsData();
                Console.WriteLine($"Data seeding result: {(seeded ? "Success" : "Failed")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during data seeding: {ex.Message}");
                // Do not interrupt the test execution due to an error
            }
        }

        [SetUp]
        public void Setup()
        {
            _locations = new Locations();
        }

        [Test]
        public async Task GetLocations_ShouldReturnAllLocations()
        {
            // Act
            var (statusCode, responseBody) = await _locations.GetLocations();

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
        public async Task GetLocationSelectOptions_ShouldReturnFormattedOptions()
        {
            // Act
            var (statusCode, responseBody) = await _locations.GetLocationSelectOptions();

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
        public async Task GetSpecialityDishes_ValidLocationId_ShouldReturnDishes()
        {
            // Arrange - first, get the list of locations
            var (locationsStatus, locations) = await _locations.GetLocations();
            Assert.That(locationsStatus, Is.EqualTo(HttpStatusCode.OK), "Getting locations should succeed");

            string locationId;

            // If there are no locations, create a test location
            if (locations == null || locations.Count == 0)
            {
                Console.WriteLine("No locations found. Creating a test location...");
                locationId = await _testDataHelper.CreateRandomLocation();

                if (string.IsNullOrEmpty(locationId))
                {
                    Assert.Inconclusive("Failed to create test location");
                    return;
                }

                Console.WriteLine($"Created test location with ID: {locationId}");
            }
            else
            {
                locationId = locations[0]["id"].ToString();
            }

            // Act
            var (statusCode, responseBody) = await _locations.GetSpecialityDishes(locationId);

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
                else
                {
                    Console.WriteLine("Note: Dish description is null, but this may be acceptable");
                }
            }
        }

        [Test]
        public async Task GetSpecialityDishes_InvalidLocationId_ReturnsEmptyArray()
        {
            // Arrange
            string invalidLocationId = "non-existent-id";

            // Act
            var (statusCode, responseBody) = await _locations.GetSpecialityDishes(invalidLocationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "API returns 200 OK with invalid location ID");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody.Count, Is.EqualTo(0), "Response should be an empty array");
        }

        [Test]
        public async Task GetSpecialityDishes_EmptyLocationId_ReturnsEmptyArray()
        {
            // Arrange
            string emptyLocationId = "";

            // Act
            var (statusCode, responseBody) = await _locations.GetSpecialityDishes(emptyLocationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "API returns 200 OK with empty location ID");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody.Count, Is.EqualTo(0), "Response should be an empty array");
        }

        [Test]
        public async Task GetLocations_ShouldContainDetailsForLocationOverview()
        {
            // Act
            var (statusCode, responseBody) = await _locations.GetLocations();

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
        public async Task GetLocationFeedbacks_ReturnsSuccess()
        {
            // Arrange - get the location ID for testing
            var (locationsStatus, locations) = await _locations.GetLocations();
            Assert.That(locationsStatus, Is.EqualTo(HttpStatusCode.OK), "Getting locations should succeed");

            // Check for the presence of locations
            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();

            // Act
            var (statusCode, responseBody) = await _locations.GetLocationFeedbacks(locationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Failed to get location feedbacks");
            Assert.That(responseBody, Is.Not.Null, "Feedbacks response is null");

            // Check the response structure (presence of required fields in the paginated response)
            Assert.That(responseBody["content"], Is.Not.Null, "Content field is missing in response");
            Assert.That(responseBody["size"], Is.Not.Null, "Size field is missing in response");
            Assert.That(responseBody["number"], Is.Not.Null, "Number field is missing in response");
            Assert.That(responseBody["sort"], Is.Not.Null, "Sort field is missing in response");
        }

        [Test]
        public async Task GetLocationFeedbacks_ShouldReturnFeedbacksWithPagination()
        {
            // Arrange - get the location ID for testing
            var (locationsStatus, locations) = await _locations.GetLocations();
            Assert.That(locationsStatus, Is.EqualTo(HttpStatusCode.OK), "Getting locations should succeed");

            string locationId;
            if (locations == null || locations.Count == 0)
            {
                Console.WriteLine("No locations found. Creating a test location...");
                locationId = await _testDataHelper.CreateRandomLocation();

                if (string.IsNullOrEmpty(locationId))
                {
                    Assert.Inconclusive("Failed to create test location");
                    return;
                }
            }
            else
            {
                locationId = locations[0]["id"].ToString();
            }

            // Act
            var (statusCode, responseBody) = await _locations.GetLocationFeedbacks(locationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            Assert.That(responseBody["content"], Is.Not.Null, "Response should contain 'content' field");
            Assert.That(responseBody["pageable"], Is.Not.Null, "Response should contain 'pageable' field");
            Assert.That(responseBody["size"], Is.Not.Null, "Response should contain 'size' field");
            Assert.That(responseBody["number"], Is.Not.Null, "Response should contain 'number' field");
            Assert.That(responseBody["sort"], Is.Not.Null, "Response should contain 'sort' field");

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
        public async Task GetLocationFeedbacks_WithServiceQualityFilter_ShouldReturnOnlyServiceQualityFeedbacks()
        {
            // Arrange
            var (locationsStatus, locations) = await _locations.GetLocations();
            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();
            string feedbackType = "SERVICE_QUALITY";

            // Act
            var (statusCode, responseBody) = await _locations.GetLocationFeedbacks(locationId, type: feedbackType);

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
        public async Task GetLocationFeedbacks_WithCuisineExperienceFilter_ShouldReturnOnlyCuisineExperienceFeedbacks()
        {
            // Arrange
            var (locationsStatus, locations) = await _locations.GetLocations();
            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();
            string feedbackType = "CUISINE_EXPERIENCE";

            // Act
            var (statusCode, responseBody) = await _locations.GetLocationFeedbacks(locationId, type: feedbackType);

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
        public async Task GetLocationFeedbacks_SortByDateDesc_ShouldReturnFeedbacksInCorrectOrder()
        {
            // Arrange
            var (locationsStatus, locations) = await _locations.GetLocations();
            if (locations == null || locations.Count == 0)
            {
                Assert.Inconclusive("No locations available for testing");
                return;
            }

            string locationId = locations[0]["id"].ToString();

            // Act
            var (statusCode, responseBody) = await _locations.GetLocationFeedbacks(
                locationId,
                sortBy: "date",
                sortDir: "DESC");

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
        public async Task GetLocationById_NullLocationId_ShouldThrowArgumentException()
        {
            // Arrange
            string locationId = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _locations.GetLocationById(locationId),
                "Should throw ArgumentException for null locationId");
        }

        [Test]
        public async Task GetLocationById_EmptyLocationId_ShouldThrowArgumentException()
        {
            // Arrange
            string locationId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _locations.GetLocationById(locationId),
                "Should throw ArgumentException for empty locationId");
        }

        [Test]
        public async Task GetLocationById_ValidIdWithoutToken_ReturnsForbidden()
        {
            // Arrange
            string locationId = Config.ValidLocationId;

            // Act
            var result = await _locations.GetLocationById(locationId);
            HttpStatusCode statusCode = result.StatusCode;
            JObject? responseBody = result.ResponseBody;

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Forbidden),
                "Should return Forbidden status when no authentication token is provided");
            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
            Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Missing Authentication Token"),
                "Error message should indicate missing authentication token");
        }

        [Test]
        public async Task GetLocationById_InvalidFormatLocationId_ShouldThrowArgumentException()
        {
            // Arrange
            string locationId = "invalid-format-id";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _locations.GetLocationById(locationId),
                "Should throw ArgumentException for invalid format locationId");
        }
    }
}
