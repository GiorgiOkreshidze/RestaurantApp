using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using System;

namespace ApiTests.Pages
{
    /// <summary>
    /// The Reservations class provides methods for interacting with reservation-related endpoints of the API.
    /// It includes methods to retrieve available tables for booking and user reservations.
    /// Each method sends an asynchronous GET request, logs the response, parses the response body, and returns the status code along with the parsed data.
    /// </summary>
    public class Reservations : BasePage
    {
        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetAvailableTables(
            string? locationId = null,
            string? date = null,
            int? guests = null,
            string? time = null)
        {
            var request = CreateGetRequest("/bookings/tables");

            if (!string.IsNullOrEmpty(locationId))
            {
                request.AddQueryParameter("locationId", locationId);
            }

            if (!string.IsNullOrEmpty(date))
            {
                request.AddQueryParameter("date", date);
            }

            if (guests.HasValue)
            {
                request.AddQueryParameter("guests", guests.Value.ToString());
            }

            if (!string.IsNullOrEmpty(time))
            {
                request.AddQueryParameter("time", time);
            }

            var response = await ExecuteGetRequestAsync(request);
            JArray? responseBody = null;

            Console.WriteLine($"GetAvailableTables response status: {response.StatusCode}");
            Console.WriteLine($"GetAvailableTables response content: {response.Content}");

            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    responseBody = JArray.Parse(response.Content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response: {ex.Message}");
                }
            }

            return (response.StatusCode, responseBody);
        }

        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetUserReservations()
        {
            var request = CreateGetRequest("/reservations");

            var response = await ExecuteGetRequestAsync(request);
            JArray? responseBody = null;

            Console.WriteLine($"GetUserReservations response status: {response.StatusCode}");
            Console.WriteLine($"GetUserReservations response content: {response.Content}");

            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    responseBody = JArray.Parse(response.Content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response: {ex.Message}");
                }
            }

            return (response.StatusCode, responseBody);
        }

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
            var request = CreatePostRequest("/reservations/client");

            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", $"Bearer {token}");
            }

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

            request.AddJsonBody(reservationData);

            var response = await ExecutePostRequestAsync(request);

            Console.WriteLine($"CreateReservation response status: {response.StatusCode}");
            Console.WriteLine($"CreateReservation response content: {response.Content}");

            JObject? responseBody = null;
            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    responseBody = JObject.Parse(response.Content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response: {ex.Message}");
                }
            }

            return (response.StatusCode, responseBody);
        }

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
            var request = CreatePostRequest("/reservations/waiter");

            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", $"Bearer {token}");
            }

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

            request.AddJsonBody(reservationData);

            var response = await ExecutePostRequestAsync(request);

            Console.WriteLine($"CreateReservationByWaiter response status: {response.StatusCode}");
            Console.WriteLine($"CreateReservationByWaiter response content: {response.Content}");

            JObject? responseBody = null;
            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    responseBody = JObject.Parse(response.Content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response: {ex.Message}");
                }
            }

            return (response.StatusCode, responseBody);
        }

        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> CancelReservation(
            string reservationId,
            string? token = null)
        {

            var request = CreateDeleteRequest($"/reservations/{reservationId}");

            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", $"Bearer {token}");
            }

            var response = await ExecuteDeleteRequestAsync(request);

            Console.WriteLine($"CancelReservation response status: {response.StatusCode}");
            Console.WriteLine($"CancelReservation response content: {response.Content}");

            JObject? responseBody = null;
            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    responseBody = JObject.Parse(response.Content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response: {ex.Message}");
                }
            }

            return (response.StatusCode, responseBody);
        }
    }
}
