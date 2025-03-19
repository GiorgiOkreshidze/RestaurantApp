using Amazon.Lambda.APIGatewayEvents;
using Function.Mappers;
using Function.Services;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Actions
{
    public class GetReservationsAction
    {
        private readonly IDynamoDBService _dynamoDBService;

        public GetReservationsAction()
        {
            _dynamoDBService = new DynamoDBService();
        }

        public async Task<APIGatewayProxyResponse> GetReservations(APIGatewayProxyRequest request)
        {
            try
            {
                var reservations = await _dynamoDBService.GetReservationsAsync();
                var response = Mapper.MapReservationsToReservationResponses(reservations);
                return ActionUtils.FormatResponse(200, response);
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
