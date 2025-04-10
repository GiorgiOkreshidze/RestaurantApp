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

        /// <summary>
        /// Retrieves the specialty dishes for a specific restaurant location
        /// </summary>
        /// <param name="locationId">ID of the restaurant location</param>
        /// <returns>Tuple containing the HTTP status code and response body as JArray</returns>
        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetSpecialtyDishes(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                Console.WriteLine("Error: locationId cannot be null or empty");
                return (HttpStatusCode.BadRequest, null);
            }

            var request = CreateGetRequest($"/locations/{locationId}/specialty-dishes");

            var response = await ExecuteGetRequestAsync(request);

            Console.WriteLine($"GetSpecialtyDishes response status: {response.StatusCode}");
            Console.WriteLine($"GetSpecialtyDishes response content: {response.Content}");

            JArray? responseBody = null;
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

        /// <summary>
        /// Получает список всех блюд с возможностью фильтрации по типу и сортировки
        /// </summary>
        /// <param name="dishType">Тип блюда для фильтрации (например, "Appetizers", "Desserts")</param>
        /// <param name="sort">Параметр сортировки (например, "PopularityAsc")</param>
        /// <returns>Tuple containing the HTTP status code and response body as JArray</returns>
        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetAllDishes(string dishType = null, string sort = null)
        {
            string endpoint = "/dishes";

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(dishType))
            {
                queryParams.Add($"dishType={dishType}");
            }

            if (!string.IsNullOrEmpty(sort))
            {
                queryParams.Add($"sort={sort}");
            }

            if (queryParams.Count > 0)
            {
                endpoint += "?" + string.Join("&", queryParams);
            }

            var request = CreateGetRequest(endpoint);
            var response = await ExecuteGetRequestAsync(request);

            Console.WriteLine($"GetAllDishes response status: {response.StatusCode}");
            Console.WriteLine($"GetAllDishes response content: {response.Content}");

            JArray? responseBody = null;
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

        /// <summary>
        /// Получает информацию о конкретном блюде по его ID
        /// </summary>
        /// <param name="dishId">Идентификатор блюда</param>
        /// <returns>Tuple containing the HTTP status code and response body as JObject</returns>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> GetDishById(string dishId)
        {
            if (string.IsNullOrEmpty(dishId))
            {
                Console.WriteLine("Error: dishId cannot be null or empty");
                return (HttpStatusCode.BadRequest, null);
            }

            var request = CreateGetRequest($"/dishes/{dishId}");
            var response = await ExecuteGetRequestAsync(request);

            Console.WriteLine($"GetDishById response status: {response.StatusCode}");
            Console.WriteLine($"GetDishById response content: {response.Content}");

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
