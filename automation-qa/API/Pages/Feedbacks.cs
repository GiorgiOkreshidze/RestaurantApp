using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using ApiTests.Utilities;
using automation_qa.Framework;

namespace ApiTests.Pages
{
    public class Feedbacks : BasePage
    {
        private readonly CurlHelper _curlHelper;
        private readonly string _baseUrl;

        public Feedbacks(string baseUrl = null)
        {
            _baseUrl = baseUrl ?? BaseConfiguration.ApiBaseUrl;
            _curlHelper = new CurlHelper("curl");
        }

        /// <summary>
        /// Создает новый отзыв для бронирования
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) CreateFeedbackWithCurl(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating,
            string token = null)
        {
            string url = $"{_baseUrl}/feedbacks";

            var feedbackData = new
            {
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating
            };

            string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(feedbackData);

            Console.WriteLine($"Creating feedback with curl for reservation: {reservationId}");

            if (!string.IsNullOrEmpty(token))
            {
                return _curlHelper.ExecutePostRequestWithAuthForObject(url, jsonBody, token);
            }
            else
            {
                return _curlHelper.ExecutePostRequestForObject(url, jsonBody);
            }
        }

        /// <summary>
        /// Создает новый отзыв для бронирования
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> CreateFeedback(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating,
            string token = null)
        {
            return await Task.Run(() => CreateFeedbackWithCurl(
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating,
                token));
        }
    }
}

