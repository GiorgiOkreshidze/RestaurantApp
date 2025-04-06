using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ApiTests.Pages
{
      /// <summary>
     /// The Locations class provides methods for interacting with the locations-related endpoints of the API.
     /// It includes methods to retrieve locations, location options, speciality dishes by location, and customer feedback.
     /// Each method sends an asynchronous GET request, logs the response, parses the response body, and returns the status code along with the parsed data.
     /// </summary>
    public class Locations : BasePage
    {
        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetLocations()
        {
            var request = CreateGetRequest("/locations");
            var response = await ExecuteGetRequestAsync(request);
            JArray? responseBody = null;
            Console.WriteLine($"Response status: {response.StatusCode}");
            Console.WriteLine($"Response content: {response.Content}");
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

        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetLocationSelectOptions()
        {
            var request = CreateGetRequest("/locations/select-options");
            var response = await ExecuteGetRequestAsync(request);
            JArray? responseBody = null;
            Console.WriteLine($"Response status: {response.StatusCode}");
            Console.WriteLine($"Response content: {response.Content}");
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

        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetSpecialityDishes(string locationId)
        {
            var request = CreateGetRequest($"/locations/{locationId}/speciality-dishes");
            var response = await ExecuteGetRequestAsync(request);
            JArray? responseBody = null;
            Console.WriteLine($"Response status: {response.StatusCode}");
            Console.WriteLine($"Response content: {response.Content}");
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

        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> GetLocationFeedbacks(
            string locationId,
            string? type = null,
            string? sortBy = "date",
            string? sortDir = "DESC",
            int page = 1,
            int size = 10)
        {
            var request = CreateGetRequest($"/locations/{locationId}/feedbacks");

            if (!string.IsNullOrEmpty(type))
            {
                request.AddQueryParameter("type", type);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                string sort = $"{sortBy},{sortDir}";
                request.AddQueryParameter("sort", sort);
            }

            request.AddQueryParameter("page", page.ToString());
            request.AddQueryParameter("size", size.ToString());

            var response = await ExecuteGetRequestAsync(request);
            JObject? responseBody = null;

            Console.WriteLine($"GetLocationFeedbacks response status: {response.StatusCode}");
            Console.WriteLine($"GetLocationFeedbacks response content: {response.Content}");

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
