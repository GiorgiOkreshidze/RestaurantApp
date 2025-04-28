using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ApiTests.Utilities;
using automation_qa.Framework;

namespace ApiTests.Pages
{
    public class Dishes : BasePage
    {
        private readonly CurlHelper _curlHelper;
        private readonly string _baseUrl;

        public Dishes(string baseUrl = null)
        {
            _baseUrl = baseUrl ?? BaseConfiguration.ApiBaseUrl;
            _curlHelper = new CurlHelper("curl");
        }

        /// <summary>
        /// Gets popular dishes using curl
        /// </summary>
        public (HttpStatusCode statusCode, JArray responseBody) GetPopularDishesWithCurl()
        {
            string url = $"{_baseUrl}/dishes/popular";
            return _curlHelper.ExecuteGetRequestForArray(url);
        }

        /// <summary>
        /// Gets specialty dishes by location ID using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JArray ResponseBody) GetSpecialtyDishesWithCurl(string locationId)
        {
            string url = $"{_baseUrl}/dishes/specialty";
            if (!string.IsNullOrEmpty(locationId))
            {
                url += $"?locationId={locationId}";
            }
            return _curlHelper.ExecuteGetRequestForArray(url);
        }

        /// <summary>
        /// Gets all dishes with optional filtering parameters using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JArray ResponseBody) GetAllDishesWithCurl(
            string dishType = null, string sort = null)
        {
            string url = $"{_baseUrl}/dishes";
            string queryParams = "";

            if (!string.IsNullOrEmpty(dishType))
            {
                queryParams += $"dishType={dishType}";
            }

            if (!string.IsNullOrEmpty(sort))
            {
                if (!string.IsNullOrEmpty(queryParams))
                {
                    queryParams += "&";
                }
                queryParams += $"sort={sort}";
            }

            if (!string.IsNullOrEmpty(queryParams))
            {
                var (statusCodeStr, responseBody) = _curlHelper.ExecuteGetRequestWithParams(url, queryParams);
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                if (int.TryParse(statusCodeStr, out int statusCodeInt))
                {
                    if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeInt))
                    {
                        statusCode = (HttpStatusCode)statusCodeInt;
                    }
                }

                JArray responseBodyArray = null;
                if (!string.IsNullOrEmpty(responseBody))
                {
                    try
                    {
                        responseBodyArray = JArray.Parse(responseBody);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing response as array: {ex.Message}");
                    }
                }

                return (statusCode, responseBodyArray);
            }
            else
            {
                return _curlHelper.ExecuteGetRequestForArray(url);
            }
        }

        /// <summary>
        /// Gets a dish by its ID using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) GetDishByIdWithCurl(string dishId)
        {
            if (string.IsNullOrEmpty(dishId))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            string url = $"{_baseUrl}/dishes/{dishId}";
            return _curlHelper.ExecuteGetRequestForObject(url);
        }

        // Async Task wrapper methods to maintain compatibility with tests

        /// <summary>
        /// Gets popular dishes
        /// </summary>
        public async Task<(HttpStatusCode statusCode, JArray responseBody)> GetPopularDishes()
        {
            // Use curl version instead of RestSharp
            var (statusCode, responseBody) = GetPopularDishesWithCurl();
            return (statusCode, responseBody);
        }

        /// <summary>
        /// Gets specialty dishes by location ID
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JArray ResponseBody)> GetSpecialtyDishes(string locationId)
        {
            // Use curl version instead of RestSharp
            return GetSpecialtyDishesWithCurl(locationId);
        }

        /// <summary>
        /// Gets all dishes with optional filtering parameters
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JArray ResponseBody)> GetAllDishes(
            string dishType = null, string sort = null)
        {
            // Use curl version instead of RestSharp
            return GetAllDishesWithCurl(dishType, sort);
        }

        /// <summary>
        /// Gets a dish by its ID
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> GetDishById(string dishId)
        {
            // Use curl version instead of RestSharp
            return GetDishByIdWithCurl(dishId);
        }
    }
}
