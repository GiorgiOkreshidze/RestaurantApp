using Newtonsoft.Json.Linq;
using System.Net;
using System;
using System.Collections.Generic;
using ApiTests.Utilities;
using automation_qa.Framework;

namespace ApiTests.Pages
{
    /// <summary>
    /// The LocationsWithCurl class provides methods for interacting with the locations-related endpoints of the API using curl.
    /// It includes methods to retrieve locations, location options, speciality dishes by location, and customer feedback.
    /// Each method sends a synchronous GET request using curl, logs the response, parses the response body, and returns the status code along with the parsed data.
    /// </summary>
    public class LocationsWithCurl : BasePage
    {
        private CurlHelper _curlHelper;
        private string _baseUrl;

        public LocationsWithCurl()
        {
            _curlHelper = new CurlHelper();
            _baseUrl = BaseConfiguration.ApiBaseUrl;
        }

        public (HttpStatusCode StatusCode, JArray ResponseBody) GetLocationsWithCurl()
        {
            string url = $"{_baseUrl}/locations";

            var (statusCode, responseBodyArray) = _curlHelper.ExecuteGetRequestForArray(url);

            Console.WriteLine($"GetLocations response status: {statusCode}");

            if (responseBodyArray != null)
            {
                string responseContent = responseBodyArray.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"GetLocations response content: {preview}");
            }

            return (statusCode, responseBodyArray);
        }

        public (HttpStatusCode StatusCode, JObject ResponseBody) GetLocationByIdWithCurl(string locationId)
        {
            // Check if locationId is in a valid format
            if (string.IsNullOrEmpty(locationId) || !Guid.TryParse(locationId, out _))
            {
                throw new ArgumentException("Location ID is not in a valid format.", nameof(locationId));
            }

            string url = $"{_baseUrl}/locations/{locationId}";

            var (statusCode, responseBodyObject) = _curlHelper.ExecuteGetRequestForObject(url);

            Console.WriteLine($"GetLocationById response status: {statusCode}");

            if (responseBodyObject != null)
            {
                string responseContent = responseBodyObject.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"GetLocationById response content: {preview}");
            }

            return (statusCode, responseBodyObject);
        }

        public (HttpStatusCode StatusCode, JArray ResponseBody) GetLocationSelectOptionsWithCurl()
        {
            string url = $"{_baseUrl}/locations/select-options";

            var (statusCode, responseBodyArray) = _curlHelper.ExecuteGetRequestForArray(url);

            Console.WriteLine($"GetLocationSelectOptions response status: {statusCode}");

            if (responseBodyArray != null)
            {
                string responseContent = responseBodyArray.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"GetLocationSelectOptions response content: {preview}");
            }

            return (statusCode, responseBodyArray);
        }

        public (HttpStatusCode StatusCode, JArray ResponseBody) GetSpecialityDishesWithCurl(string locationId)
        {
            string url = $"{_baseUrl}/locations/{locationId}/speciality-dishes";

            var (statusCode, responseBodyArray) = _curlHelper.ExecuteGetRequestForArray(url);

            Console.WriteLine($"GetSpecialityDishes response status: {statusCode}");

            if (responseBodyArray != null)
            {
                string responseContent = responseBodyArray.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"GetSpecialityDishes response content: {preview}");
            }

            return (statusCode, responseBodyArray);
        }

        public (HttpStatusCode StatusCode, JObject ResponseBody) GetLocationFeedbacksWithCurl(
            string locationId,
            string type = null,
            string sortBy = "date",
            string sortDir = "DESC",
            int page = 1,
            int size = 10)
        {
            string url = $"{_baseUrl}/locations/{locationId}/feedbacks";
            string queryParams = "";

            List<string> parameters = new List<string>();

            if (!string.IsNullOrEmpty(type))
            {
                parameters.Add($"type={type}");
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                string sort = $"{sortBy},{sortDir}";
                parameters.Add($"sort={sort}");
            }

            parameters.Add($"page={page}");
            parameters.Add($"size={size}");

            queryParams = string.Join("&", parameters);

            var (statusCodeStr, responseBody) = _curlHelper.ExecuteGetRequestWithParams(url, queryParams);

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            if (int.TryParse(statusCodeStr, out int statusCodeInt))
            {
                if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeInt))
                {
                    statusCode = (HttpStatusCode)statusCodeInt;
                }
            }

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

            Console.WriteLine($"GetLocationFeedbacks response status: {statusCode}");

            if (responseBodyObject != null)
            {
                string responseContent = responseBodyObject.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"GetLocationFeedbacks response content: {preview}");
            }

            return (statusCode, responseBodyObject);
        }
    }
}
