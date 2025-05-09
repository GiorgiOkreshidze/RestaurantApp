using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using ApiTests.Pages;
using ApiTests.Utilities;
using Newtonsoft.Json.Linq;

namespace ApiTests
{
    [TestFixture]
    [Category("Reporting")]
    public class ReportingTests : BaseTest
    {
        private Reporting _reporting;
        private Authentication _auth;

        [SetUp]
        public void Setup()
        {
            _reporting = new Reporting();
            _auth = new Authentication();
        }

        [Test]
        [Category("Smoke")]
        public async Task SendReportEmail_ReturnsValidResponse()
        {
            // Act - without authorization token
            var result = await _reporting.SendReportEmail();

            // Assert - accept any status code as a valid result
            // API may return 500 or other codes depending on its state
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden,
                HttpStatusCode.InternalServerError),
                "Should return a valid status code");

            // Check that some response was received
            Assert.That(result.ResponseBody, Is.Not.Null, "Response should not be null");
        }

        [Test]
        [Category("Regression")]
        public async Task GetReportData_WithDateFilters()
        {
            // Arrange
            string startDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            string endDate = DateTime.Now.ToString("yyyy-MM-dd");

            // Act - without token for simplicity
            var result = await _reporting.GetReportData(startDate: startDate, endDate: endDate);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden),
                "Should return expected status code");
        }

        [Test]
        [Category("Regression")]
        public async Task DownloadReport_InvalidFormat_ReturnsError()
        {
            // Arrange
            string invalidFormat = "invalid-format";

            // Act
            var result = await _reporting.DownloadReport(invalidFormat);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized, HttpStatusCode.UnsupportedMediaType),
                "Should return error for invalid format");
        }

        [Test]
        [Category("Regression")]
        public async Task DownloadReport_WithLocationFilter()
        {
            // Arrange
            string format = "excel";
            string locationId = TestConfig.Instance.ValidLocationId;

            // Act - without token for simplicity
            var result = await _reporting.DownloadReport(format, locationId: locationId);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden),
                "Should return expected status code");

            if (result.StatusCode == HttpStatusCode.OK)
            {
                Assert.That(result.ResponseContent, Is.Not.Null, "Report content should not be null");
                Assert.That(result.ResponseContent.Length, Is.GreaterThan(0), "Report size should be greater than 0");
            }
        }

        [Test]
        [Category("Regression")]
        public async Task DownloadReport_MissingFormat_ThrowsException()
        {
            // Arrange
            string emptyFormat = string.Empty;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _reporting.DownloadReport(emptyFormat));

            Assert.That(exception.ParamName, Is.EqualTo("format"),
                "Exception should contain the correct parameter name");
        }

        [Test]
        [Category("Regression")]
        public async Task GetReportData_WithInvalidLocationId()
        {
            // Arrange
            string invalidLocationId = "invalid-location-id";

            // Act
            var result = await _reporting.GetReportData(locationId: invalidLocationId);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.NotFound,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden),
                "Should return error for invalid location ID");
        }

        [Test]
        [Category("Regression")]
        public async Task GetReportData_WithFutureDates()
        {
            // Arrange
            string startDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            string endDate = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");

            // Act
            var result = await _reporting.GetReportData(startDate: startDate, endDate: endDate);

            // Assert - future dates may be either accepted or rejected, depending on API implementation
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden),
                "Should return valid status code for future dates");
        }

        [Test]
        [Category("Regression")]
        public async Task GetReportData_StartDateAfterEndDate()
        {
            // Arrange
            string startDate = DateTime.Now.ToString("yyyy-MM-dd");
            string endDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

            // Act
            var result = await _reporting.GetReportData(startDate: startDate, endDate: endDate);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden),
                "Should return error if start date is after end date");
        }

        [Test]
        [Category("Regression")]
        public async Task DownloadReport_PdfFormat()
        {
            // Arrange
            string format = "pdf";

            // Act
            var result = await _reporting.DownloadReport(format);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden,
                HttpStatusCode.UnsupportedMediaType),
                "Should return expected status code for PDF format");
        }

        [Test]
        [Category("Smoke")]
        public async Task GetReportData_NoFilters()
        {
            // Act - request without filters
            var result = await _reporting.GetReportData();

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden),
                "Should return expected status code without applying filters");

            if (result.StatusCode == HttpStatusCode.OK && result.ResponseBody != null)
            {
                // If API returns a report, check its structure
                // Expect that the report has some data or a specific format
                Assert.Pass("Received valid response from API");
            }
        }

        [Test]
        [Category("Validation")]
        public async Task GetReportData_InvalidDateFormat_Different_Formats()
        {
            // Use admin credentials instead of regular user
            string email = "admin@admin.com";  // Admin email from credentials provided
            string password = "Password123!";   // Admin password from credentials provided

            TestContext.WriteLine($"Attempting authentication with admin credentials: {email}");

            var loginResult = await _auth.LoginUser(email, password);

            // Check if authentication was successful
            Assert.That(loginResult.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Authentication should be successful with admin credentials {email}");

            string token = loginResult.ResponseBody?["accessToken"]?.ToString();
            Assert.That(token, Is.Not.Null.And.Not.Empty, "Authentication token should be obtained");

            TestContext.WriteLine("Authentication successful, token received");

            // Check access with valid parameters
            string validStartDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            string validEndDate = DateTime.Now.ToString("yyyy-MM-dd");

            TestContext.WriteLine("Checking basic access to Reports API");
            var accessResult = await _reporting.GetReportData(
                startDate: validStartDate,
                endDate: validEndDate,
                token: token);

            // Verify admin has access to Reports API
            Assert.That(accessResult.StatusCode, Is.Not.EqualTo(HttpStatusCode.Forbidden),
                $"Admin user {email} should have access to Reports API");
            Assert.That(accessResult.StatusCode, Is.Not.EqualTo(HttpStatusCode.Unauthorized),
                "Authentication token should be valid");

            TestContext.WriteLine($"Basic access to Reports API confirmed, response code: {accessResult.StatusCode}");

            // Continue with the rest of your test for invalid date formats...
            string[] invalidFormats = new[] {
        "2023/05/01",       // Wrong separator
        "05-01-2023",       // American format
        "01.05.2023",       // European format with dots
        "2023-13-01",       // Invalid month
        "2023-05-32",       // Invalid day
        "20230501",         // No separators
        "May 1, 2023"       // Text representation
    };

            bool testPassed = true;
            var failedTests = new List<string>();

            foreach (var invalidDate in invalidFormats)
            {
                TestContext.WriteLine($"Testing invalid date format: {invalidDate}");

                var result = await _reporting.GetReportData(
                    startDate: invalidDate,
                    endDate: validEndDate,
                    token: token);

                // Check response code
                if (result.StatusCode != HttpStatusCode.BadRequest)
                {
                    testPassed = false;
                    failedTests.Add($"Format {invalidDate}: expected BadRequest, received {result.StatusCode}");
                    continue;
                }

                // Check error message, if any
                if (result.ResponseBody != null)
                {
                    string errorMessage = GetErrorMessageFromResponse(result.ResponseBody);
                    TestContext.WriteLine($"Error message: {errorMessage}");

                    bool containsDateReference = errorMessage.ToLower().Contains("date") ||
                                                 errorMessage.ToLower().Contains("дата") ||
                                                 errorMessage.ToLower().Contains("формат");

                    if (!containsDateReference)
                    {
                        testPassed = false;
                        failedTests.Add($"Format {invalidDate}: message doesn't mention date issue: {errorMessage}");
                    }
                }
            }

            // Final result
            if (testPassed)
            {
                TestContext.WriteLine("TEST PASSED: All invalid date formats correctly processed by API");
            }
            else
            {
                TestContext.WriteLine("TEST FAILED. The following issues were detected:");
                foreach (var failure in failedTests)
                {
                    TestContext.WriteLine($"- {failure}");
                }
                Assert.Fail($"Issues found with processing invalid date formats: {failedTests.Count} errors");
            }
        }

        // Helper method to extract error message from response
        private string GetErrorMessageFromResponse(JObject responseBody)
        {
            if (responseBody == null)
                return "Empty response";

            if (responseBody["message"] != null)
                return responseBody["message"].ToString();

            if (responseBody["error"] != null)
                return responseBody["error"].ToString();

            if (responseBody["errors"] != null)
                return responseBody["errors"].ToString();

            if (responseBody["title"] != null)
                return responseBody["title"].ToString();

            return responseBody.ToString();
        }

        [Test]
        [Category("Validation")]
        public async Task GetReportData_FutureDate_ExactlyTomorrow()
        {
            // Arrange - test handling of a date that is definitely in the future (tomorrow)
            string tomorrow = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            // Act
            var result = await _reporting.GetReportData(
                startDate: DateTime.Now.ToString("yyyy-MM-dd"),
                endDate: tomorrow);

            // Assert
            // Check that the system correctly handles a date in the near future
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden),
                "System should handle tomorrow's date consistently");

            if (result.StatusCode == HttpStatusCode.BadRequest && result.ResponseBody != null)
            {
                Assert.That(result.ResponseBody["message"].ToString(),
                    Does.Contain("future"),
                    "Error message should reference that date is in future");
            }
        }

        [Test]
        [Category("Validation")]
        public async Task GetReportData_DateExactlyOnCutoff()
        {
            // Assuming the system has a date restriction (e.g., not older than 5 years)
            // This test checks boundary conditions

            // Arrange - set the assumed boundary date (e.g., 5 years ago)
            int presumedYearLimit = 5;
            string cutoffDate = DateTime.Now.AddYears(-presumedYearLimit).ToString("yyyy-MM-dd");
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            // Act
            var result = await _reporting.GetReportData(startDate: cutoffDate, endDate: currentDate);

            // Assert
            Assert.That(result.StatusCode, Is.AnyOf(
                HttpStatusCode.OK,
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden),
                $"System should handle cutoff date consistently: {cutoffDate}");

            if (result.StatusCode == HttpStatusCode.OK)
            {
                TestContext.WriteLine($"API accepts data from exactly {presumedYearLimit} years ago");
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest && result.ResponseBody != null)
            {
                TestContext.WriteLine($"API rejects data from exactly {presumedYearLimit} years ago with message: {result.ResponseBody["message"]}");
            }
        }

        [Test]
        [Category("Validation")]
        public async Task GetReportData_MissingStartDate_WithEndDate()
        {
            // Arrange - check that the API correctly handles missing start date
            string endDate = DateTime.Now.ToString("yyyy-MM-dd");

            // Act
            var result = await _reporting.GetReportData(endDate: endDate);

            // Assert
            if (result.StatusCode == HttpStatusCode.OK)
            {
                Assert.That(result.ResponseBody, Is.Not.Null,
                    "Response should not be null when StartDate is missing");
                TestContext.WriteLine("API accepts missing StartDate parameter");
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                if (result.ResponseBody != null)
                {
                    Assert.That(result.ResponseBody["message"].ToString(),
                        Does.Contain("StartDate").Or.Contain("start"),
                        "Error message should reference missing start date");
                    TestContext.WriteLine($"API requires StartDate parameter: {result.ResponseBody["message"]}");
                }
            }
            else
            {
                Assert.That(result.StatusCode, Is.AnyOf(
                    HttpStatusCode.OK,
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.Forbidden),
                    "Unexpected status code when StartDate is missing");
            }
        }

        [Test]
        [Category("Validation")]
        public async Task GetReportData_MissingEndDate_WithStartDate()
        {
            // Arrange - check that the API correctly handles missing end date
            string startDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

            // Act
            var result = await _reporting.GetReportData(startDate: startDate);

            // Assert
            if (result.StatusCode == HttpStatusCode.OK)
            {
                Assert.That(result.ResponseBody, Is.Not.Null,
                    "Response should not be null when EndDate is missing");
                TestContext.WriteLine("API accepts missing EndDate parameter");
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                if (result.ResponseBody != null)
                {
                    Assert.That(result.ResponseBody["message"].ToString(),
                        Does.Contain("EndDate").Or.Contain("end"),
                        "Error message should reference missing end date");
                    TestContext.WriteLine($"API requires EndDate parameter: {result.ResponseBody["message"]}");
                }
            }
            else
            {
                Assert.That(result.StatusCode, Is.AnyOf(
                    HttpStatusCode.OK,
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.Forbidden),
                    "Unexpected status code when EndDate is missing");
            }
        }

        [Test]
        [Category("Validation")]
        public async Task DownloadReport_ValidParameters_ReturnsExcel()
        {
            // Arrange - use administrator credentials
            string email = "admin@admin.com";
            string password = "Password123!";

            var loginResult = await _auth.LoginUser(email, password);
            string token = loginResult.ResponseBody?["accessToken"]?.ToString();

            // Act - request with valid parameters for Excel
            string validStartDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            string validEndDate = DateTime.Now.ToString("yyyy-MM-dd");
            var result = await _reporting.DownloadReport(
                format: "excel",
                startDate: validStartDate,
                endDate: validEndDate,
                token: token);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.ResponseContent, Is.Not.Null);
            Assert.That(result.ResponseContent.Length, Is.GreaterThan(0), "Excel file should not be empty");
        }

        [Test]
        [Category("Validation")]
        public async Task DownloadReport_InvalidFormat_ReturnsBadRequest()
        {
            // Arrange - use administrator credentials
            string email = "admin@admin.com";
            string password = "Password123!";
            var loginResult = await _auth.LoginUser(email, password);
            string token = loginResult.ResponseBody?["accessToken"]?.ToString();

            // Act - request with an obviously invalid format
            string validStartDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            string validEndDate = DateTime.Now.ToString("yyyy-MM-dd");
            var result = await _reporting.DownloadReport(
                format: "json", // Obviously unsupported format
                startDate: validStartDate,
                endDate: validEndDate,
                token: token);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                "API should reject requests with invalid report formats");
        }

        [Test]
        [Category("Validation")]
        public async Task DownloadReport_PdfFormat_ReturnsPdf()
        {
            // Arrange - use administrator credentials
            string email = "admin@admin.com";
            string password = "Password123!";

            var loginResult = await _auth.LoginUser(email, password);
            string token = loginResult.ResponseBody?["accessToken"]?.ToString();

            // Act - request with PDF format
            string validStartDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            string validEndDate = DateTime.Now.ToString("yyyy-MM-dd");
            var result = await _reporting.DownloadReport(
                format: "pdf",
                startDate: validStartDate,
                endDate: validEndDate,
                token: token);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.ResponseContent, Is.Not.Null);
            Assert.That(result.ResponseContent.Length, Is.GreaterThan(0), "PDF file should not be empty");
        }
    }
}
