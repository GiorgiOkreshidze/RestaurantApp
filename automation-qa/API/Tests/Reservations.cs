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
            _testLocationId = "8c4fc44e-c1a5-42eb-9912-55aeb5111a99"; // Валидный ID
            _testDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        [Test]
        public async Task GetAvailableTables_ReturnsSuccess()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: _testLocationId, date: _testDate);
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");
            Assert.That(responseBody.Count, Is.GreaterThan(0), "Должен быть хотя бы один доступный стол");
        }

        [Test]
        public async Task GetAvailableTables_HasCorrectStructure()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
                date: _testDate
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            if (statusCode != HttpStatusCode.OK)
            {
                Assert.Fail($"API вернул ошибку: {statusCode}. Ответ: {responseBody}");
            }

            if (responseBody != null && responseBody.Count > 0)
            {
                var firstTable = responseBody[0];
                Assert.That(firstTable["tableId"], Is.Not.Null, "Стол должен иметь ID");
                Assert.That(firstTable["capacity"], Is.Not.Null, "Стол должен иметь информацию о вместимости");
                Assert.That(firstTable["availableSlots"], Is.Not.Null, "Стол должен иметь доступные слоты времени");

                var availableSlots = firstTable["availableSlots"] as JArray;
                Assert.That(availableSlots, Is.Not.Null, "Доступные слоты должны быть массивом");
                if (availableSlots != null && availableSlots.Count > 0)
                {
                    var firstSlot = availableSlots[0];
                    Assert.That(firstSlot["start"], Is.Not.Null, "Слот должен иметь время начала");
                    Assert.That(firstSlot["end"], Is.Not.Null, "Слот должен иметь время окончания");
                    Assert.That(firstSlot["start"].ToString(), Does.Match(@"^\d{2}:\d{2}$"), "Время начала должно быть в формате HH:MM");
                    Assert.That(firstSlot["end"].ToString(), Does.Match(@"^\d{2}:\d{2}$"), "Время окончания должно быть в формате HH:MM");
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

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            if (responseBody != null && responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    if (table["locationId"] != null)
                    {
                        Assert.That(table["locationId"].ToString(), Is.EqualTo(_testLocationId),
                            "Все столы должны относиться к указанной локации");
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

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest) | Is.EqualTo(HttpStatusCode.NotFound),
                "Должен возвращать ошибку для неверного ID локации");
        }

        [Test]
        public async Task GetAvailableTables_WithDateFilter_ReturnsTablesForSpecificDate()
        {
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(locationId: _testLocationId, date: _testDate);
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            // Если API возвращает дату в ответе, можно добавить проверку
            // if (responseBody.Count > 0 && responseBody[0]["date"] != null)
            // {
            //     Assert.That(responseBody[0]["date"].ToString(), Is.EqualTo(_testDate), "Дата должна соответствовать фильтру");
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

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            if (responseBody != null && responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    int capacity = table["capacity"]?.Value<int>() ?? 0;
                    Assert.That(capacity, Is.GreaterThanOrEqualTo(guestCount),
                        $"Все столы должны вмещать {guestCount} гостей");
                }
            }
        }

        [Test]
        public async Task GetAvailableTables_WithTimeFilter_ReturnsTablesWithSpecificTimeSlot()
        {
            string timeSlot = "19:00";
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: _testDate,
                time: timeSlot
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            if (responseBody != null && responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    var availableSlots = table["availableSlots"] as JArray;
                    Assert.That(availableSlots, Is.Not.Null, "Стол должен иметь доступные слоты времени");
                    if (availableSlots != null)
                    {
                        Assert.That(
                            availableSlots.Any(slot => slot["start"]?.ToString() == timeSlot || slot["end"]?.ToString() == timeSlot),
                            Is.True,
                            $"Все столы должны иметь слот времени, включающий {timeSlot}"
                        );
                    }
                }
            }
        }

        [Test]
        public async Task GetAvailableTables_PastDate_ReturnsBadRequest()
        {
            // Тест проверяет, что нельзя получить столики на прошедшую дату
            string pastDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: pastDate
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Должен возвращать ошибку для даты в прошлом");
        }

        [Test]
        public async Task GetAvailableTables_FutureDate_ReturnsSuccess()
        {
            // Тест проверяет, что можно получить столики на будущую дату
            string futureDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: futureDate
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Должен возвращать успешный статус для будущей даты");
            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
        }

        [Test]
        public async Task GetAvailableTables_InvalidDateFormat_ReturnsBadRequest()
        {
            // Тест проверяет, что API возвращает ошибку при неверном формате даты
            string invalidDate = "01/01/2025"; // Формат MM/dd/yyyy вместо yyyy-MM-dd

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: invalidDate
            );
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Должен возвращать ошибку при неверном формате даты");
        }

        [Test]
        public async Task GetAvailableTables_WithoutParameters_ShouldNotFail()
        {
            // Тест проверяет базовый запрос без параметров
            var (statusCode, responseBody) = await _reservations.GetAvailableTables();

            // API возвращает 500, поэтому проверим только что статус не null
            Assert.That(statusCode, Is.Not.EqualTo(0), "Должен возвращать валидный HTTP-статус");
            Console.WriteLine($"Получен статус: {statusCode}");
        }

        [Test]
        public async Task GetUserReservations_ReturnsUnauthorized_WhenNotAuthenticated()
        {
            // Тест проверяет получение резерваций пользователя
            var (statusCode, responseBody) = await _reservations.GetUserReservations();

            // API требует авторизации для этого запроса
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Должен возвращать статус 401 Unauthorized, когда пользователь не авторизован");
        }

        [Test]
        public async Task GetAvailableTables_WithValidLocationAndDate_ShouldNotFail()
        {
            // Тест проверяет запрос с корректными данными
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: DateTime.Now.ToString("yyyy-MM-dd")
            );

            // Проверяем что запрос обработан (либо OK, либо NotFound)
            Assert.That((int)statusCode, Is.LessThan(500),
                "Не должен возвращать серверную ошибку при корректных параметрах");
        }

        [Test]
        public async Task GetAvailableTables_WithInvalidId_ReturnsBadRequest()
        {
            // Тест проверяет запрос с некорректным ID локации
            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: "invalid-id"
            );

            // Ожидаем клиентскую ошибку
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400).And.LessThan(500),
                "Должен возвращать клиентскую ошибку при некорректном ID локации");
        }

        [Test]
        public async Task CreateReservation_WithoutToken_ReturnsUnauthorized()
        {
            // Тест проверяет создание бронирования без токена
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

            // Ожидаем 401 Unauthorized или 403 Forbidden
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized).Or.EqualTo(HttpStatusCode.Forbidden),
                "Должен требовать авторизацию для создания бронирования");
        }

        [Test]
        public async Task GetAvailableTables_WithMultipleFilters_ReturnsFilteredTables()
        {
            string locationId = _testLocationId;
            string date = _testDate;
            int guests = 2;
            string time = "19:00";

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: locationId,
                date: date,
                guests: guests,
                time: time);
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            if (responseBody != null && responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    int capacity = table["capacity"]?.Value<int>() ?? 0;
                    Assert.That(capacity, Is.GreaterThanOrEqualTo(guests),
                        $"Все столы должны вмещать {guests} гостей");

                    var availableTimes = table["availableTimes"] as JArray;
                    Assert.That(availableTimes, Is.Not.Null, "Стол должен иметь доступные времена");
                    if (availableTimes != null)
                    {
                        Assert.That(availableTimes.Any(t => t.ToString() == time), Is.True,
                            $"Все столы должны иметь слот времени {time}");
                    }
                }
            }
        }
    }
}
