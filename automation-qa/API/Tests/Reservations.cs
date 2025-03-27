using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;

namespace ApiTests
{
    [TestFixture]
    [Category("Reservations")]
    public class ReservationsTests : BaseTest
    {
        private Reservations _reservations;
        private string _testLocationId;
        private string _testDate;

        [SetUp]
        public void Setup()
        {
            _reservations = new Reservations();
            _testLocationId = "1001";
            _testDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        [Test]
        public async Task GetAvailableTables_ReturnsSuccess()
        {
            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
            Assert.That(responseBody.Count, Is.GreaterThan(0), "There should be at least one available table");
        }

        [Test]
        public async Task GetAvailableTables_HasCorrectStructure()
        {
            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");

            // Check structure of the first table entry
            if (responseBody != null && responseBody.Count > 0)
            {
                var firstTable = responseBody[0];
                Assert.That(firstTable["tableId"], Is.Not.Null, "Table should have an ID");
                Assert.That(firstTable["capacity"], Is.Not.Null, "Table should have capacity info");
                Assert.That(firstTable["availableTimes"], Is.Not.Null, "Table should have available times");

                // Check available times structure
                var availableTimes = firstTable["availableTimes"] as JArray;
                Assert.That(availableTimes, Is.Not.Null, "Available times should be an array");
                if (availableTimes.Count > 0)
                {
                    Assert.That(availableTimes[0].Type, Is.EqualTo(JTokenType.String), "Available time should be a string");
                    Assert.That(availableTimes[0].ToString(), Does.Match(@"^\d{2}:\d{2}$"), "Time should be in HH:MM format");
                }
            }
        }

        [Test]
        public async Task GetAvailableTables_WithLocationFilter_ReturnsFilteredTables()
        {
            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: _testLocationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check that all tables belong to the specified location
            if (responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    // This assumes the response includes locationId. Adjust if the API works differently.
                    if (table["locationId"] != null)
                    {
                        Assert.That(table["locationId"].ToString(), Is.EqualTo(_testLocationId),
                            "All tables should belong to the requested location");
                    }
                }
            }
        }

        [Test]
        public async Task GetAvailableTables_WithInvalidLocationId_ReturnsBadRequest()
        {
            // Arrange
            string invalidLocationId = "nonexistent";

            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: invalidLocationId);

            // Assert
            // The API might return a 400 Bad Request or a 404 Not Found
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest) | Is.EqualTo(HttpStatusCode.NotFound),
                "Should return an error for invalid location ID");
        }

        [Test]
        public async Task GetAvailableTables_WithDateFilter_ReturnsTablesForSpecificDate()
        {
            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(date: _testDate);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Add assertions specific to date filtering if the API returns date information
        }

        [Test]
        public async Task GetAvailableTables_WithGuestsFilter_ReturnsTablesWithSufficientCapacity()
        {
            // Arrange
            int guestCount = 4;

            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(guests: guestCount);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check that all returned tables have sufficient capacity
            if (responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    int capacity = table["capacity"].Value<int>();
                    Assert.That(capacity, Is.GreaterThanOrEqualTo(guestCount),
                        $"All tables should have capacity for {guestCount} guests");
                }
            }
        }

        [Test]
        public async Task GetAvailableTables_WithTimeFilter_ReturnsTablesWithSpecificTimeSlot()
        {
            // Arrange
            string timeSlot = "19:00";

            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(time: timeSlot);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check that all returned tables have the specified time slot available
            if (responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    var availableTimes = table["availableTimes"] as JArray;
                    Assert.That(availableTimes, Is.Not.Null, "Table should have available times");
                    Assert.That(availableTimes.Any(t => t.ToString() == timeSlot), Is.True,
                        $"All tables should have the time slot {timeSlot} available");
                }
            }
        }

        [Test]
        public async Task GetAvailableTables_WithMultipleFilters_ReturnsFilteredTables()
        {
            // Arrange
            string locationId = _testLocationId;
            string date = _testDate;
            int guests = 2;
            string time = "19:00";

            // Act
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: locationId,
                date: date,
                guests: guests,
                time: time);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Check that all tables meet the criteria
            if (responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    // Check capacity
                    int capacity = table["capacity"].Value<int>();
                    Assert.That(capacity, Is.GreaterThanOrEqualTo(guests),
                        $"All tables should have capacity for {guests} guests");

                    // Check time slot availability
                    var availableTimes = table["availableTimes"] as JArray;
                    Assert.That(availableTimes, Is.Not.Null, "Table should have available times");
                    Assert.That(availableTimes.Any(t => t.ToString() == time), Is.True,
                        $"All tables should have the time slot {time} available");
                }
            }
        }

        [Test]
        public async Task GetUserReservations_ReturnsSuccess()
        {
            // Act
            var (statusCode, responseBody) = await _reservations.GetUserReservations();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");
        }

        [Test]
        public async Task GetUserReservations_HasCorrectStructure()
        {
            // Act
            var (statusCode, responseBody) = await _reservations.GetUserReservations();

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status 200 OK");

            // Check structure of reservation data
            if (responseBody != null && responseBody.Count > 0)
            {
                var firstReservation = responseBody[0];
                Assert.That(firstReservation["id"], Is.Not.Null, "Reservation should have an ID");
                Assert.That(firstReservation["locationId"], Is.Not.Null, "Reservation should have a location ID");
                Assert.That(firstReservation["locationName"], Is.Not.Null, "Reservation should have a location name");
                Assert.That(firstReservation["date"], Is.Not.Null, "Reservation should have a date");
                Assert.That(firstReservation["time"], Is.Not.Null, "Reservation should have a time");
                Assert.That(firstReservation["guests"], Is.Not.Null, "Reservation should have guests count");
                Assert.That(firstReservation["status"], Is.Not.Null, "Reservation should have a status");
                Assert.That(firstReservation["createdAt"], Is.Not.Null, "Reservation should have creation time");
            }
        }
    }
}
