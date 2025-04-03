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

            // Добавляем заголовок авторизации, если предоставлен токен
            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", $"Bearer {token}");
            }

            // Создаем объект с данными бронирования
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

            // Добавляем тело запроса в формате JSON
            request.AddJsonBody(reservationData);

            // Выполняем POST-запрос
            var response = await ExecutePostRequestAsync(request);

            // Логируем результат
            Console.WriteLine($"CreateReservation response status: {response.StatusCode}");
            Console.WriteLine($"CreateReservation response content: {response.Content}");

            // Парсим ответ
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
