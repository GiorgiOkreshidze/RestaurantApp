using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ApiTests.Utilities;
using automation_qa.Framework;

namespace ApiTests.Pages
{
    public class Authentication : BasePage
    {
        private readonly CurlHelper _curlHelper;
        private readonly string _baseUrl;

        public Authentication(string baseUrl = null)
        {
            _baseUrl = baseUrl ?? BaseConfiguration.ApiBaseUrl;
            _curlHelper = new CurlHelper("curl");
        }

        /// <summary>
        /// Registers a new user in the system using curl instead of RestSharp
        /// </summary>
        public (HttpStatusCode statusCode, string email, JObject responseBody) RegisterUserWithCurl(
            string firstName, string lastName, string email = null, string password = "StrongP@ss123!")
        {
            if (string.IsNullOrEmpty(email))
            {
                email = $"test_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
            }

            string url = $"{_baseUrl}/auth/signup";

            var userData = new
            {
                firstName,
                lastName,
                email,
                password
            };

            string jsonBody = JsonConvert.SerializeObject(userData);

            var (statusCode, responseBody) = _curlHelper.ExecutePostRequestForObject(url, jsonBody);

            return (statusCode, email, responseBody);
        }

        /// <summary>
        /// Uses curl to login a user with the provided credentials
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) LoginUserWithCurl(
            string email, string password)
        {
            string url = $"{_baseUrl}/auth/signin";

            // Создаем JSON для запроса
            var loginData = new
            {
                email,
                password
            };

            string jsonBody = JsonConvert.SerializeObject(loginData);

            var (statusCode, responseBody) = _curlHelper.ExecutePostRequestForObject(url, jsonBody);

            if (responseBody != null && responseBody.ContainsKey("idToken") && !responseBody.ContainsKey("accessToken"))
            {
                responseBody["accessToken"] = responseBody["idToken"];
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Uses curl to log out a user from the system
        /// </summary>
        public (HttpStatusCode StatusCode, string ResponseBody) LogoutUserWithCurl(
            string refreshToken, string idToken = null)
        {
            string url = $"{_baseUrl}/auth/signout";

            var logoutData = new
            {
                refreshToken
            };

            string jsonBody = JsonConvert.SerializeObject(logoutData);

            if (!string.IsNullOrEmpty(idToken))
            {
                return _curlHelper.ExecutePostRequestWithAuthForString(url, jsonBody, idToken);
            }
            else
            {
                var (statusCodeStr, responseBodyStr) = _curlHelper.ExecutePostRequest(url, jsonBody);

                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                if (int.TryParse(statusCodeStr, out int statusCodeInt))
                {
                    if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeInt))
                    {
                        statusCode = (HttpStatusCode)statusCodeInt;
                    }
                }

                return (statusCode, responseBodyStr);
            }
        }

        /// <summary>
        /// Uses curl to update the authentication token using the refresh token
        /// </summary>
        public (HttpStatusCode statusCode, JObject responseBody, string rawResponse) RefreshAuthTokenWithCurl(
            string refreshToken)
        {
            string url = $"{_baseUrl}/auth/refresh";

            var refreshData = new
            {
                refreshToken
            };

            string jsonBody = JsonConvert.SerializeObject(refreshData);

            var (statusCode, responseBody) = _curlHelper.ExecutePostRequestForObject(url, jsonBody);

            var (_, rawResponse) = _curlHelper.ExecutePostRequest(url, jsonBody);

            return (statusCode, responseBody, rawResponse);
        }

        /// <summary>
        /// Uses curl to retrieve the user profile using authentication tokens
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) GetUserProfileWithCurl(
            string idToken, string accessToken = null)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new ArgumentNullException(nameof(idToken), "ID token must be provided.");
            }

            string url = $"{_baseUrl}/users/profile";

            var (statusCode, responseBody) = _curlHelper.ExecuteGetRequestWithAuthForObject(url, idToken, accessToken);

            return (statusCode, responseBody);
        }

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
            var (statusCode, generatedEmail, responseBody) = RegisterUserWithCurl(
                firstName, lastName, email, password);

            return (statusCode, generatedEmail, responseBody);
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
            var (statusCode, responseBody) = LoginUserWithCurl(email, password);

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Logs out a user from the system
        /// </summary>
        /// <param name="refreshToken">User's refresh token</param>
        /// <returns>HTTP status code</returns>
        public async Task<(HttpStatusCode StatusCode, string ResponseBody)> LogoutUser(string refreshToken, string idToken = null)
        {
            return LogoutUserWithCurl(refreshToken, idToken);
        }

        /// <summary>
        /// Updates the authentication token using the refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>A tuple containing the HTTP status code, new tokens as a JObject, and the raw response content</returns>
        public async Task<(HttpStatusCode statusCode, JObject responseBody, string rawResponse)> RefreshAuthToken(
            string refreshToken)
        {
            return RefreshAuthTokenWithCurl(refreshToken);
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
            return GetUserProfileWithCurl(idToken, accessToken);
        }

        /// <summary>
        /// Uses curl to update user information by ID
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) UpdateUserWithCurl(
            string userId,
            string firstName,
            string lastName,
            string email,
            string idToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User ID must be provided.");
            }

            if (string.IsNullOrEmpty(idToken))
            {
                throw new ArgumentNullException(nameof(idToken), "ID token must be provided.");
            }

            string url = $"{_baseUrl}/users/{userId}";

            var userData = new
            {
                firstName,
                lastName,
                email
            };

            string jsonBody = JsonConvert.SerializeObject(userData);

            var (statusCode, responseBody) = _curlHelper.ExecutePutRequestWithAuthForObject(url, jsonBody, idToken);

            Console.WriteLine($"UpdateUser response status: {statusCode}");

            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"UpdateUser response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Updates user information by ID
        /// </summary>
        /// <param name="userId">ID of the user to update</param>
        /// <param name="firstName">User's new first name</param>
        /// <param name="lastName">User's new last name</param>
        /// <param name="email">User's new email address</param>
        /// <param name="idToken">Authentication token</param>
        /// <returns>Tuple containing HTTP status code and response body as JObject</returns>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> UpdateUser(
            string userId,
            string firstName,
            string lastName,
            string email,
            string idToken)
        {
            var (statusCode, responseBody) = UpdateUserWithCurl(
                userId, firstName, lastName, email, idToken);
            return (statusCode, responseBody);
        }

        /// <summary>
        /// Uses curl to update the profile info for the currently authenticated user
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) UpdateProfileWithCurl(
            string firstName,
            string lastName,
            string base64EncodedImage,
            string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Authentication token must be provided.");
            }

            string url = $"{_baseUrl}/users/profile";

            var profileData = new JObject();

            if (!string.IsNullOrEmpty(firstName))
                profileData["firstName"] = firstName;

            if (!string.IsNullOrEmpty(lastName))
                profileData["lastName"] = lastName;

            if (!string.IsNullOrEmpty(base64EncodedImage))
                profileData["base64EncodedImage"] = base64EncodedImage;

            string jsonBody = profileData.ToString();

            var (statusCode, responseBody) = _curlHelper.ExecutePutRequestWithAuthForObject(url, jsonBody, token);

            Console.WriteLine($"UpdateProfile response status: {statusCode}");

            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"UpdateProfile response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Updates the profile info for the currently authenticated user
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> UpdateProfile(
            string firstName,
            string lastName,
            string base64EncodedImage,
            string token)
        {
            return await Task.Run(() => UpdateProfileWithCurl(firstName, lastName, base64EncodedImage, token));
        }

        /// <summary>
        /// Uses curl to update the password for the currently authenticated user
        /// </summary>
        public (HttpStatusCode StatusCode, JObject ResponseBody) UpdatePasswordWithCurl(
            string oldPassword,
            string newPassword,
            string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Authentication token must be provided.");
            }

            if (string.IsNullOrEmpty(oldPassword))
            {
                throw new ArgumentNullException(nameof(oldPassword), "Old password must be provided.");
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException(nameof(newPassword), "New password must be provided.");
            }

            string url = $"{_baseUrl}/users/profile/password";

            var passwordData = new
            {
                oldPassword,
                newPassword
            };

            string jsonBody = JsonConvert.SerializeObject(passwordData);

            var (statusCode, responseBody) = _curlHelper.ExecutePutRequestWithAuthForObject(url, jsonBody, token);

            Console.WriteLine($"UpdatePassword response status: {statusCode}");

            if (responseBody != null)
            {
                string responseContent = responseBody.ToString();
                string preview = responseContent.Length > 100 ? responseContent.Substring(0, 100) + "..." : responseContent;
                Console.WriteLine($"UpdatePassword response content: {preview}");
            }

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Updates the password for the currently authenticated user
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject ResponseBody)> UpdatePassword(
            string oldPassword,
            string newPassword,
            string token)
        {
            return await Task.Run(() => UpdatePasswordWithCurl(oldPassword, newPassword, token));
        }
    }
}
