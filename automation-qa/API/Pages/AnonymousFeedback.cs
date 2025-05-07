using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using ApiTests.Utilities;
using automation_qa.Framework;

namespace ApiTests.Pages
{
    public class AnonymousFeedback : BasePage
    {
        private readonly CurlHelper _curlHelper;
        private readonly string _baseUrl;

        public AnonymousFeedback(string baseUrl = null)
        {
            _baseUrl = baseUrl ?? BaseConfiguration.ApiBaseUrl;
            _curlHelper = new CurlHelper("curl");
        }

        /// <summary>
        /// Проверяет токен анонимного отзыва и возвращает информацию о бронировании, если токен действителен
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) ValidateTokenWithCurl(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Токен не может быть пустым");
            }

            string url = $"{_baseUrl}/anonymous-feedback/validate-token?token={token}";

            Console.WriteLine($"Executing GET request to validate token: {url}");

            var (statusCode, responseBody) = _curlHelper.ExecuteGetRequestForObject(url);

            Console.WriteLine($"ValidateTokenWithCurl response status: {statusCode}");
            Console.WriteLine($"ValidateTokenWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Проверяет токен анонимного отзыва и возвращает информацию о бронировании, если токен действителен
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> ValidateToken(string token)
        {
            return await Task.Run(() => ValidateTokenWithCurl(token));
        }

        /// <summary>
        /// Отправляет анонимный отзыв для завершенного бронирования
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) SubmitFeedbackWithCurl(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating)
        {
            if (string.IsNullOrEmpty(reservationId))
            {
                throw new ArgumentNullException(nameof(reservationId), "ID бронирования не может быть пустым");
            }

            string url = $"{_baseUrl}/anonymous-feedback/submit-feedback";

            var feedbackData = new
            {
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating
            };

            string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(feedbackData);

            Console.WriteLine($"Executing POST request to submit anonymous feedback: {url}");
            Console.WriteLine($"Request body: {jsonBody}");

            var (statusCode, responseBody) = _curlHelper.ExecutePostRequestForObject(url, jsonBody);

            Console.WriteLine($"SubmitFeedbackWithCurl response status: {statusCode}");
            Console.WriteLine($"SubmitFeedbackWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Отправляет анонимный отзыв для завершенного бронирования
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> SubmitFeedback(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating)
        {
            return await Task.Run(() => SubmitFeedbackWithCurl(
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating));
        }
    }
}