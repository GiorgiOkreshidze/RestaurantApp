using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ApiTests.Pages
{
    public class Authentication : BasePage
    {
        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="email">User's email address (auto-generated if not provided)</param>
        /// <param name="password">User's password (uses default strong password if not provided)</param>
        /// <returns>Tuple containing HTTP status code, email, and response body as JObject</returns>
        public async Task<(HttpStatusCode statusCode, string email, JObject responseBody)> RegisterUser(
            string firstName, string lastName, string email = null, string password = "StrongP@ss123!")
        {
            if (string.IsNullOrEmpty(email))
            {
                email = $"test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            }

            var request = CreatePostRequest("/signup");
            var userData = new
            {
                firstName,
                lastName,
                email,
                password
            };
            request.AddJsonBody(userData);
            var response = await ExecutePostRequestAsync(request);

            JObject responseBody = null;
            try
            {
                if (!string.IsNullOrEmpty(response.Content))
                {
                    responseBody = JObject.Parse(response.Content);
                }
            }
            catch (JsonReaderException)
            {
                responseBody = new JObject();
            }
            return (response.StatusCode, email, responseBody);
        }

        /// <summary>
        /// Logs in a user with the provided credentials
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="password">User's password</param>
        /// <returns>Tuple containing HTTP status code and response body as JObject</returns>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> LoginUser(
            string email, string password)
        {
            var request = CreatePostRequest("/signin");
            var loginData = new
            {
                email,
                password
            };
            request.AddJsonBody(loginData);
            var response = await ExecutePostRequestAsync(request);
            JObject responseBody = null;
            try
            {
                if (!string.IsNullOrEmpty(response.Content))
                {
                    responseBody = JObject.Parse(response.Content);

                    if (responseBody.ContainsKey("idToken") && !responseBody.ContainsKey("accessToken"))
                    {
                        responseBody["accessToken"] = responseBody["idToken"];
                    }
                }
                else
                {
                    responseBody = new JObject();
                }
            }
            catch (JsonReaderException)
            {
                responseBody = new JObject();
            }
            return (response.StatusCode, responseBody);
        }

        /// <summary>
        /// Logs out a user from the system
        /// </summary>
        /// <param name="refreshToken">User's refresh token</param>
        /// <returns>HTTP status code</returns>
        public async Task<(HttpStatusCode StatusCode, string ResponseBody)> LogoutUser(string refreshToken, string idToken = null)
        {
            var request = CreatePostRequest("/signout");

            var body = new
            {
                refreshToken = refreshToken
            };

            request.AddJsonBody(body);

            if (!string.IsNullOrEmpty(idToken))
            {
                request.AddHeader("Authorization", $"Bearer {idToken}");
            }

            var response = await ExecutePostRequestAsync(request);

            return (response.StatusCode, response.Content ?? string.Empty);
        }

        /// <summary>
        /// Updates the authentication token using the refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>A tuple containing the HTTP status code, new tokens as a JObject, and the raw response content</returns>

        public async Task<(HttpStatusCode statusCode, JObject responseBody, string rawResponse)> RefreshAuthToken(
            string refreshToken)
        {
            var request = CreatePostRequest("/auth/refresh");

            var refreshData = new
            {
                refreshToken
            };

            request.AddJsonBody(refreshData);

            var response = await ExecutePostRequestAsync(request);
            JObject responseBody = null;

            try
            {
                if (!string.IsNullOrEmpty(response.Content))
                {
                    responseBody = JObject.Parse(response.Content);
                }
                else
                {
                    responseBody = new JObject();
                }
            }
            catch (JsonReaderException)
            {
                responseBody = new JObject();
            }

            return (response.StatusCode, responseBody, response.Content);
        }

        /// <summary>
        /// Retrieves the user profile using authentication tokens
        /// </summary>
        /// <param name="idToken">User's ID token</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Tuple containing HTTP status code and user data as JObject</returns>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> GetUserProfile(
            string idToken, string accessToken = null)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new ArgumentNullException(nameof(idToken), "ID token must be provided.");
            }

            var request = CreateGetRequest("/users/profile");

            request.AddHeader("Authorization", $"Bearer {idToken}");

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("X-Amz-Security-Token", accessToken);
            }

            request.AddHeader("Date", DateTime.UtcNow.ToString("r"));

            Console.WriteLine($"Getting profile with Bearer idToken header: {idToken.Substring(0, Math.Min(20, idToken.Length))}...");
            var response = await ExecuteGetRequestAsync(request);

            JObject responseBody = null;
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

            Console.WriteLine($"Profile response status: {response.StatusCode}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Profile response content: {response.Content}");
            }

            return (response.StatusCode, responseBody);
        }
    }
}
