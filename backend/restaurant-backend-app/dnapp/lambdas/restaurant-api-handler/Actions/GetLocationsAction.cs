using Function.Services.Interfaces;
using Function.Services;
using SimpleLambdaFunction.Services.Interfaces;
using SimpleLambdaFunction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Actions;

namespace Function.Actions
{
    public class GetLocationsAction
    {
        private readonly IDynamoDBService _dynamoDBService;
        public GetLocationsAction()
        {
            _dynamoDBService = new DynamoDBService();
        }

        public async Task<APIGatewayProxyResponse> GetLocations(APIGatewayProxyRequest request)
        {
            try
            {
                var locations = await _dynamoDBService.GetListOfLocations();
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
