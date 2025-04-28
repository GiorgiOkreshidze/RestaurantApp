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

        public (HttpStatusCode StatusCode, JObject ResponseBody) CreateFeedbackWithCurl(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating,
            string idToken = null)
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

            if (!string.IsNullOrEmpty(idToken))
            {
                var (statusCode, responseBody) = _curlHelper.ExecutePostRequestWithAuthForString(url, jsonBody, idToken);

                JObject responseBodyObject = null;
                if (!string.IsNullOrEmpty(responseBody))
                {
                    try
                    {
                        responseBodyObject = JObject.Parse(responseBody);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing response as object: {ex.Message}");
                    }
                }

                return (statusCode, responseBodyObject);
            }
            else
            {
                return _curlHelper.ExecutePostRequestForObject(url, jsonBody);
            }
        }

        public (HttpStatusCode StatusCode, JObject ResponseBody) UpdateFeedbackWithCurl(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating,
            string idToken = null)
        {
            return CreateFeedbackWithCurl(
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating,
                idToken);
        }

        public (HttpStatusCode StatusCode, JArray ResponseBody) GetFeedbacksByReservationIdWithCurl(
            string reservationId,
            string idToken = null)
        {
            if (string.IsNullOrEmpty(reservationId))
            {
                Console.WriteLine("Error: reservationId cannot be null or empty");
                return (HttpStatusCode.BadRequest, null);
            }

            string url = $"{_baseUrl}/feedbacks?reservationId={reservationId}";
            Console.WriteLine($"Getting feedbacks with curl for reservation: {reservationId}");

            if (!string.IsNullOrEmpty(idToken))
            {
                var (statusCode, responseBodyObj) = _curlHelper.ExecuteGetRequestWithAuthForObject(url, idToken);

                JArray responseBodyArray = null;
                if (responseBodyObj != null)
                {
                    try
                    {
                        if (responseBodyObj["data"] != null && responseBodyObj["data"].Type == JTokenType.Array)
                        {
                            responseBodyArray = (JArray)responseBodyObj["data"];
                        }
                        else
                        {
                            responseBodyArray = new JArray { responseBodyObj };
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting response to array: {ex.Message}");
                    }
                }

                return (statusCode, responseBodyArray);
            }
            else
            {
                return _curlHelper.ExecuteGetRequestForArray(url);
            }
        }

        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> CreateFeedback(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating,
            string idToken = null)
        {
            return CreateFeedbackWithCurl(
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating,
                idToken);
        }

        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> UpdateFeedback(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating,
            string idToken = null)
        {
            return UpdateFeedbackWithCurl(
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating,
                idToken);
        }

        public async Task<(HttpStatusCode StatusCode, JArray ResponseBody)> GetFeedbacksByReservationId(
            string reservationId,
            string idToken = null)
        {
            return GetFeedbacksByReservationIdWithCurl(reservationId, idToken);
        }
    }
}

