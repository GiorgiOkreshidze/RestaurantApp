using Function.Services.Interfaces;
using Function.Services;
using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Actions;

namespace Function.Actions
{
    public class GetLocationOptionsAction
    {
        private readonly IDynamoDBService _dynamoDBService;

        public GetLocationOptionsAction()
        {
            _dynamoDBService = new DynamoDBService();
        }

        public async Task<APIGatewayProxyResponse> GetOptions(APIGatewayProxyRequest request)
        {
            try
            {
                var locations = await _dynamoDBService.GetLocationDropdownOptions();
                return ActionUtils.FormatResponse(200, locations);
            }
            catch (Exception e)
            {
                return ActionUtils.FormatResponse(500, new
                {
                    message = e.Message
                });
            }
        }
    }
}
