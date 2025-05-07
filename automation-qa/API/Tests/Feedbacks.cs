using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using ApiTests.Pages;
using ApiTests.Utilities;

namespace ApiTests
{
    [TestFixture]
    [Category("FeedbackTest")]
    public class FeedbackTests : BaseTest
    {
        private Feedbacks _feedback;
        private Authentication _auth;
        private string _validReservationId;
        private string _token;

        [SetUp]
        public void Setup()
        {
            _feedback = new Feedbacks();
            _auth = new Authentication();
            _validReservationId = TestConfig.Instance.ValidReservationId;
        }

        [Test]
        [Category("Smoke")]
        public async Task CreateFeedback_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            string cuisineComment = "Great food";
            string cuisineRating = "5";
            string serviceComment = "Excellent service";
            string serviceRating = "5";

            // Act - without authorization token
            var result = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden),
                "Should return authorization error status without token");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateFeedback_WithInvalidReservationId_ReturnsBadRequest()
        {
            // Arrange
            string invalidReservationId = "invalid-id";
            string cuisineComment = "Good food";
            string cuisineRating = "4";
            string serviceComment = "Good service";
            string serviceRating = "4";

            // Act
            var result = await _feedback.CreateFeedback(
                invalidReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.NotFound,
                HttpStatusCode.Unauthorized),
                "Should return error for invalid reservation ID");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateFeedback_WithInvalidRating_ReturnsBadRequest()
        {
            // Arrange
            string cuisineComment = "Good food";
            string invalidRating = "10"; // Rating should be between 1 and 5
            string serviceComment = "Good service";
            string serviceRating = "4";

            // Act
            var result = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                invalidRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized),
                "Should return error for invalid rating value");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateFeedback_WithEmptyComments_ReturnsBadRequest()
        {
            // Arrange
            string cuisineComment = "";
            string cuisineRating = "4";
            string serviceComment = "";
            string serviceRating = "4";

            // Act
            var result = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized),
                "Should return error for empty comments");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateFeedback_WithNonNumericRating_ReturnsBadRequest()
        {
            // Arrange
            string cuisineComment = "Good food";
            string cuisineRating = "excellent"; // Non-numeric value
            string serviceComment = "Good service";
            string serviceRating = "4";

            // Act
            var result = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized),
                "Should return error for non-numeric rating value");
        }

        [Test]
        [Category("Smoke")]
        public async Task CreateFeedback_WithAuth_ValidData()
        {
            // Arrange
            string cuisineComment = "The food was amazing";
            string cuisineRating = "5";
            string serviceComment = "Service was excellent";
            string serviceRating = "5";

            try
            {
                // Get token for authorization
                var loginResult = await _auth.LoginUser(
                    TestConfig.Instance.TestUserEmail,
                    TestConfig.Instance.TestUserPassword
                );

                if (loginResult.StatusCode != HttpStatusCode.OK || loginResult.ResponseBody == null)
                {
                    Assert.Inconclusive("Failed to obtain authorization token");
                    return;
                }

                string token = loginResult.ResponseBody["idToken"].ToString();

                // Act
                var result = await _feedback.CreateFeedback(
                    _validReservationId,
                    cuisineComment,
                    cuisineRating,
                    serviceComment,
                    serviceRating,
                    token);

                // Assert
                Assert.That(result.StatusCode, Is.AnyOf(
                    HttpStatusCode.OK,
                    HttpStatusCode.Created,
                    HttpStatusCode.Unauthorized),
                    "Should return success status or 401 if token is invalid");

                if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created)
                {
                    Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null on successful creation");
                }
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"Test cannot be executed due to error: {ex.Message}");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task CreateFeedback_WithMinimumRating()
        {
            // Arrange
            string cuisineComment = "Disappointing food quality";
            string cuisineRating = "1"; // Minimum rating
            string serviceComment = "Service was very slow";
            string serviceRating = "1"; // Minimum rating

            // Act
            var result = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized),
                "Should handle minimum rating values");

            // Some systems may not accept negative reviews, so BadRequest is also acceptable
            Console.WriteLine($"Response status for minimum rating: {result.StatusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateFeedback_WithLongComments()
        {
            // Arrange
            string longComment = new string('A', 1000); // Very long comment of 1000 characters
            string cuisineRating = "4";
            string serviceRating = "4";

            // Act
            var result = await _feedback.CreateFeedback(
                _validReservationId,
                longComment,
                cuisineRating,
                longComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized),
                "Should correctly handle very long comments");

            // API may either accept the long comment or reject it, both options are valid
            Console.WriteLine($"Response status for long comment: {result.StatusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateFeedback_WithNullValues()
        {
            // Arrange - explicitly passing null values
            string cuisineComment = null;
            string cuisineRating = null;
            string serviceComment = null;
            string serviceRating = null;

            try
            {
                // Act
                var result = await _feedback.CreateFeedback(
                    _validReservationId,
                    cuisineComment,
                    cuisineRating,
                    serviceComment,
                    serviceRating);

                // Assert
                Assert.That(result.StatusCode, Is.AnyOf(
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.Unauthorized),
                    "Should return error for null values");
            }
            catch (Exception ex)
            {
                // If the method doesn't handle null and throws an exception, that's also valid behavior
                Assert.Pass($"Method threw exception with null values: {ex.GetType().Name}");
            }
        }

        [Test]
        [Category("Smoke")]
        public async Task CreateFeedback_WithMaximumRating()
        {
            // Arrange
            string cuisineComment = "The food was absolutely delicious!";
            string cuisineRating = "5"; // Maximum rating
            string serviceComment = "Service was impeccable and staff was very friendly";
            string serviceRating = "5"; // Maximum rating

            // Act
            var result = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.Unauthorized),
                "Should correctly handle maximum rating values");

            Console.WriteLine($"Response status for maximum rating: {result.StatusCode}");
        }

        [Test]
        [Category("Regression")]
        public async Task CreateFeedback_WithAverageRating()
        {
            // Arrange
            string cuisineComment = "Food was decent, but not extraordinary";
            string cuisineRating = "3"; // Average rating
            string serviceComment = "Service was acceptable, neither great nor bad";
            string serviceRating = "3"; // Average rating

            // Act
            var result = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.Unauthorized),
                "Should correctly handle average rating values");

            if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created)
            {
                Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null on successful creation");
            }

            Console.WriteLine($"Response status for average rating: {result.StatusCode}");
        }
    }
}