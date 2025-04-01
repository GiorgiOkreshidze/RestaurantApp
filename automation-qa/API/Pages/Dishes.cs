using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using System;

namespace ApiTests.Pages
{
    /// <summary>
    /// This class provides functionality to interact with the API to retrieve popular dishes.
    /// It sends a GET request to the "/dishes/popular" endpoint and processes the response.
    /// </summary>
    public class Dishes : BasePage
    {
        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetPopularDishes()
        {
            var request = CreateGetRequest("/dishes/popular");
            var response = await ExecuteGetRequestAsync(request);
            JArray? responseBody = null;

            Console.WriteLine($"GetPopularDishes response status: {response.StatusCode}");
            Console.WriteLine($"GetPopularDishes response content: {response.Content}");

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
    }
}
