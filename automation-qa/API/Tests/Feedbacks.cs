using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;

namespace ApiTests
{
    [TestFixture]
    [Category("Feedback")]
    public class FeedbackTests : BaseTest
    {
        private Feedback _feedback;
        private Authentication _auth;
        private readonly string _validReservationId = "43a479ef-4aa0-4472-b72e-7833f90a591";
        private string _idToken;

        [SetUp]
        public async Task Setup()
        {
            _feedback = new Feedback();
            _auth = new Authentication();

            // Получаем токен аутентификации
            var (statusCode, responseBody) = await _auth.LoginUser("test@example.com", "StrongP@ss123!");

            if (statusCode == HttpStatusCode.OK && responseBody != null && responseBody.ContainsKey("idToken"))
            {
                _idToken = responseBody["idToken"].ToString();
                Console.WriteLine("Авторизация успешна, токен получен");
            }
            else
            {
                Console.WriteLine($"Предупреждение: Не удалось выполнить вход: {statusCode}");
                // Не фейлим тест здесь, чтобы проверить поведение без токена
            }
        }

        [Test]
        public async Task CreateFeedback_WithValidData_ReturnsSuccess()
        {
            // Arrange
            string cuisineComment = "Excellent cuisine! Over the top!";
            string cuisineRating = "4";
            string serviceComment = "I like it very much";
            string serviceRating = "4";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Assert.That(responseBody, Is.Not.Null, "Тело ответа не должно быть null");
            Console.WriteLine("Отзыв успешно создан");
        }

        [Test]
        public async Task CreateFeedback_WithInvalidReservationId_ReturnsError()
        {
            // Arrange
            string invalidReservationId = "invalid-id";
            string cuisineComment = "Good food";
            string cuisineRating = "3";
            string serviceComment = "Good service";
            string serviceRating = "3";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                invalidReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "Должен возвращать код ошибки при недействительном ID бронирования");
            Console.WriteLine($"Получен ожидаемый статус ошибки: {statusCode}");
        }

        [Test]
        public async Task UpdateFeedback_WithValidData_ReturnsSuccess()
        {
            // Arrange
            string cuisineComment = "Updated comment";
            string cuisineRating = "5";
            string serviceComment = "Service improved";
            string serviceRating = "5";

            // Act
            var (statusCode, responseBody) = await _feedback.UpdateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Должен возвращать статус 200 OK");
            Console.WriteLine("Отзыв успешно обновлен");
        }

        [Test]
        public async Task CreateFeedback_WithInvalidRating_ReturnsError()
        {
            // Arrange
            string cuisineComment = "Good food";
            string invalidRating = "10"; // Рейтинг должен быть от 1 до 5
            string serviceComment = "Good service";
            string serviceRating = "3";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                invalidRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "Должен возвращать код ошибки при недопустимом значении рейтинга");
            Console.WriteLine($"Получен ожидаемый статус ошибки: {statusCode}");
        }

        [Test]
        public async Task GetFeedbacksByReservationId_WithEmptyId_ReturnsBadRequest()
        {
            // Arrange
            string emptyReservationId = "";

            // Act
            var (statusCode, responseBody) = await _feedback.GetFeedbacksByReservationId(emptyReservationId);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Должен возвращать статус 400 Bad Request при пустом ID бронирования");
            Console.WriteLine("Получен ожидаемый статус Bad Request при пустом ID");
        }

        [Test]
        public async Task CreateFeedback_WithMissingComments_ReturnsSuccess()
        {
            // Arrange - только рейтинги, без комментариев
            string cuisineComment = "";
            string cuisineRating = "4";
            string serviceComment = "";
            string serviceRating = "5";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Должен принимать отзыв без комментариев, только с рейтингами");
            Console.WriteLine("Отзыв без комментариев успешно создан");
        }

        [Test]
        public async Task CreateFeedback_WithLowRatings_ReturnsSuccess()
        {
            // Arrange - минимальные рейтинги
            string cuisineComment = "Poor food quality";
            string cuisineRating = "1";
            string serviceComment = "Very slow service";
            string serviceRating = "1";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Должен принимать отзыв с минимальными рейтингами");
            Console.WriteLine("Отзыв с низкими рейтингами успешно создан");
        }

        [Test]
        public async Task CreateFeedback_WithNonNumericRating_ReturnsError()
        {
            // Arrange - нечисловой рейтинг
            string cuisineComment = "Good food";
            string cuisineRating = "excellent";  // Нечисловое значение
            string serviceComment = "Good service";
            string serviceRating = "3";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "Должен возвращать ошибку при нечисловом значении рейтинга");
            Console.WriteLine($"Получен ожидаемый статус ошибки: {statusCode}");
        }

        [Test]
        public async Task CreateFeedback_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            string cuisineComment = "Great food";
            string cuisineRating = "4";
            string serviceComment = "Good service";
            string serviceRating = "5";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Должен возвращать статус 401 Unauthorized без аутентификации");
            Console.WriteLine("Получен ожидаемый статус Unauthorized");
        }
    }
}
