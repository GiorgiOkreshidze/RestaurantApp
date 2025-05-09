using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ApiTests.Utilities;
using automation_qa.Framework;

namespace ApiTests.Pages
{
    public class Cart : BasePage
    {
        private readonly CurlHelper _curlHelper;
        private readonly string _baseUrl;

        public Cart(string baseUrl = null)
        {
            _baseUrl = baseUrl ?? BaseConfiguration.ApiBaseUrl;
            _curlHelper = new CurlHelper("curl");
        }

        /// <summary>
        /// Retrieves the current user's cart using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) GetCartWithCurl(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token must be provided");
            }

            string url = $"{_baseUrl}/cart";

            // Execute GET request with authorization header
            var (statusCode, responseBody) = _curlHelper.ExecuteGetRequestWithAuthForObject(url, token);

            Console.WriteLine($"GetCartWithCurl response status: {statusCode}");

            // Output first 100 characters of response or full response if shorter
            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"GetCartWithCurl response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Retrieves the current user's cart
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> GetCart(string token)
        {
            return await Task.Run(() => GetCartWithCurl(token));
        }

        /// <summary>
        /// Adds item to cart using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) AddToCartWithCurl(
            string token,
            string dishId,
            int quantity)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token must be provided");
            }

            if (string.IsNullOrEmpty(dishId))
            {
                throw new ArgumentNullException(nameof(dishId), "Dish ID must be provided");
            }

            string url = $"{_baseUrl}/cart/items";

            // Create JSON request body
            var cartItemData = new
            {
                dishId,
                quantity
            };

            string jsonBody = JsonConvert.SerializeObject(cartItemData);

            // Execute POST request with authorization header
            var (statusCode, responseBody) = _curlHelper.ExecutePostRequestWithAuthForObject(url, jsonBody, token);

            Console.WriteLine($"AddToCartWithCurl response status: {statusCode}");

            // Output first 100 characters of response or full response if shorter
            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"AddToCartWithCurl response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Adds item to cart
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> AddToCart(
            string token,
            string dishId,
            int quantity)
        {
            return await Task.Run(() => AddToCartWithCurl(token, dishId, quantity));
        }

        /// <summary>
        /// Updates cart item quantity using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) UpdateCartItemWithCurl(
            string token,
            string dishId,
            int quantity)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token must be provided");
            }

            if (string.IsNullOrEmpty(dishId))
            {
                throw new ArgumentNullException(nameof(dishId), "Dish ID must be provided");
            }

            string url = $"{_baseUrl}/cart/items/{dishId}";

            // Create JSON request body
            var updateData = new
            {
                quantity
            };

            string jsonBody = JsonConvert.SerializeObject(updateData);

            // Execute PUT request with authorization header
            var (statusCode, responseBody) = _curlHelper.ExecutePutRequestWithAuthForObject(url, jsonBody, token);

            Console.WriteLine($"UpdateCartItemWithCurl response status: {statusCode}");

            // Output first 100 characters of response or full response if shorter
            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"UpdateCartItemWithCurl response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Updates cart item quantity
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> UpdateCartItem(
            string token,
            string dishId,
            int quantity)
        {
            return await Task.Run(() => UpdateCartItemWithCurl(token, dishId, quantity));
        }

        /// <summary>
        /// Removes item from cart using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) RemoveFromCartWithCurl(
            string token,
            string dishId)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token must be provided");
            }

            if (string.IsNullOrEmpty(dishId))
            {
                throw new ArgumentNullException(nameof(dishId), "Dish ID must be provided");
            }

            string url = $"{_baseUrl}/cart/items/{dishId}";

            // Execute DELETE request
            var (statusCode, responseBody) = _curlHelper.ExecuteDeleteRequest(url, token);

            Console.WriteLine($"RemoveFromCartWithCurl response status: {statusCode}");

            // Output response content
            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"RemoveFromCartWithCurl response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Removes item from cart
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> RemoveFromCart(
            string token,
            string dishId)
        {
            return await Task.Run(() => RemoveFromCartWithCurl(token, dishId));
        }

        /// <summary>
        /// Clears the entire cart using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) ClearCartWithCurl(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token must be provided");
            }

            string url = $"{_baseUrl}/cart";

            // Execute DELETE request
            var (statusCode, responseBody) = _curlHelper.ExecuteDeleteRequest(url, token);

            Console.WriteLine($"ClearCartWithCurl response status: {statusCode}");

            // Output response content
            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"ClearCartWithCurl response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Clears the entire cart
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> ClearCart(string token)
        {
            return await Task.Run(() => ClearCartWithCurl(token));
        }

        /// <summary>
        /// Updates cart pre-order information using curl
        /// </summary>
        /// <summary>
        /// Updates cart pre-order information using curl
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) UpdateCartPreOrderWithCurl(
            string token,
            string id = null,
            string reservationId = null,
            string address = null,
            string status = null,
            string reservationDate = null,
            string timeSlot = null,
            string[] dishItems = null)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token must be provided");
            }
            string url = $"{_baseUrl}/cart";

            // Create JSON request body
            var preOrderData = new JObject();

            // Add optional fields if provided
            if (!string.IsNullOrEmpty(id))
                preOrderData["id"] = id;
            if (!string.IsNullOrEmpty(reservationId))
                preOrderData["reservationId"] = reservationId;
            if (!string.IsNullOrEmpty(address))
                preOrderData["address"] = address;
            if (!string.IsNullOrEmpty(status))
                preOrderData["status"] = status;
            if (!string.IsNullOrEmpty(reservationDate))
                preOrderData["reservationDate"] = reservationDate;
            if (!string.IsNullOrEmpty(timeSlot))
                preOrderData["timeSlot"] = timeSlot;

            // Add dish items if provided
            if (dishItems != null && dishItems.Length > 0)
            {
                var dishItemsArray = new JArray();
                foreach (var item in dishItems)
                {
                    dishItemsArray.Add(JObject.Parse(item));
                }
                preOrderData["dishItems"] = dishItemsArray;
            }

            string jsonBody = preOrderData.ToString();

            // Execute PUT request with authorization header
            var (statusCode, responseBody) = _curlHelper.ExecutePutRequestWithAuthForObject(url, jsonBody, token);
            Console.WriteLine($"UpdateCartPreOrderWithCurl response status: {statusCode}");

            // Output first 100 characters of response or full response if shorter
            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"UpdateCartPreOrderWithCurl response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Updates cart pre-order information
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> UpdateCartPreOrder(
            string token,
            string id = null,
            string reservationId = null,
            string address = null,
            string status = null,
            string reservationDate = null,
            string timeSlot = null,
            string[] dishItems = null)
        {
            return await Task.Run(() => UpdateCartPreOrderWithCurl(token, id, reservationId, address, status, reservationDate, timeSlot, dishItems));
        }
    }
}
