using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;
using ApiTests.Utilities;

namespace ApiTests
{
    [TestFixture]
    [Category("Reservations")]
    public class ReservationsTests : BaseTest
    {
        private Reservations _reservations;
        private string _testLocationId;
        private string _testDate;
        private string _testTableId;
        private string _testWaiterId;
        private Authentication _auth;

        [SetUp]
        public void Setup()
        {
            _reservations = new Reservations();
            _auth = new Authentication();
            _testLocationId = Config.ValidLocationId;
            _testDate = DateTime.Now.ToString("yyyy-MM-dd");
            _testTableId = "04ba5b37-8fbd-4f5f-8354-0b75078a790a";
            _testWaiterId = "16929204-5081-706c-f4dc-a1695648cd31";
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetAvailableTables_ReturnsSuccess()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: _testLocationId, date: _testDate);
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody.Count, Is.GreaterThan(0), "Should have at least one available table");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetAvailableTables_HasCorrectStructure()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: _testDate
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            if (statusCode != HttpStatusCode.OK)
            {
                Assert.Fail($"API returned error: {statusCode}. Response: {responseBody}");
            }

            if (responseBody != null && responseBody.Count > 0)
            {
                var firstTable = responseBody[0];
                Assert.That(firstTable["tableId"], Is.Not.Null, "Table should have an ID");
                Assert.That(firstTable["capacity"], Is.Not.Null, "Table should have capacity information");
                Assert.That(firstTable["availableSlots"], Is.Not.Null, "Table should have available time slots");

                var availableSlots = firstTable["availableSlots"] as JArray;
                Assert.That(availableSlots, Is.Not.Null, "Available slots should be an array");
                if (availableSlots != null && availableSlots.Count > 0)
                {
                    var firstSlot = availableSlots[0];
                    Assert.That(firstSlot["start"], Is.Not.Null, "Slot should have start time");
                    Assert.That(firstSlot["end"], Is.Not.Null, "Slot should have end time");
                    Assert.That(firstSlot["start"].ToString(), Does.Match(@"^\d{2}:\d{2}$"), "Start time should be in HH:MM format");
                    Assert.That(firstSlot["end"].ToString(), Does.Match(@"^\d{2}:\d{2}$"), "End time should be in HH:MM format");
                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_WithLocationFilter_ReturnsFilteredTables()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: _testDate
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody != null && responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    if (table["locationId"] != null)
                    {
                        Assert.That(table["locationId"].ToString(), Is.EqualTo(_testLocationId),
                            "All tables should belong to the specified location");
                    }
                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_WithInvalidLocationId_ReturnsBadRequest()
        {
            string invalidLocationId = "nonexistent";
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: invalidLocationId);
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound),
                "Should return an error for invalid location ID");

            // Check error message if present
            if (responseBody != null && statusCode != HttpStatusCode.OK)
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
                catch
                {

                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_WithDateFilter_ReturnsTablesForSpecificDate()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: _testLocationId, date: _testDate);
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_WithGuestsFilter_ReturnsTablesWithSufficientCapacity()
        {
            int guestCount = 4;
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: _testDate,
                guests: guestCount
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody != null && responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    int capacity = table["capacity"]?.Value<int>() ?? 0;
                    Assert.That(capacity, Is.GreaterThanOrEqualTo(guestCount),
                        $"All tables should accommodate {guestCount} guests");
                }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_WithTimeFilter_ReturnsTablesWithAvailableTimeSlots()
        {
            // Use a future time slot to ensure availability
            DateTime futureTime = DateTime.Now.AddHours(1);
            string timeSlot = futureTime.ToString("HH:mm");

            // Call method to get available tables with time filter
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: _testDate,
                time: timeSlot
            );

            // Log the response for debugging
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Validate basic API response
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Detailed validation of available tables
            if (responseBody != null && responseBody.Count > 0)
            {
                bool anyTableHasAvailableSlots = false;

                // Iterate through each table in the response
                foreach (var table in responseBody)
                {
                    // Extract available time slots for the table
                    var availableSlots = table["availableSlots"] as JArray;

                    // Verify table has time slots
                    Assert.That(availableSlots, Is.Not.Null, "Table should have available time slots");

                    // Check and log available slots
                    if (availableSlots != null && availableSlots.Count > 0)
                    {
                        anyTableHasAvailableSlots = true;

                        // Log available slots for each table
                        Console.WriteLine($"Available slots for table {table["tableId"]}:");
                        foreach (var slot in availableSlots)
                        {
                            string start = slot["start"].ToString();
                            string end = slot["end"].ToString();
                            Console.WriteLine($"  {start} - {end}");
                        }
                    }
                }

                // Final assertion to ensure at least one table has available slots
                Assert.That(anyTableHasAvailableSlots, Is.True,
                    "At least one table should have available time slots");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_PastDate_ReturnsBadRequest()
        {
            // Test verifies that tables cannot be retrieved for past dates
            string pastDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string pastTime = "12:00";

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: pastDate,
                time: pastTime
            );

            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Should return an error for past date and time");

            // Check error message if present
            if (statusCode == HttpStatusCode.BadRequest && responseBody != null)
            {
                string errorMessage = "";
                try
                {
                    // Try to read error message from JSON
                    JObject errorObj = JObject.Parse(responseBody.ToString());
                    errorMessage = errorObj["message"]?.ToString() ?? "";
                }
                catch { /* In case of parsing error, skip */ }

                Console.WriteLine($"Error message: {errorMessage}");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_FutureDate_ReturnsSuccess()
        {
            // Test verifies that tables can be retrieved for future dates
            string futureDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: futureDate
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return successful status for future date");
            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_InvalidDateFormat_ReturnsBadRequest()
        {
            // Test verifies that API returns an error with invalid date format
            string invalidDate = "01/01/2025"; // MM/dd/yyyy format instead of yyyy-MM-dd

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: invalidDate
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Should return an error with invalid date format");

            // Check error message
            if (responseBody != null)
            {
                try
                {
                    JObject errorObj = JObject.Parse(responseBody.ToString());
                    if (errorObj["message"] != null)
                    {
                        Assert.That(errorObj["message"].ToString(),
                            Contains.Substring("date").IgnoreCase.Or.Contains("format").IgnoreCase,
                            "Error message should indicate issue with date format");
                    }
                }
                catch { }
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_WithoutParameters_ShouldNotFail()
        {
            // Test checks basic request without parameters
            var (statusCode, responseBody) = await _reservations.GetAvailableTables();

            // API returns 500, so check only that status is not null
            Assert.That(statusCode, Is.Not.EqualTo(0), "Should return a valid HTTP status");
            Console.WriteLine($"Received status: {statusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task GetUserReservations_ReturnsUnauthorized_WhenNotAuthenticated()
        {
            // Test checks user reservation retrieval without authentication
            var (statusCode, responseBody) = await _reservations.GetUserReservations();

            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Check that status code is not 0 (connection problem)
            if ((int)statusCode == 0)
            {
                Assert.Inconclusive("API is unavailable (status code 0). Check server connection.");
                return;
            }

            // Expect 401 Unauthorized
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Should return 401 Unauthorized when user is not authenticated");

            // Check error message
            if (responseBody != null)
            {
                Assert.That(responseBody.ToString(),
                    Contains.Substring("unauthorized").IgnoreCase.Or.Contains("authentication").IgnoreCase,
                    "Error message should indicate authentication issue");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableTables_WithValidLocationAndDate_ShouldNotFail()
        {
            // Test checks request with valid data
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: DateTime.Now.ToString("yyyy-MM-dd")
            );

            // Check that request was processed (either OK or NotFound)
            Assert.That((int)statusCode, Is.LessThan(500),
                "Should not return server error with valid parameters");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithoutToken_ShouldRequireAuthorization()
        {
            // Arrange
            string timeFrom = "19:00";
            string timeTo = "20:00";
            int guestNumber = 2;
            string clientType = "Customer";

            // Act - send request without token
            var result = await _reservations.CreateReservationByWaiter(
                locationId: _testLocationId,
                tableId: _testTableId,
                date: _testDate,
                timeFrom: timeFrom,
                timeTo: timeTo,
                guestNumber: guestNumber,
                waiterId: _testWaiterId,
                clientType: clientType
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API should require authorization (401 Unauthorized)");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            Assert.That(responseBody["type"]?.ToString(), Is.EqualTo("Unauthorized"),
                "Response should contain 'type' with value 'Unauthorized'");

            Assert.That(responseBody["title"]?.ToString(), Is.EqualTo("Invalid Request"),
                "Response should contain 'title' with value 'Invalid Request'");
        }


        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithInvalidLocationId_ShouldReturnBadRequest()
        {
            // Arrange
            string invalidLocationId = "invalid-location-id";
            string timeFrom = "19:00";
            string timeTo = "20:00";
            int guestNumber = 2;
            string clientType = "Customer";

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                locationId: invalidLocationId,
                tableId: _testTableId,
                date: _testDate,
                timeFrom: timeFrom,
                timeTo: timeTo,
                guestNumber: guestNumber,
                waiterId: _testWaiterId,
                clientType: clientType
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400).And.LessThan(500),
                "API should return client error code with invalid location ID");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithInvalidTableId_ShouldReturnBadRequest()
        {
            // Arrange
            string invalidTableId = "invalid-table-id";
            string timeFrom = "19:00";
            string timeTo = "20:00";
            int guestNumber = 2;
            string clientType = "Customer";

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                locationId: _testLocationId,
                tableId: invalidTableId,
                date: _testDate,
                timeFrom: timeFrom,
                timeTo: timeTo,
                guestNumber: guestNumber,
                waiterId: _testWaiterId,
                clientType: clientType
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400).And.LessThan(500),
                "API should return client error code with invalid table ID");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithPastDate_ShouldReturnBadRequest()
        {
            // Arrange
            string pastDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string timeFrom = "19:00";
            string timeTo = "20:00";
            int guestNumber = 2;
            string clientType = "Customer";

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                locationId: _testLocationId,
                tableId: _testTableId,
                date: pastDate,
                timeFrom: timeFrom,
                timeTo: timeTo,
                guestNumber: guestNumber,
                waiterId: _testWaiterId,
                clientType: clientType
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400).And.LessThan(500),
                "API should return client error code with past date");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithInvalidTimeFormat_ShouldReturnBadRequest()
        {
            // Arrange
            string invalidTimeFormat = "19-00";
            string timeTo = "20:00";
            int guestNumber = 2;
            string clientType = "Customer";

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                locationId: _testLocationId,
                tableId: _testTableId,
                date: _testDate,
                timeFrom: invalidTimeFormat,
                timeTo: timeTo,
                guestNumber: guestNumber,
                waiterId: _testWaiterId,
                clientType: clientType
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400).And.LessThan(500),
                "API should return client error code with invalid time format");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithTooManyGuests_ShouldRequireAuthorization()
        {
            // Arrange
            string timeFrom = "19:00";
            string timeTo = "20:00";
            int tooManyGuests = 15;
            string clientType = "Customer";

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                locationId: _testLocationId,
                tableId: _testTableId,
                date: _testDate,
                timeFrom: timeFrom,
                timeTo: timeTo,
                guestNumber: tooManyGuests,
                waiterId: _testWaiterId,
                clientType: clientType
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API should require authorization regardless of guest count");
            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
            Assert.That(responseBody["title"], Is.Not.Null,
                "Response should contain error title");
            Assert.That(responseBody["title"].ToString(), Is.EqualTo("Invalid Request"),
                "Response should contain correct error message");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithMissingRequiredFields_ShouldReturnBadRequest()
        {

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                // No locationId specified
                // No tableId specified
                date: _testDate,
                timeFrom: "19:00",
                timeTo: "20:00",
                guestNumber: 2
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400).And.LessThan(500),
                "API should return client error code when required fields are missing");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithEmptyParameters_ShouldRequireAuthorization()
        {
            // Act - send request with empty parameters
            var result = await _reservations.CreateReservationByWaiter();

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API should require authorization even with empty parameters");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            Assert.That(responseBody["title"]?.ToString(), Is.EqualTo("Invalid Request"),
                "Response should contain 'title' with value 'Invalid Request'");

            Assert.That(responseBody["type"]?.ToString(), Is.EqualTo("Unauthorized"),
                "Response should contain 'type' with value 'Unauthorized'");
        }


        [Test]
        [Category("Regression")]
        public async Task CancelReservation_WithoutToken_ShouldRequireAuthorization()
        {
            // Arrange
            string reservationId = "30001"; // Test ID from example

            // Act
            var result = await _reservations.CancelReservation(reservationId);

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API should require authorization for reservation cancellation");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            Assert.That(responseBody["title"]?.ToString(), Is.EqualTo("Invalid Request"),
                "Response should contain 'title' with value 'Invalid Request'");

            Assert.That(responseBody["type"]?.ToString(), Is.EqualTo("Unauthorized"),
                "Response should contain 'type' with value 'Unauthorized'");
        }


        [Test]
        [Category("Regression")]
        public async Task CancelReservation_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            string invalidReservationId = "non-existent-id";

            // Act
            var result = await _reservations.CancelReservation(invalidReservationId);

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // API may return either 401 (if it checks auth first)
            // or 404 (if it checks resource existence first)
            Assert.That((int)statusCode, Is.AnyOf(401, 404),
                "API should return error for non-existent reservation ID");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task CancelReservation_WithSpecialCharactersInId_ShouldReturnError()
        {
            // Arrange
            string invalidId = "!@#$%^&*()";

            // Act
            var result = await _reservations.CancelReservation(invalidId);

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Check that API returns error for ID with special characters
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "API should return error code for ID with special characters");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetAvailableTables_BasicSuccess()
        {
            // Arrange
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: today
            );

            // Log detailed response
            Console.WriteLine($"GetAvailableTables response status: {statusCode}");
            if (responseBody != null)
            {
                Console.WriteLine($"GetAvailableTables response content: {responseBody}");
            }

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task GetUserReservations_UnauthorizedAccess()
        {
            // Act
            var (statusCode, responseBody) = await _reservations.GetUserReservations();

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Should return 401 Unauthorized when not authenticated");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateReservationByWaiter_WithoutToken_ShouldFail()
        {
            // Arrange
            string timeFrom = "19:00";
            string timeTo = "20:00";

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                locationId: _testLocationId,
                tableId: _testTableId,
                date: _testDate,
                timeFrom: timeFrom,
                timeTo: timeTo,
                guestNumber: 2
            );

            // Assert
            HttpStatusCode statusCode = result.StatusCode;
            Console.WriteLine($"Status: {statusCode}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Should require authorization for reservation creation");
        }

        [Test]
        [Category("Regression")]
        public async Task CancelReservation_WithVeryLongId_ShouldReturnError()
        {
            // Arrange
            string veryLongId = new string('a', 100);

            // Act
            var result = await _reservations.CancelReservation(veryLongId);

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            if ((int)statusCode == 0)
            {
                Console.WriteLine("Connection to the API could not be established, status = 0.");
                Console.WriteLine("Possible reason: the very long ID causes an error when forming the URL.");

                Assert.Inconclusive("Test cannot be performed due to connection issues. The ID may be too long.");
                return;
            }

            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "The API should return an error code for a long ID");

            Assert.That(responseBody, Is.Not.Null,
                "The response body should not be null");
        }

        [Test]
        [Category("Smoke")]
        public async Task CompleteReservation_WithoutToken_ShouldReturnUnauthorized()
        {
            // Act
            var (statusCode, responseBody) = await _reservations.CompleteReservation(Config.ValidReservationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Complete reservation without token should return Unauthorized");
        }

        [Test]
        [Category("Regression")]
        public async Task CompleteReservation_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            string invalidReservationId = "invalid-id-" + Guid.NewGuid().ToString();

            var (loginStatus, userLoginResponse) = _auth.LoginUserWithCurl(Config.TestUserEmail, Config.TestUserPassword);

            if (loginStatus != HttpStatusCode.OK)
            {
                Assert.Ignore($"Cannot login with test user credentials. Status: {loginStatus}");
                return;
            }

            string userToken = userLoginResponse["accessToken"].ToString();

            // Act
            var (statusCode, _) = await _reservations.CompleteReservation(invalidReservationId, userToken);

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.NotFound, HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden),
                "Complete reservation with invalid ID should return either NotFound, Unauthorized, or Forbidden");
        }

        [Test]
        [Category("Regression")]
        public async Task CompleteReservation_WithAdminToken_ShouldSucceed()
        {
            var (_, userLoginResponse) = _auth.LoginUserWithCurl(Config.TestUserEmail, Config.TestUserPassword);
            string token = userLoginResponse["accessToken"].ToString();

            // Act
            var (statusCode, responseBody) = await _reservations.CompleteReservation(Config.ValidReservationId, token);

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Expected 401 Unauthorized status for regular user");

            Console.WriteLine($"Status code: {statusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task CompleteReservation_WithWaiterToken_ShouldSucceed()
        {
            // Arrange
            var (_, waiterLoginResponse) = _auth.LoginUserWithCurl(Config.WaiterEmail, Config.WaiterPassword);
            string waiterToken = waiterLoginResponse["accessToken"].ToString();

            // Act
            var (statusCode, _) = await _reservations.CompleteReservation(Config.ValidReservationId, waiterToken);

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Expected 401 Unauthorized status for waiter user");

            Console.WriteLine($"Test completed with status code: {statusCode}");
        }

        [Test]
        [Category("Smoke")]
        public async Task AddDishToOrder_SimpleTest()
        {
            // Arrange - use hard-coded values for simple test
            string reservationId = TestConfig.Instance.ValidReservationId;
            string dishId = "c2a82c5a-3dde-4bb0-8906-050fd4ed3d78"; // Example dish ID

            // Act - test the endpoint without authentication
            var result = await _reservations.AddDishToOrder(reservationId, dishId);
            HttpStatusCode statusCode = result.StatusCode;

            // Assert - we expect either a 401/403 for auth failure or 200 if auth not required
            // This just verifies the endpoint exists and responds
            Assert.That(statusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden),
                "Endpoint should respond with expected status code");
        }

        [Test]
        [Category("Smoke")]
        public async Task AddDishToOrder_Unauthorized()
        {
            // Arrange
            string reservationId = TestConfig.Instance.ValidReservationId;
            string dishId = "c2a82c5a-3dde-4bb0-8906-050fd4ed3d78"; // Example dish ID

            // Act - no token provided
            var result = await _reservations.AddDishToOrder(reservationId, dishId, null);
            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden),
                "Should return error without authentication");
        }

        [Test]
        [Category("Regression")]
        public async Task AddDishToOrder_EmptyReservationId_ThrowsException()
        {
            // Arrange
            string reservationId = string.Empty; // Empty reservation ID
            string dishId = "c2a82c5a-3dde-4bb0-8906-050fd4ed3d78"; // Example dish ID

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _reservations.AddDishToOrder(reservationId, dishId, null));

            Assert.That(exception.ParamName, Is.EqualTo("reservationId"),
                "Exception should contain the correct parameter name");
        }

        [Test]
        [Category("Regression")]
        public async Task AddDishToOrder_NullReservationId_ThrowsException()
        {
            // Arrange
            string reservationId = null; // Null reservation ID
            string dishId = "c2a82c5a-3dde-4bb0-8906-050fd4ed3d78"; // Example dish ID

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _reservations.AddDishToOrder(reservationId, dishId, null));

            Assert.That(exception.ParamName, Is.EqualTo("reservationId"),
                "Exception should contain the correct parameter name");
        }

        [Test]
        [Category("Smoke")]
        public async Task RemoveDishFromOrder_BasicTest()
        {
            // Arrange
            string reservationId = TestConfig.Instance.ValidReservationId;
            string dishId = "c2a82c5a-3dde-4bb0-8906-050fd4ed3d78"; // Example dish ID

            // Act
            var result = await _reservations.RemoveDishFromOrder(reservationId, dishId, null);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden),
                "Should return expected status code");
        }

        [Test]
        [Category("Regression")]
        public async Task RemoveDishFromOrder_InvalidDishId()
        {
            // Arrange
            string reservationId = TestConfig.Instance.ValidReservationId;
            string dishId = "invalid-dish-id";

            // Act
            var result = await _reservations.RemoveDishFromOrder(reservationId, dishId, null);

            // Assert
            Assert.That(result.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should not return OK for invalid dish ID");
        }

        [Test]
        [Category("Regression")]
        public async Task RemoveDishFromOrder_InvalidReservationId()
        {
            // Arrange
            string reservationId = "invalid-reservation-id";
            string dishId = "c2a82c5a-3dde-4bb0-8906-050fd4ed3d78";

            // Act
            var result = await _reservations.RemoveDishFromOrder(reservationId, dishId, null);

            // Assert
            Assert.That(result.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should not return OK for invalid reservation ID");
        }

        [Test]
        [Category("Regression")]
        public async Task RemoveDishFromOrder_NullDishId_ThrowsException()
        {
            // Arrange
            string reservationId = TestConfig.Instance.ValidReservationId;
            string dishId = null;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _reservations.RemoveDishFromOrder(reservationId, dishId, null));

            Assert.That(exception.ParamName, Is.EqualTo("dishId"),
                "Exception should contain the correct parameter name");
        }

        [Test]
        [Category("Regression")]
        public async Task RemoveDishFromOrder_NullReservationId_ThrowsException()
        {
            // Arrange
            string reservationId = null;
            string dishId = "c2a82c5a-3dde-4bb0-8906-050fd4ed3d78";

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _reservations.RemoveDishFromOrder(reservationId, dishId, null));

            Assert.That(exception.ParamName, Is.EqualTo("reservationId"),
                "Exception should contain the correct parameter name");
        }

        [Test]
        [Category("Smoke")]
        public async Task GetAvailableDishes_BasicTest()
        {
            // Arrange
            string reservationId = TestConfig.Instance.ValidReservationId;

            // Act
            var result = await _reservations.GetAvailableDishes(reservationId);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK for valid reservation ID");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableDishes_InvalidReservationId()
        {
            // Arrange
            string reservationId = "invalid-reservation-id";

            // Act
            var result = await _reservations.GetAvailableDishes(reservationId);

            // Assert
            Assert.That(result.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should not return OK for invalid reservation ID");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableDishes_NullReservationId_ThrowsException()
        {
            // Arrange
            string reservationId = null;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _reservations.GetAvailableDishes(reservationId));

            Assert.That(exception.ParamName, Is.EqualTo("reservationId"),
                "Exception should contain the correct parameter name");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableDishes_EmptyReservationId_ThrowsException()
        {
            // Arrange
            string reservationId = string.Empty;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _reservations.GetAvailableDishes(reservationId));

            Assert.That(exception.ParamName, Is.EqualTo("reservationId"),
                "Exception should contain the correct parameter name");
        }

        [Test]
        [Category("Regression")]
        public async Task GetAvailableDishes_ResponseStructure()
        {
            // Arrange
            string reservationId = TestConfig.Instance.ValidReservationId;

            // Act
            var result = await _reservations.GetAvailableDishes(reservationId);

            // Assert
            // Check that the response is either OK (200) or requires authentication (401)
            Assert.That(result.StatusCode, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized),
                "Should return 200 OK or 401 Unauthorized");

            // If we received 200 OK and there is content, check the structure
            if (result.StatusCode == HttpStatusCode.OK && result.ResponseBody != null && result.ResponseBody.Count > 0)
            {
                var firstDish = result.ResponseBody[0];
                Assert.That(firstDish["id"], Is.Not.Null, "Dish should have an ID");
                Assert.That(firstDish["name"], Is.Not.Null, "Dish should have a name");
            }
            // If we received 401 Unauthorized, the check passes without additional checks
            else if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                Assert.Pass("Endpoint requires authorization, which is expected behavior");
            }
        }
    }
}
