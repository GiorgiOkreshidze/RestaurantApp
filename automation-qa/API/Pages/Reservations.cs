using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ApiTests.Utilities;
using System.Collections.Specialized;
using System.Web;
using automation_qa.Framework;

namespace ApiTests.Pages
{
    public class Reservations : BasePage
    {
        private readonly CurlHelper _curlHelper;
        private readonly string _baseUrl;

        public Reservations(string baseUrl = null)
        {
            _baseUrl = baseUrl ?? BaseConfiguration.ApiBaseUrl;
            _curlHelper = new CurlHelper("curl");
        }

        /// <summary>
        /// Retrieves available tables for booking using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JArray? ResponseBody) GetAvailableTablesWithCurl(
            string? locationId = null,
            string? date = null,
            int? guests = null,
            string? time = null)
        {
            var queryParams = new NameValueCollection();
            if (!string.IsNullOrEmpty(locationId)) queryParams.Add("locationId", locationId);
            if (!string.IsNullOrEmpty(date)) queryParams.Add("date", date);
            if (guests.HasValue) queryParams.Add("guests", guests.Value.ToString());
            if (!string.IsNullOrEmpty(time)) queryParams.Add("time", time);

            string queryString = BuildQueryString(queryParams);
            string url = $"{_baseUrl}/reservations/tables{(string.IsNullOrEmpty(queryString) ? "" : $"?{queryString}")}";

            var (statusCode, responseBody) = _curlHelper.ExecuteGetRequestForArray(url);

            Console.WriteLine($"GetAvailableTablesWithCurl response status: {statusCode}");
            Console.WriteLine($"GetAvailableTablesWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Retrieves available tables for booking
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetAvailableTables(
            string? locationId = null,
            string? date = null,
            int? guests = null,
            string? time = null)
        {
            return await Task.Run(() => GetAvailableTablesWithCurl(locationId, date, guests, time));
        }

        /// <summary>
        /// Retrieves user reservations using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JArray? ResponseBody) GetUserReservationsWithCurl()
        {
            string url = $"{_baseUrl}/reservations";

            var (statusCode, responseBody) = _curlHelper.ExecuteGetRequestForArray(url);

            Console.WriteLine($"GetUserReservationsWithCurl response status: {statusCode}");
            Console.WriteLine($"GetUserReservationsWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Retrieves user reservations
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetUserReservations()
        {
            return await Task.Run(() => GetUserReservationsWithCurl());
        }

        /// <summary>
        /// Creates a reservation using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject? ResponseBody) CreateReservationWithCurl(
            string? token = null,
            string? locationId = null,
            string? tableId = null,
            string? date = null,
            string? startTime = null,
            string? endTime = null,
            int guests = 0,
            string? name = null,
            string? email = null,
            string? phone = null,
            string? specialRequests = null)
        {
            string url = $"{_baseUrl}/reservations/client";
            var reservationData = new
            {
                locationId,
                tableId,
                date,
                startTime,
                endTime,
                guests,
                name,
                email,
                phone,
                specialRequests
            };

            string jsonBody = JsonConvert.SerializeObject(reservationData);

            HttpStatusCode statusCode;
            JObject? responseBody;

            if (token == null)
            {
                (statusCode, responseBody) = _curlHelper.ExecutePostRequestForObject(url, jsonBody);
            }
            else
            {
                var (authStatusCode, authResponseBody) = _curlHelper.ExecutePostRequestWithAuthForString(url, jsonBody, token);
                statusCode = authStatusCode;
                try
                {
                    responseBody = string.IsNullOrEmpty(authResponseBody) ? null : JObject.Parse(authResponseBody);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing response body: {ex.Message}");
                    responseBody = null;
                }
            }

            Console.WriteLine($"CreateReservationWithCurl response status: {statusCode}");
            Console.WriteLine($"CreateReservationWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Creates a reservation
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> CreateReservation(
            string? token = null,
            string? locationId = null,
            string? tableId = null,
            string? date = null,
            string? startTime = null,
            string? endTime = null,
            int guests = 0,
            string? name = null,
            string? email = null,
            string? phone = null,
            string? specialRequests = null)
        {
            return await Task.Run(() => CreateReservationWithCurl(token, locationId, tableId, date, startTime, endTime, guests, name, email, phone, specialRequests));
        }

        /// <summary>
        /// Creates a reservation by waiter using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject? ResponseBody) CreateReservationByWaiterWithCurl(
            string? locationId = null,
            string? tableId = null,
            string? date = null,
            string? timeFrom = null,
            string? timeTo = null,
            int guestNumber = 0,
            string? status = null,
            string? userEmail = null,
            string? userInfo = null,
            DateTime? createdAt = null,
            string? waiterId = null,
            string? preorder = null,
            string? clientType = null,
            string? token = null)
        {
            string url = $"{_baseUrl}/reservations/waiter";
            var reservationData = new
            {
                locationId,
                tableId,
                date,
                timeFrom,
                timeTo,
                guestNumber,
                status,
                userEmail,
                userInfo,
                createdAt = createdAt?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                waiterId,
                preorder,
                clientType
            };

            string jsonBody = JsonConvert.SerializeObject(reservationData);

            HttpStatusCode statusCode;
            JObject? responseBody;

            if (token == null)
            {
                (statusCode, responseBody) = _curlHelper.ExecutePostRequestForObject(url, jsonBody);
            }
            else
            {
                var (authStatusCode, authResponseBody) = _curlHelper.ExecutePostRequestWithAuthForString(url, jsonBody, token);
                statusCode = authStatusCode;
                try
                {
                    responseBody = string.IsNullOrEmpty(authResponseBody) ? null : JObject.Parse(authResponseBody);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing response body: {ex.Message}");
                    responseBody = null;
                }
            }

            Console.WriteLine($"CreateReservationByWaiterWithCurl response status: {statusCode}");
            Console.WriteLine($"CreateReservationByWaiterWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Creates a reservation by waiter
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> CreateReservationByWaiter(
            string? locationId = null,
            string? tableId = null,
            string? date = null,
            string? timeFrom = null,
            string? timeTo = null,
            int guestNumber = 0,
            string? status = null,
            string? userEmail = null,
            string? userInfo = null,
            DateTime? createdAt = null,
            string? waiterId = null,
            string? preorder = null,
            string? clientType = null,
            string? token = null)
        {
            return await Task.Run(() => CreateReservationByWaiterWithCurl(locationId, tableId, date, timeFrom, timeTo, guestNumber, status, userEmail, userInfo, createdAt, waiterId, preorder, clientType, token));
        }

        /// <summary>
        /// Cancels a reservation using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject? ResponseBody) CancelReservationWithCurl(
            string reservationId,
            string? token = null)
        {
            string url = $"{_baseUrl}/reservations/{reservationId}";

            string tempHeadersFile = null;
            if (!string.IsNullOrEmpty(token))
            {
                tempHeadersFile = Path.GetTempFileName();
                using (StreamWriter writer = new StreamWriter(tempHeadersFile))
                {
                    writer.WriteLine($"Authorization: Bearer {token}");
                }
            }

            try
            {
                var process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                var arguments = $"/c {_curlHelper.GetCurlPath()} -k -s -o - -w \"%{{http_code}}\" -X DELETE";
                if (!string.IsNullOrEmpty(token))
                {
                    arguments += $" -H @{tempHeadersFile}";
                }
                arguments += $" \"{url}\"";
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                if (output.Length >= 3)
                {
                    string statusCodeStr = output.Substring(output.Length - 3);
                    string responseBodyStr = output.Substring(0, output.Length - 3).Trim();

                    HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                    if (int.TryParse(statusCodeStr, out int statusCodeInt))
                    {
                        if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeInt))
                        {
                            statusCode = (HttpStatusCode)statusCodeInt;
                        }
                    }

                    JObject? responseBody = null;
                    if (!string.IsNullOrEmpty(responseBodyStr))
                    {
                        try
                        {
                            responseBody = JObject.Parse(responseBodyStr);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing response: {ex.Message}");
                        }
                    }

                    Console.WriteLine($"CancelReservationWithCurl response status: {statusCode}");
                    Console.WriteLine($"CancelReservationWithCurl response content: {responseBody?.ToString() ?? "No content"}");

                    return (statusCode, responseBody);
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output: {output}");
                    return (HttpStatusCode.InternalServerError, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing curl: {ex.Message}");
                return (HttpStatusCode.InternalServerError, JObject.FromObject(new { error = ex.Message }));
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempHeadersFile) && File.Exists(tempHeadersFile))
                {
                    File.Delete(tempHeadersFile);
                }
            }
        }

        /// <summary>
        /// Cancels a reservation
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> CancelReservation(
            string reservationId,
            string? token = null)
        {
            return await Task.Run(() => CancelReservationWithCurl(reservationId, token));
        }

        private string BuildQueryString(NameValueCollection queryParams)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (string key in queryParams)
            {
                query[key] = queryParams[key];
            }
            return query.ToString();
        }

        /// <summary>
        /// Completes a reservation using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject? ResponseBody) CompleteReservationWithCurl(
            string reservationId,
            string? token = null)
        {
            if (string.IsNullOrEmpty(reservationId))
            {
                throw new ArgumentNullException(nameof(reservationId), "Reservation ID must be provided");
            }

            string url = $"{_baseUrl}/reservations/{reservationId}/complete";

            HttpStatusCode statusCode;
            JObject? responseBody;

            if (token == null)
            {
                // Execute request without authorization
                var (statusCodeStr, responseBodyStr) = _curlHelper.ExecutePostRequest(url, "");

                statusCode = HttpStatusCode.InternalServerError;
                if (int.TryParse(statusCodeStr, out int statusCodeInt))
                {
                    if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeInt))
                    {
                        statusCode = (HttpStatusCode)statusCodeInt;
                    }
                }

                try
                {
                    responseBody = string.IsNullOrEmpty(responseBodyStr) ? null : JObject.Parse(responseBodyStr);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing response body: {ex.Message}");
                    responseBody = null;
                }
            }
            else
            {
                // Execute request with authorization
                var (authStatusCode, authResponseBody) = _curlHelper.ExecutePostRequestWithAuthForString(url, "", token);
                statusCode = authStatusCode;

                try
                {
                    responseBody = string.IsNullOrEmpty(authResponseBody) ? null : JObject.Parse(authResponseBody);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing response body: {ex.Message}");
                    responseBody = null;
                }
            }

            Console.WriteLine($"CompleteReservationWithCurl response status: {statusCode}");
            Console.WriteLine($"CompleteReservationWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Completes a reservation
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> CompleteReservation(
            string reservationId,
            string? token = null)
        {
            return await Task.Run(() => CompleteReservationWithCurl(reservationId, token));
        }
    }
}
