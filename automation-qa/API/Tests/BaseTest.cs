using System;
using System.Net;
using System.Threading.Tasks;
using ApiTests.Pages;
using ApiTests.Utilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using automation_qa.Framework;

namespace ApiTests
{
    public class BaseTest
    {
        protected RestClient _client;

        // Shared authentication tokens for all tests
        protected static string IdToken;
        protected static string RefreshToken;
        protected static string AccessToken;

        // Shared API pages
        protected static Authentication AuthPage;

        // Test configuration
        protected static TestConfig Config = TestConfig.Instance;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            AuthPage = new Authentication();

            // Register and login a test user once for all tests
            await RegisterAndLoginTestUser();
        }

        [SetUp]
        public void Setup()
        {
            _client = new RestClient(BaseConfiguration.ApiBaseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
        }

        private async Task RegisterAndLoginTestUser()
        {
            // First, try to login
            var (loginStatus, loginResponse) = await AuthPage.LoginUser(
                Config.TestUserEmail,
                Config.TestUserPassword);

            Console.WriteLine($"Login attempt status: {loginStatus}");

            // If login fails, try to register a new user and then login
            if (loginStatus != HttpStatusCode.OK || loginResponse == null || !loginResponse.ContainsKey("idToken"))
            {
                Console.WriteLine("Login failed. Attempting to register a new test user.");

                var (registerStatus, email, registerResponse) = await AuthPage.RegisterUser(
                    "Test",
                    "User",
                    Config.TestUserEmail,
                    Config.TestUserPassword);

                Console.WriteLine($"Registration status: {registerStatus}");

                if (registerStatus == HttpStatusCode.OK || registerStatus == HttpStatusCode.Created)
                {
                    // After registration, login again
                    (loginStatus, loginResponse) = await AuthPage.LoginUser(
                        Config.TestUserEmail,
                        Config.TestUserPassword);

                    Console.WriteLine($"Login after registration status: {loginStatus}");
                }
                else
                {
                    Console.WriteLine($"Registration failed: {registerStatus}");
                    if (registerResponse != null)
                    {
                        Console.WriteLine($"Registration response: {registerResponse}");
                    }
                }
            }

            // Store tokens if login was successful
            if (loginStatus == HttpStatusCode.OK && loginResponse != null)
            {
                if (loginResponse.ContainsKey("idToken"))
                {
                    IdToken = loginResponse["idToken"].ToString();
                    Console.WriteLine("ID token retrieved successfully");
                }

                if (loginResponse.ContainsKey("refreshToken"))
                {
                    RefreshToken = loginResponse["refreshToken"].ToString();
                    Console.WriteLine("Refresh token retrieved successfully");
                }

                if (loginResponse.ContainsKey("accessToken"))
                {
                    AccessToken = loginResponse["accessToken"].ToString();
                    Console.WriteLine("Access token retrieved successfully");
                }

                if (string.IsNullOrEmpty(IdToken) && !string.IsNullOrEmpty(AccessToken))
                {
                    // If idToken is missing but accessToken exists, use accessToken as idToken
                    IdToken = AccessToken;
                    Console.WriteLine("Using access token as ID token");
                }
            }
            else
            {
                Console.WriteLine("Warning: Failed to obtain authentication tokens. API tests requiring authentication may fail.");
                if (loginResponse != null)
                {
                    Console.WriteLine($"Login response: {loginResponse}");
                }
            }
        }
    }
}