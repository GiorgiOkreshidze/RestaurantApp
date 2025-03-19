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
            var firstName = userInfo.GetValueOrDefault("given_name");
            var lastName = userInfo.GetValueOrDefault("family_name");
            var fullName = $"{firstName} {lastName}";

            if (reservationRequest == null)
            {
                throw new ArgumentException("Reservation request body was null");
            }

            if (int.TryParse(reservationRequest.GuestsNumber, out int guests) && guests > 10)
            {
                throw new ArgumentException("The maximum number of guests allowed is 10");
            }

            var location = await _dynamoDBService.GetLocationById(reservationRequest.LocationId);

            var existingReservations = await _dynamoDBService.GetReservationsByDateLocationTable(
                reservationRequest.Date,
                location.Address,
                reservationRequest.TableNumber);

            var newTimeFrom = TimeSpan.Parse(reservationRequest.TimeFrom);
            var newTimeTo = TimeSpan.Parse(reservationRequest.TimeTo);

            foreach (var existingReservation in existingReservations)
            {
                var existingTimeFrom = TimeSpan.Parse(existingReservation.TimeFrom);
                var existingTimeTo = TimeSpan.Parse(existingReservation.TimeTo);

                // Check if the time slots overlap
                if (newTimeFrom < existingTimeTo &&
                    newTimeTo > existingTimeFrom &&
                    existingReservation.UserInfo != fullName)
                {
                    throw new ArgumentException(
                        $"Table #{reservationRequest.TableNumber} at location " +
                        $"{location.Address} is already booked during the requested time period."
                    );
                }
            }

            var reservationId = Guid.NewGuid().ToString();
            if (reservationRequest.Id != null)
            {
                reservationId = reservationRequest.Id;
            }

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
                TimeFrom = reservationRequest.TimeFrom,
                TimeTo = reservationRequest.TimeTo,
                TimeSlot = reservationRequest.TimeFrom + " - " + reservationRequest.TimeTo,
                UserInfo = fullName,
            };

            var reservationResponse = await _dynamoDBService.UpsertReservation(reservation);

            return ActionUtils.FormatResponse(200, reservationResponse);
        }
        catch (ArgumentException e)
        {
            return ActionUtils.FormatResponse(400, new
            {
                message = e.Message
            });
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