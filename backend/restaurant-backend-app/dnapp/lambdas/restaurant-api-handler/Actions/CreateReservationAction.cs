using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Actions;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;

namespace Function.Services;

public class CreateReservationAction
{
    private readonly IDynamoDBService _dynamoDBService;
    private readonly IAuthenticationService _authenticationService;

    public CreateReservationAction()
    {
        _dynamoDBService = new DynamoDBService();
        _authenticationService = new AuthenticationService();
    }

    public async Task<APIGatewayProxyResponse> CreateReservation(APIGatewayProxyRequest request)
    {
        var accessToken = ActionUtils.GetAccessToken(request);

        try
        {
            var userInfo = await _authenticationService.GetUserDetailsAsync(accessToken);
            var reservationRequest = JsonSerializer.Deserialize<ReservationRequest>(request.Body);

            if (reservationRequest == null)
            {
                throw new ArgumentException("Reservation request body was null");
            }
            
            var reservationId = Guid.NewGuid().ToString();
            if (reservationRequest.Id != null)
            {
                reservationId = reservationRequest.Id;
            }

            var location = await _dynamoDBService.GetLocationById(reservationRequest.LocationId);

            var firstName = userInfo.GetValueOrDefault("given_name");
            var lastName = userInfo.GetValueOrDefault("family_name");

            var reservation = new Reservation
            {
                Id = reservationId,
                Date = reservationRequest.Date,
                FeedbackId = "NOT IMPLEMENTED YET AND ISN'T REQUIRED",
                GuestsNumber = reservationRequest.GuestsNumber,
                LocationAddress = location.Address,
                PreOrder = "NOT IMPLEMENTED YET",
                Status = Status.Reserved.ToString(),
                TableNumber = reservationRequest.TableNumber,
                TimeFrom = reservationRequest.TimeFrom ,
                TimeTo = reservationRequest.TimeTo,
                TimeSlot = reservationRequest.TimeFrom + " - " + reservationRequest.TimeTo,
                UserInfo = firstName + " " + lastName,
            };

            var reservationResponse = await _dynamoDBService.UpsertReservation(reservation);
            
            return ActionUtils.FormatResponse(200, reservationResponse);
        }
        catch (JsonException e)
        {
            return ActionUtils.FormatResponse(400, new
            {
                message = "The json was in invalid format"
            });
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