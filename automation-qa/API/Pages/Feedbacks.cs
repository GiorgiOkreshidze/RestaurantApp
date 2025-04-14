using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.Pages
{
    /// <summary>
    /// This class provides functionality to interact with the API to create and manage feedback.
    /// It sends requests to the "/feedbacks" endpoint and processes the responses.
    /// </summary>
    public class Feedback : BasePage
    {
        /// <summary>
        /// Creates a new feedback for a reservation
        /// </summary>
        /// <param name="reservationId">ID of the reservation</param>
        /// <param name="cuisineComment">Comment about the cuisine</param>
        /// <param name="cuisineRating">Rating for the cuisine (1-5)</param>
        /// <param name="serviceComment">Comment about the service</param>
        /// <param name="serviceRating">Rating for the service (1-5)</param>
        /// <param name="idToken">Authentication token for authorization</param>
        /// <returns>Tuple containing the HTTP status code and response body as JObject</returns>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> CreateFeedback(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating,
            string idToken = null)
        {
            // Create a POST request
            var request = CreatePostRequest("/feedbacks");

            // Add authorization header if token is provided
            if (!string.IsNullOrEmpty(idToken))
            {
                request.AddHeader("Authorization", $"Bearer {idToken}");
            }

            // Create request body
            var feedbackData = new
            {
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating
            };

            // Add request body as JSON
            request.AddJsonBody(feedbackData);

            // Execute the request
            var response = await ExecutePostRequestAsync(request);

            // Log the result
            Console.WriteLine($"CreateFeedback response status: {response.StatusCode}");
            Console.WriteLine($"CreateFeedback response content: {response.Content}");

            // Parse response into JObject if it's not empty
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

        /// <summary>
        /// Updates an existing feedback
        /// </summary>
        /// <param name="reservationId">ID of the reservation</param>
        /// <param name="cuisineComment">Updated comment about the cuisine</param>
        /// <param name="cuisineRating">Updated rating for the cuisine (1-5)</param>
        /// <param name="serviceComment">Updated comment about the service</param>
        /// <param name="serviceRating">Updated rating for the service (1-5)</param>
        /// <param name="idToken">Authentication token for authorization</param>
        /// <returns>Tuple containing the HTTP status code and response body as JObject</returns>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> UpdateFeedback(
            string reservationId,
            string cuisineComment,
            string cuisineRating,
            string serviceComment,
            string serviceRating,
            string idToken = null)
        {
            // Use the same endpoint and method as for creation
            // The API, based on the screenshot, uses the same endpoint for creation and update
            return await CreateFeedback(
                reservationId,
                cuisineComment,
                cuisineRating,
                serviceComment,
                serviceRating,
                idToken);
        }

        /// <summary>
        /// Gets all feedbacks for a reservation
        /// </summary>
        /// <param name="reservationId">ID of the reservation</param>
        /// <param name="idToken">Authentication token for authorization</param>
        /// <returns>Tuple containing the HTTP status code and response body as JArray</returns>
        public async Task<(HttpStatusCode StatusCode, JArray? ResponseBody)> GetFeedbacksByReservationId(
            string reservationId,
            string idToken = null)
        {
            // Check the input parameter
            if (string.IsNullOrEmpty(reservationId))
            {
                Console.WriteLine("Error: reservationId cannot be null or empty");
                return (HttpStatusCode.BadRequest, null);
            }

            // Create a GET request
            var request = CreateGetRequest($"/feedbacks?reservationId={reservationId}");

            // Add authorization header if token is provided
            if (!string.IsNullOrEmpty(idToken))
            {
                request.AddHeader("Authorization", $"Bearer {idToken}");
            }

            // Execute the request
            var response = await ExecuteGetRequestAsync(request);

            // Log the result
            Console.WriteLine($"GetFeedbacksByReservationId response status: {response.StatusCode}");
            Console.WriteLine($"GetFeedbacksByReservationId response content: {response.Content}");

            // Parse response into JArray if it's not empty
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
    }
}

