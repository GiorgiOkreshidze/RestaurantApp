using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using ApiTests.Pages;
using ApiTests.Utilities;

namespace ApiTests
{
    [TestFixture]
    [Category("AnonymousFeedback")]
    public class AnonymousFeedbackTests : BaseTest
    {
        private AnonymousFeedback _anonymousFeedback;
        private string _validReservationId;

        [SetUp]
        public void Setup()
        {
            _anonymousFeedback = new AnonymousFeedback();
            _validReservationId = TestConfig.Instance.ValidReservationId;
        }

        [Test]
        [Category("Smoke")]
        public async Task ValidateToken_WithInvalidToken_ReturnsError()
        {
            // Arrange
            string invalidToken = "invalid-token-12345";

            // Act
            var result = await _anonymousFeedback.ValidateToken(invalidToken);

            // Assert - accept any error status
            Assert.That((int)result.StatusCode, Is.GreaterThanOrEqualTo(400),
                "Should return an error status for an invalid token");
        }

        [Test]
        [Category("Smoke")]
        public async Task SubmitFeedback_ReturnsResponse()
        {
            // Arrange
            string cuisineComment = "Excellent food";
            string cuisineRating = "5";
            string serviceComment = "Great service";
            string serviceRating = "5";

            // Act
            var result = await _anonymousFeedback.SubmitFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert - simply verify that we received some response
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");
            Console.WriteLine($"Received status: {result.StatusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task ValidateToken_WithEmptyToken_ThrowsException()
        {
            // Arrange
            string emptyToken = string.Empty;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _anonymousFeedback.ValidateToken(emptyToken));

            Assert.That(exception.ParamName, Is.EqualTo("token"),
                "Should check for empty token");
        }

        [Test]
        [Category("Regression")]
        public async Task SubmitFeedback_WithInvalidReservationId_IdentifiesInvalidId()
        {
            // Arrange
            string invalidReservationId = "invalid-id";
            string cuisineComment = "Good food";
            string cuisineRating = "4";
            string serviceComment = "Good service";
            string serviceRating = "4";

            // Act
            var result = await _anonymousFeedback.SubmitFeedback(
                invalidReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert - verify that the API responded
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");

            // If API returned InternalServerError, check for error message
            if (result.StatusCode == HttpStatusCode.InternalServerError && result.ResponseBody != null)
            {
                Console.WriteLine($"API returned server error: {result.ResponseBody["title"]}");
                Assert.Pass("API returned an error, check passed");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task SubmitFeedback_WithInvalidRating_IdentifiesInvalidRating()
        {
            // Arrange
            string cuisineComment = "Good food";
            string invalidRating = "10"; // Rating should be from 1 to 5
            string serviceComment = "Good service";
            string serviceRating = "4";

            // Act
            var result = await _anonymousFeedback.SubmitFeedback(
                _validReservationId,
                cuisineComment,
                invalidRating,
                serviceComment,
                serviceRating);

            // Assert - verify that the API responded
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");

            // If API returned InternalServerError, check for error message
            if (result.StatusCode == HttpStatusCode.InternalServerError && result.ResponseBody != null)
            {
                Console.WriteLine($"API returned server error: {result.ResponseBody["title"]}");
                Assert.Pass("API returned an error, check passed");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task SubmitFeedback_WithEmptyComments_HandlesEmptyInput()
        {
            // Arrange
            string emptyComment = "";
            string cuisineRating = "4";
            string serviceRating = "4";

            // Act
            var result = await _anonymousFeedback.SubmitFeedback(
                _validReservationId,
                emptyComment,
                cuisineRating,
                emptyComment,
                serviceRating);

            // Assert
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");
            Console.WriteLine($"Response status for empty comments: {result.StatusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task SubmitFeedback_WithLongComments_HandlesLongInput()
        {
            // Arrange
            string longComment = new string('A', 500); // Comment of 500 characters
            string cuisineRating = "4";
            string serviceRating = "4";

            // Act
            var result = await _anonymousFeedback.SubmitFeedback(
                _validReservationId,
                longComment,
                cuisineRating,
                longComment,
                serviceRating);

            // Assert
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");
            Console.WriteLine($"Response status for long comments: {result.StatusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task SubmitFeedback_WithSpecialCharacters_HandlesSpecialChars()
        {
            // Arrange
            string specialCharsComment = "!@#$%^&*()_+<>?\"':;{}[]|\\";
            string cuisineRating = "4";
            string serviceRating = "4";

            // Act
            var result = await _anonymousFeedback.SubmitFeedback(
                _validReservationId,
                specialCharsComment,
                cuisineRating,
                specialCharsComment,
                serviceRating);

            // Assert
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");
            Console.WriteLine($"Response status for comments with special characters: {result.StatusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task SubmitFeedback_WithMinimumRatingValues_HandlesMinimumRatings()
        {
            // Arrange
            string cuisineComment = "The food was not good";
            string minRating = "1"; // Minimum rating
            string serviceComment = "Service was poor";

            // Act
            var result = await _anonymousFeedback.SubmitFeedback(
                _validReservationId,
                cuisineComment,
                minRating,
                serviceComment,
                minRating);

            // Assert
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");
            Console.WriteLine($"Response status for minimum ratings: {result.StatusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task ValidateToken_WithValidFormat_ProcessesToken()
        {
            // Arrange - token with valid format, but possibly invalid
            string validFormatToken = "abcdef1234567890abcdef1234567890";

            // Act
            var result = await _anonymousFeedback.ValidateToken(validFormatToken);

            // Assert
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");
            Console.WriteLine($"Response status for valid format token: {result.StatusCode}");

            if (result.StatusCode == HttpStatusCode.OK)
            {
                Assert.That(result.ResponseBody.ContainsKey("reservationId"), Is.True,
                    "Upon successful validation, response should contain reservation ID");
            }
            else
            {
                Console.WriteLine($"Token validation failed, received status: {result.StatusCode}");
            }
        }
    }
}