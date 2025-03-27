using NUnit.Framework;
using RestSharp;
using automation_qa.Framework;

namespace ApiTests
{
    public class BaseTest
    {
        protected RestClient _client;

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
    }
}