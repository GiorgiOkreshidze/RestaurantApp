using Function.Services.Interfaces;
using Function.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Actions;

namespace Function.Actions
{
    public class GetLocationOptions
    {
        private readonly IDynamoDBService _dynamoDBService;
        public GetLocationOptions()
        {
            _dynamoDBService = new DynamoDBService();
        }

        public async Task<APIGatewayProxyResponse> GetOptions(APIGatewayProxyRequest request)
        {
            try
            {
                var locations = await _dynamoDBService.GetOptionsOfLocations();
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
