using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ApiTests.Pages;
using ApiTests.Utilities;

namespace ApiTests
{
    [TestFixture]
    [Category("Feedbacks")]
    public class FeedbackTests : BaseTest
    {
        private Feedback _feedback;
        private Authentication _auth;
        private string _validReservationId;
        private string _idToken;

        [SetUp]
        public async Task Setup()
        {
            _feedback = new Feedback();
            _auth = new Authentication();
            _validReservationId = Config.ValidReservationId;

            // Get authentication token
            var (statusCode, responseBody) = await _auth.LoginUser(Config.TestUserEmail, Config.TestUserPassword);

            if (statusCode == HttpStatusCode.OK && responseBody != null && responseBody.ContainsKey("idToken"))
            {
                _idToken = responseBody["idToken"].ToString();
                Console.WriteLine("Authentication successful, token received");
            }
            else
            {
                Console.WriteLine($"Warning: Failed to log in: {statusCode}");
                // Don't fail the test here to check behavior without a token
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
                serviceRating,
                _idToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");
            Assert.That(responseBody, Is.Not.Null, "Response body should not be null");

            // Verify success message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("success").IgnoreCase.Or.Contains("added").IgnoreCase,
                    "Response should contain success message");
            }

            Console.WriteLine("Feedback successfully created");
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
                serviceRating,
                _idToken);

            // Assert
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "Should return error code with invalid reservation ID");

            // Verify error message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("reservation").IgnoreCase.Or.Contains("id").IgnoreCase,
                    "Error message should indicate issue with reservation ID");
            }

            Console.WriteLine($"Received expected error status: {statusCode}");
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
                serviceRating,
                _idToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Should return 200 OK status");

            // Verify success message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("success").IgnoreCase.Or.Contains("updated").IgnoreCase,
                    "Response should contain success message");
            }

            Console.WriteLine("Feedback successfully updated");
        }

        [Test]
        public async Task CreateFeedback_WithInvalidRating_ReturnsError()
        {
            // Arrange
            string cuisineComment = "Good food";
            string invalidRating = "10"; // Rating should be between 1 and 5
            string serviceComment = "Good service";
            string serviceRating = "3";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                invalidRating,
                serviceComment,
                serviceRating,
                _idToken);

            // Assert
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "Should return error code with invalid rating value");

            // Verify error message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("rating").IgnoreCase,
                    "Error message should indicate issue with rating value");
            }

            Console.WriteLine($"Received expected error status: {statusCode}");
        }

        [Test]
        public async Task GetFeedbacksByReservationId_WithEmptyId_ReturnsBadRequest()
        {
            // Arrange
            string emptyReservationId = "";

            // Act
            var (statusCode, responseBody) = await _feedback.GetFeedbacksByReservationId(emptyReservationId, _idToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "Should return 400 Bad Request status with empty reservation ID");

            // Verify error message
            if (responseBody != null)
            {
                JObject errorObj = JObject.Parse(responseBody.ToString());
                if (errorObj["message"] != null)
                {
                    Assert.That(errorObj["message"].ToString(),
                        Contains.Substring("empty").IgnoreCase.Or.Contains("reservation").IgnoreCase,
                        "Error message should indicate issue with empty ID");
                }
            }

            Console.WriteLine("Received expected Bad Request status with empty ID");
        }

        [Test]
        public async Task CreateFeedback_WithMissingComments_ReturnsSuccess()
        {
            // Arrange - ratings only, no comments
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
                serviceRating,
                _idToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should accept feedback with ratings only, no comments");

            // Verify success message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("success").IgnoreCase.Or.Contains("added").IgnoreCase,
                    "Response should contain success message");
            }

            Console.WriteLine("Feedback without comments successfully created");
        }

        [Test]
        public async Task CreateFeedback_WithLowRatings_ReturnsSuccess()
        {
            // Arrange - minimum ratings
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
                serviceRating,
                _idToken);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should accept feedback with minimum ratings");

            // Verify success message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("success").IgnoreCase.Or.Contains("added").IgnoreCase,
                    "Response should contain success message");
            }

            Console.WriteLine("Feedback with low ratings successfully created");
        }

        [Test]
        public async Task CreateFeedback_WithNonNumericRating_ReturnsError()
        {
            // Arrange - non-numeric rating
            string cuisineComment = "Good food";
            string cuisineRating = "excellent";  // Non-numeric value
            string serviceComment = "Good service";
            string serviceRating = "3";

            // Act
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating,
                _idToken);

            // Assert
            Assert.That((int)statusCode, Is.GreaterThanOrEqualTo(400),
                "Should return error with non-numeric rating value");

            // Verify error message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("rating").IgnoreCase.Or.Contains("number").IgnoreCase,
                    "Error message should indicate issue with rating format");
            }

            Console.WriteLine($"Received expected error status: {statusCode}");
        }

        [Test]
        public async Task CreateFeedback_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            string cuisineComment = "Great food";
            string cuisineRating = "4";
            string serviceComment = "Good service";
            string serviceRating = "5";

            // Act - explicitly pass null to avoid using token
            var (statusCode, responseBody) = await _feedback.CreateFeedback(
                _validReservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating,
                null);

            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Should return 401 Unauthorized status without authentication");

            // Verify error message
            if (responseBody != null && responseBody.ContainsKey("message"))
            {
                Assert.That(responseBody["message"].ToString(),
                    Contains.Substring("unauthorized").IgnoreCase.Or.Contains("authentication").IgnoreCase,
                    "Error message should indicate authentication issue");
            }

            Console.WriteLine("Received expected Unauthorized status");
        }
    }
}
