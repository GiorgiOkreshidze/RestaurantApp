using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Runtime;
using Function.Models;
using Function.Models.User;
using Function.Services;
using Function.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Function.Actions.Reservations
{
    public class CompleteReservationAction
    {
        private readonly IReservationService _reservationService;

        public CompleteReservationAction()
        {
            _reservationService = new ReservationService();
        }

        public async Task<APIGatewayProxyResponse> CompleteReservation(APIGatewayProxyRequest request)
        {
            var token = ActionUtils.ExtractJwtToken(request);
            var role = token.Claims.FirstOrDefault(c => c.Type == "custom:role")!.Value.ToRoles();

            if (role != Roles.Waiter)
            {
                throw new UnauthorizedException("You don't have permission to access this resource.");
            }

            string reservationId;

            try
            {
                reservationId = request.PathParameters["id"];
            }
            catch (Exception)
            {
                throw new ArgumentException("Reservation id is required.");
            }

            await _reservationService.CompleteReservationAsync(reservationId);

            return ActionUtils.FormatResponse(200, new { message = "Reservation was completed successfully" });
        }
    }
}
