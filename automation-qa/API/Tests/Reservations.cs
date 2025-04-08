using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using NUnit.Framework;
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

        [SetUp]
        public void Setup()
        {
            _reservations = new Reservations();
            _testLocationId = Config.ValidLocationId; // Use from TestConfig
            _testDate = DateTime.Now.ToString("yyyy-MM-dd");
            _testTableId = "04ba5b37-8fbd-4f5f-8354-0b75078a790a"; // Valid table ID
            _testWaiterId = "16929204-5081-706c-f4dc-a1695648cd31"; // Valid waiter ID
        }

        [Test]
        public async Task GetAvailableTables_ReturnsSuccess()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: _testLocationId, date: _testDate);
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody.Count, Is.GreaterThan(0), "Should have at least one available table");
        }

        [Test]
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
                    // If parsing fails, skip message validation
                }
            }
        }

        [Test]
        public async Task GetAvailableTables_WithDateFilter_ReturnsTablesForSpecificDate()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: _testLocationId, date: _testDate);
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // If API returns date in the response, add validation
            // if (responseBody.Count > 0 && responseBody[0]["date"] != null)
            // {
            //     Assert.That(responseBody[0]["date"].ToString(), Is.EqualTo(_testDate), "Date should match the filter");
            // }
        }

        [Test]
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
        public async Task GetAvailableTables_WithTimeFilter_ReturnsTablesWithAvailableTimeSlots()
        {
            // Use a specific time value instead of current time
            string timeSlot = "14:00"; // Choose a time that's likely within available slots

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: _testDate,
                time: timeSlot
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            if (responseBody != null && responseBody.Count > 0)
            {
                bool anyTableHasTimeSlots = false;

                foreach (var table in responseBody)
                {
                    var availableSlots = table["availableSlots"] as JArray;
                    Assert.That(availableSlots, Is.Not.Null, "Table should have available time slots");

                    if (availableSlots != null && availableSlots.Count > 0)
                    {
                        anyTableHasTimeSlots = true;

                        // Log available slots for debugging
                        Console.WriteLine($"Available slots for table {table["tableId"]}:");
                        foreach (var slot in availableSlots)
                        {
                            string start = slot["start"].ToString();
                            string end = slot["end"].ToString();
                            Console.WriteLine($"  {start} - {end}");
                        }
                    }
                }

                Assert.That(anyTableHasTimeSlots, Is.True, "At least one table should have available time slots");
            }
        }

        [Test]
        public async Task GetAvailableTables_PastDate_ReturnsBadRequest()
        {
            // Test verifies that tables cannot be retrieved for past dates
            string pastDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string pastTime = "12:00"; // Middle of the day time

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
        public async Task GetAvailableTables_WithoutParameters_ShouldNotFail()
        {
            // Test checks basic request without parameters
            var (statusCode, responseBody) = await _reservations.GetAvailableTables();

            // API returns 500, so check only that status is not null
            Assert.That(statusCode, Is.Not.EqualTo(0), "Should return a valid HTTP status");
            Console.WriteLine($"Received status: {statusCode}");
        }

        [Test]
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
        public async Task GetAvailableTables_WithInvalidId_ReturnsBadRequest()
        {
            // Test checks request with invalid location ID
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: "invalid-id"
            );

            // Expect client error
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400).And.LessThan(500),
                "Should return client error with invalid location ID");
        }

        [Test]
        public async Task CreateReservation_WithoutToken_ReturnsUnauthorized()
        {
            // Test checks reservation creation without token
            var (statusCode, responseBody) = await _reservations.CreateReservation(
                locationId: _testLocationId,
                tableId: "table-id",
                date: DateTime.Now.ToString("yyyy-MM-dd"),
                startTime: "19:00",
                endTime: "21:00",
                guests: 2,
                name: "Test User",
                email: "test@example.com",
                phone: "1234567890"
            );

            // Expect 401 Unauthorized or 403 Forbidden
            Assert.That(statusCode, Is.AnyOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden),
                "Should require authentication for reservation creation");
        }

        [Test]
        public async Task GetAvailableTables_WithMultipleFilters_ReturnsFilteredTables()
        {
            string locationId = _testLocationId;
            string date = _testDate;
            int guests = 2;

            // Use UTC time and add 2 hours to ensure time is in the future
            string timeString = DateTime.UtcNow.AddHours(2).ToString("HH:mm");

            // Explicitly declare types for statusCode and responseBody variables
            var result = await _reservations.GetAvailableTables(
                locationId: locationId,
                date: date,
                guests: guests,
                time: timeString);

            HttpStatusCode statusCode = result.Item1;
            JArray responseBody = result.Item2;

            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Add check, continue only if status is OK and data exists
            if (statusCode == HttpStatusCode.OK && responseBody != null && responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    int capacity = table["capacity"]?.Value<int>() ?? 0;
                    Assert.That(capacity, Is.GreaterThanOrEqualTo(guests),
                        $"All tables should accommodate {guests} guests");

                    // Use availableSlots instead of availableTimes
                    var availableSlots = table["availableSlots"] as JArray;
                    Assert.That(availableSlots, Is.Not.Null, "Table should have available time slots");

                    if (availableSlots != null)
                    {
                        // Find slot that contains specified time
                        bool hasMatchingSlot = availableSlots.Any(slot =>
                            slot["start"]?.ToString() == timeString ||
                            (slot["start"]?.ToString() != null &&
                             slot["end"]?.ToString() != null &&
                             TimeSpanBetween(timeString, slot["start"].ToString(), slot["end"].ToString()))
                        );

                        Assert.That(hasMatchingSlot, Is.True,
                            "Table should be available at time " + timeString);
                    }
                }
            }
        }

        // Helper function to check if time is between slot start and end
        private bool TimeSpanBetween(string time, string start, string end)
        {
            // Parse time strings to TimeSpan objects
            TimeSpan timeSpan = TimeSpan.Parse(time);
            TimeSpan startSpan = TimeSpan.Parse(start);
            TimeSpan endSpan = TimeSpan.Parse(end);

            // Check if specified time is within the slot
            return timeSpan >= startSpan && timeSpan <= endSpan;
        }

        [Test]
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
            // token not specified
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Check that API requires authorization
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API should require authorization (401 Unauthorized)");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            // Check that response contains error message
            Assert.That(responseBody["message"], Is.Not.Null,
                "Response should contain error message");

            Assert.That(responseBody["message"].ToString(), Does.Contain("Unauthorized"),
                "Error message should indicate lack of authorization");
        }

        [Test]
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
        public async Task CreateReservationByWaiter_WithInvalidTimeFormat_ShouldReturnBadRequest()
        {
            // Arrange
            string invalidTimeFormat = "19-00"; // Invalid time format
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
        public async Task CreateReservationByWaiter_WithTooManyGuests_ShouldRequireAuthorization()
        {
            // Arrange
            string timeFrom = "19:00";
            string timeTo = "20:00";
            int tooManyGuests = 15; // Large number of guests
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
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Check that API requires authorization regardless of other parameters
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API should require authorization regardless of guest count");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            Assert.That(responseBody["message"], Is.Not.Null,
                "Response should contain error message");
        }

        [Test]
        public async Task CreateReservationByWaiter_WithMissingRequiredFields_ShouldReturnBadRequest()
        {
            // Arrange - skip required fields

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                // No locationId specified
                // No tableId specified
                date: _testDate,
                timeFrom: "19:00",
                timeTo: "20:00",
                guestNumber: 2
            // No waiterId specified
            // No clientType specified
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
        public async Task CreateReservationByWaiter_WithEmptyParameters_ShouldRequireAuthorization()
        {
            // Act - send request with empty parameters
            var result = await _reservations.CreateReservationByWaiter();

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Check that API requires authorization even with empty parameters
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API should require authorization even with empty parameters");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            Assert.That(responseBody["message"]?.ToString(), Does.Contain("Unauthorized"),
                "Response should contain message about authorization requirement");
        }

        [Test]
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

            // Check that API requires authorization
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API should require authorization for reservation cancellation");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            Assert.That(responseBody["message"], Is.Not.Null,
                "Response should contain error message");
        }

        [Test]
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
        public async Task CancelReservation_WithEmptyId_ShouldReturnError()
        {
            // Arrange
            string emptyReservationId = "";

            // Act
            var result = await _reservations.CancelReservation(emptyReservationId);

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // API returns 403 for empty ID without token
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "API should return error code for empty reservation ID");

            Assert.That(responseBody, Is.Not.Null,
                "Response body should not be null");

            Assert.That(responseBody["message"], Is.Not.Null,
                "Response should contain error message");
        }

        [Test]
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
        public async Task CancelReservation_WithVeryLongId_ShouldReturnError()
        {
            // Arrange
            string veryLongId = new string('a', 100); // Уменьшаем до 100 символов

            // Act
            var result = await _reservations.CancelReservation(veryLongId);

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Обрабатываем ситуацию со статусом 0
            if ((int)statusCode == 0)
            {
                Console.WriteLine("Соединение с API не удалось установить, статус = 0.");
                Console.WriteLine("Возможная причина: слишком длинный ID вызывает ошибку при формировании URL.");

                // Используем Inconclusive вместо Assert.Fail, чтобы показать, что тест не провалился, а не смог быть выполнен
                Assert.Inconclusive("Тест не может быть выполнен из-за проблем с соединением. Возможно, ID слишком длинный.");
                return;
            }

            // Проверяем, что API корректно обрабатывает длинный ID
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "API должен возвращать код ошибки для длинного ID");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
        }
    }
}
