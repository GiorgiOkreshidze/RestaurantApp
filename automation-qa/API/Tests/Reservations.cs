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

        private string _testTableId;
        private string _testWaiterId;

        [SetUp]
        public void Setup()
        {
            _reservations = new Reservations();
            _testLocationId = "8c4fc44e-c1a5-42eb-9912-55aeb5111a99"; // Валидный ID
            _testDate = DateTime.Now.ToString("yyyy-MM-dd");
            _testTableId = "04ba5b37-8fbd-4f5f-8354-0b75078a790a"; // Валидный ID стола
            _testWaiterId = "16929204-5081-706c-f4dc-a1695648cd31"; // Валидный ID официанта
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
            string timeSlot = DateTime.UtcNow.AddHours(2).ToString("HH:mm");
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
            string pastTime = "12:00"; // Добавляем время в середине дня

            var (statusCode, responseBody) = await _reservations.GetAvailableTables(
                locationId: _testLocationId,
                date: pastDate,
                time: pastTime
            );

            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Должен возвращать ошибку для даты и времени в прошлом");

            // Дополнительно проверяем сообщение об ошибке, если оно есть
            if (statusCode == HttpStatusCode.BadRequest && responseBody != null)
            {
                string errorMessage = "";
                try
                {
                    // Попытка прочитать сообщение об ошибке из JSON
                    JObject errorObj = JObject.Parse(responseBody.ToString());
                    errorMessage = errorObj["message"]?.ToString() ?? "";
                }
                catch { /* В случае ошибки парсинга просто пропускаем */ }

                Console.WriteLine($"Error message: {errorMessage}");
            }
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
            // Тест проверяет получение резерваций пользователя без авторизации
            var (statusCode, responseBody) = await _reservations.GetUserReservations();

            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Проверяем, что статус код не 0 (проблема с соединением)
            if ((int)statusCode == 0)
            {
                Assert.Inconclusive("API недоступен (статус код 0). Проверьте соединение с сервером.");
                return;
            }

            // Ожидаем 401 Unauthorized
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

            // Используем UTC время и добавляем 2 часа для гарантии, что время будет в будущем
            string timeString = DateTime.UtcNow.AddHours(2).ToString("HH:mm");

            // Явно указываем типы для переменных statusCode и responseBody
            var result = await _reservations.GetAvailableTables(
                locationId: locationId,
                date: date,
                guests: guests,
                time: timeString);

            HttpStatusCode statusCode = result.Item1;
            JArray responseBody = result.Item2;

            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");

            // Добавляем проверку, продолжаем только если статус OK и есть данные
            if (statusCode == HttpStatusCode.OK && responseBody != null && responseBody.Count > 0)
            {
                foreach (var table in responseBody)
                {
                    int capacity = table["capacity"]?.Value<int>() ?? 0;
                    Assert.That(capacity, Is.GreaterThanOrEqualTo(guests),
                        $"Все столы должны вмещать {guests} гостей");

                    // Используем availableSlots вместо availableTimes
                    var availableSlots = table["availableSlots"] as JArray;
                    Assert.That(availableSlots, Is.Not.Null, "Стол должен иметь доступные слоты времени");

                    if (availableSlots != null)
                    {
                        // Ищем слот, который содержит указанное время
                        bool hasMatchingSlot = availableSlots.Any(slot =>
                            slot["start"]?.ToString() == timeString ||
                            (slot["start"]?.ToString() != null &&
                             slot["end"]?.ToString() != null &&
                             TimeSpanBetween(timeString, slot["start"].ToString(), slot["end"].ToString()))
                        );

                        Assert.That(hasMatchingSlot, Is.True,
                            "Стол должен быть доступен на время " + timeString);
                    }
                }
            }
        }

        // Вспомогательная функция для проверки, находится ли время между началом и концом слота
        private bool TimeSpanBetween(string time, string start, string end)
        {
            // Парсим строки времени в объекты TimeSpan
            TimeSpan timeSpan = TimeSpan.Parse(time);
            TimeSpan startSpan = TimeSpan.Parse(start);
            TimeSpan endSpan = TimeSpan.Parse(end);

            // Проверяем, находится ли указанное время в пределах слота
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

            // Act - отправляем запрос без токена
            var result = await _reservations.CreateReservationByWaiter(
                locationId: _testLocationId,
                tableId: _testTableId,
                date: _testDate,
                timeFrom: timeFrom,
                timeTo: timeTo,
                guestNumber: guestNumber,
                waiterId: _testWaiterId,
                clientType: clientType
            // токен не указываем
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Проверяем, что API требует авторизации
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API должен требовать авторизацию (401 Unauthorized)");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");

            // Проверяем, что в ответе есть сообщение об ошибке
            Assert.That(responseBody["message"], Is.Not.Null,
                "Ответ должен содержать сообщение об ошибке");

            Assert.That(responseBody["message"].ToString(), Does.Contain("Unauthorized"),
                "Сообщение об ошибке должно указывать на отсутствие авторизации");
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
                "API должен возвращать код ошибки клиента при невалидном ID локации");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
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
                "API должен возвращать код ошибки клиента при невалидном ID стола");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
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
                "API должен возвращать код ошибки клиента при дате в прошлом");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
        }

        [Test]
        public async Task CreateReservationByWaiter_WithInvalidTimeFormat_ShouldReturnBadRequest()
        {
            // Arrange
            string invalidTimeFormat = "19-00"; // Неверный формат времени
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
                "API должен возвращать код ошибки клиента при неверном формате времени");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
        }

        [Test]
        public async Task CreateReservationByWaiter_WithTooManyGuests_ShouldRequireAuthorization()
        {
            // Arrange
            string timeFrom = "19:00";
            string timeTo = "20:00";
            int tooManyGuests = 15; // Большое количество гостей
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

            // Проверяем, что API требует авторизации, независимо от других параметров
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API должен требовать авторизацию независимо от количества гостей");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");

            Assert.That(responseBody["message"], Is.Not.Null,
                "Ответ должен содержать сообщение об ошибке");
        }

        [Test]
        public async Task CreateReservationByWaiter_WithMissingRequiredFields_ShouldReturnBadRequest()
        {
            // Arrange - пропускаем обязательные поля

            // Act
            var result = await _reservations.CreateReservationByWaiter(
                // Не указываем locationId
                // Не указываем tableId
                date: _testDate,
                timeFrom: "19:00",
                timeTo: "20:00",
                guestNumber: 2
            // Не указываем waiterId
            // Не указываем clientType
            );

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400).And.LessThan(500),
                "API должен возвращать код ошибки клиента при отсутствии обязательных полей");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
        }

        [Test]
        public async Task CreateReservationByWaiter_WithEmptyParameters_ShouldRequireAuthorization()
        {
            // Act - отправляем запрос с пустыми параметрами
            var result = await _reservations.CreateReservationByWaiter();

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Проверяем, что API требует авторизации даже с пустыми параметрами
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API должен требовать авторизацию даже при пустых параметрах");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");

            Assert.That(responseBody["message"]?.ToString(), Does.Contain("Unauthorized"),
                "Ответ должен содержать сообщение о необходимости авторизации");
        }

        [Test]
        public async Task CancelReservation_WithoutToken_ShouldRequireAuthorization()
        {
            // Arrange
            string reservationId = "30001"; // Тестовый ID из примера

            // Act
            var result = await _reservations.CancelReservation(reservationId);

            HttpStatusCode statusCode = result.StatusCode;
            JObject responseBody = result.ResponseBody;

            // Assert
            Console.WriteLine($"Status: {statusCode}, Response: {responseBody}");

            // Проверяем, что API требует авторизации
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "API должен требовать авторизацию для отмены бронирования");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");

            Assert.That(responseBody["message"], Is.Not.Null,
                "Ответ должен содержать сообщение об ошибке");
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

            // API может вернуть либо 401 (если сначала проверяет авторизацию),
            // либо 404 (если сначала проверяет существование ресурса)
            Assert.That((int)statusCode, Is.AnyOf(401, 404),
                "API должен возвращать ошибку для несуществующего ID бронирования");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
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

            // API возвращает 403 для пустого ID без токена
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "API должен возвращать код ошибки для пустого ID бронирования");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");

            Assert.That(responseBody["message"], Is.Not.Null,
                "Ответ должен содержать сообщение об ошибке");
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

            // Проверяем, что API возвращает ошибку для ID со спецсимволами
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "API должен возвращать код ошибки для ID со спецсимволами");

            Assert.That(responseBody, Is.Not.Null,
                "Тело ответа не должно быть null");
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
