using Amazon.Lambda.APIGatewayEvents;
using Function.Models;
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
    class DeleteReservationAction
    {
        private readonly IDynamoDBService _dynamoDBService;
        public DeleteReservationAction()
        {
            _dynamoDBService = new DynamoDBService();
        }

        public async Task<APIGatewayProxyResponse> DeleteReservation(APIGatewayProxyRequest request)
        {
            try
            {
                var reservationId = request.PathParameters["id"];

                await _dynamoDBService.CancelReservation(reservationId);
                
                return ActionUtils.FormatResponse(200, "Reservation cancelled successfuly");

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
