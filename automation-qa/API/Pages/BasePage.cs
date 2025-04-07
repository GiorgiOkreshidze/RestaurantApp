using RestSharp;
using System.Net;
using System.Threading.Tasks;
using automation_qa.Framework;
using Newtonsoft.Json.Linq;

namespace ApiTests.Pages
{
     /// <summary>
     /// BasePage class provides common functionality for making HTTP requests using RestSharp.
     /// It supports creating and executing GET and POST requests, checking success status, 
     /// and parsing response bodies into JObject.
     /// </summary>
    public abstract class BasePage
    {
        protected RestClient _client;

        public BasePage()
        {
            _client = new RestClient(BaseConfiguration.ApiBaseUrl);
        }

        protected RestRequest CreatePostRequest(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            return request;
        }

        protected async Task<RestResponse> ExecutePostRequestAsync(RestRequest request)
        {
            return await _client.ExecuteAsync(request);
        }

        protected bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.OK;
        }

        protected RestRequest CreateGetRequest(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.Get);
            request.AddHeader("Accept", "application/json");
            return request;
        }

        protected async Task<RestResponse> ExecuteGetRequestAsync(RestRequest request)
        {
            return await _client.ExecuteAsync(request);
        }

        protected JObject ParseResponseBody(RestResponse response)
        {
            if (string.IsNullOrEmpty(response.Content))
                throw new InvalidOperationException("Response content is empty");
            return JObject.Parse(response.Content);
        }

        protected RestRequest CreateDeleteRequest(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.Delete);
            request.AddHeader("Accept", "application/json");
            return request;
        }

        protected async Task<RestResponse> ExecuteDeleteRequestAsync(RestRequest request)
        {
            return await _client.ExecuteAsync(request);
        }
    }
}
