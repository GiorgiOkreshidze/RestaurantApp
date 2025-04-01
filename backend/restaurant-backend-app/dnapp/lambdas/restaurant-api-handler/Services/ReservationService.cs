using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Reservations;
using Function.Models.User;
using Function.Models.Requests;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Repository;
using Function.Actions;
using Amazon.CognitoIdentityProvider.Model;
using Function.Models.Interfaces;
using ResourceNotFoundException = Function.Exceptions.ResourceNotFoundException;
using Amazon.SQS.Model;
using Amazon.SQS;
using System.Text.Json;
using Function.Models.Responses;

namespace Function.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IWaiterRepository _waiterRepository;
    private readonly AmazonSQSClient _amazonSqsClient;

    private readonly string? _sqsQueueName = Environment.GetEnvironmentVariable("SQS_EVENTS_QUEUE_NAME");

    public ReservationService()
    {
        _reservationRepository = new ReservationRepository();
        _userRepository = new UserRepository();
        _locationRepository = new LocationRepository();
        _tableRepository = new TableRepository();
        _waiterRepository = new WaiterRepository();
        _amazonSqsClient = new AmazonSQSClient();
    }

    public async Task<Reservation> UpsertReservationAsync(IReservationRequest reservationRequest, string userId)
    {
        //Available UTC time slots:
        // 06:30 - 08:00
        // 08:15 - 09:45
        // 10:00 - 11:30
        // 11:45 - 13:15
        // 13:30 - 15:00
        // 15:15 - 16:45
        // 17:00 - 18:30
        var predefinedSlots = ActionUtils.GeneratePredefinedTimeSlots();
        var newTimeFrom = TimeSpan.Parse(reservationRequest.TimeFrom);
        var newTimeTo = TimeSpan.Parse(reservationRequest.TimeTo);
        var firstSlot = TimeSpan.Parse(predefinedSlots.First().Start);
        var lastSlot = TimeSpan.Parse(predefinedSlots.Last().End);

        if (newTimeFrom < firstSlot || newTimeTo > lastSlot)
        {
            throw new ArgumentException("Reservation must be within restaurant working hours.");
        }

        bool isValidSlot = predefinedSlots.Any(slot =>
            TimeSpan.Parse(slot.Start) == newTimeFrom &&
            TimeSpan.Parse(slot.End) == newTimeTo);

        if (!isValidSlot)
        {
            throw new ArgumentException("Reservation must exactly match one of the predefined time slots.");
        }
        
        var location = await _locationRepository.GetLocationByIdAsync(reservationRequest.LocationId);
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user is null)
        {
            throw new UnauthorizedException("User is not registered");
        }

        var existingReservations = await _reservationRepository.GetReservationsByDateLocationTable(
            reservationRequest.Date,
            location.Address,
            reservationRequest.TableId);

        foreach (var existingReservation in existingReservations)
        {
            var existingTimeFrom = TimeSpan.Parse(existingReservation.TimeFrom);
            var existingTimeTo = TimeSpan.Parse(existingReservation.TimeTo);
            
            // Check if the time slots overlap
            if (newTimeFrom <= existingTimeTo &&
                newTimeTo >= existingTimeFrom)
            {
                if (existingReservation.UserEmail == user.Email)
                {
                    throw new ArgumentException(
                   $"You already have reservation booked at location " +
                   $"{location.Address} during the requested time period.");

                }

                throw new ArgumentException(
                    $"Reservation #{reservationRequest.Id} at location " +
                    $"{location.Address} is already booked during the requested time period."
                );
            }
        }
        
        var reservationId = Guid.NewGuid().ToString();

        if (reservationRequest.Id != null)
        {
            reservationId = reservationRequest.Id;
        }

        var table = await _tableRepository.GetTableById(reservationRequest.TableId);

        if (table is null)
        {
            throw new ResourceNotFoundException($"Table with ID {reservationRequest.TableId} not found.");
        }

        if (!int.TryParse(table.Capacity, out int capacity) || !int.TryParse(reservationRequest.GuestsNumber, out int guestsNumber))
        {
            throw new ArgumentException("Invalid number format for Capacity or GuestsNumber.");
        }

        if (capacity < guestsNumber)
        {
            throw new ArgumentException($"Table with ID {reservationRequest.TableId} cannot accommodate {reservationRequest.GuestsNumber} guests. " +
                $"Maximum capacity: {table.Capacity}."
            );
        }

        var reservation = new Reservation
        {
            Id = reservationId,
            Date = reservationRequest.Date,
            FeedbackId = "NOT IMPLEMENTED YET AND ISN'T REQUIRED",
            GuestsNumber = reservationRequest.GuestsNumber,
            LocationAddress = location.Address,
            LocationId = location.Id,
            PreOrder = "NOT IMPLEMENTED YET",
            Status = Status.Reserved.ToString(),
            TableId = reservationRequest.TableId,
            TableNumber = table.TableNumber,
            TimeFrom = reservationRequest.TimeFrom,
            TimeTo = reservationRequest.TimeTo,
            TimeSlot = reservationRequest.TimeFrom + " - " + reservationRequest.TimeTo,
            UserEmail = user.Email,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };
        
        switch (reservationRequest)
        {
            case WaiterReservationRequest waiterRequest:
            {
                reservation.ClientType = waiterRequest.ClientType;
                reservation.UserInfo = waiterRequest.ClientType switch
                {
                    ClientType.CUSTOMER => $"Customer {waiterRequest.CustomerName}",
                    ClientType.VISITOR => $"Waiter {user.GetFullName()} (Visitor)",
                    _ => reservation.UserInfo
                };
                reservation.WaiterId = user.Id;
                    if (user.Role != Roles.Waiter)
                    {
                        throw new UnauthorizedException("Only waiters can create or modify waiter reservations");
                    }

                    break;
            }
            case ClientReservationRequest:
                reservation.UserEmail = user.Email;
                reservation.UserInfo = user.GetFullName();
                reservation.ClientType = ClientType.CUSTOMER;
                break;
            default:
                throw new Amazon.CognitoIdentityProvider.Model.UnsupportedOperationException("Unsupported ReservationRequest type");
        }

        var reservationExists = await _reservationRepository.ReservationExistsAsync(reservation.Id);
        
        if (!reservationExists)
        {
            reservation.WaiterId ??= await GetLeastBusyWaiter(reservation.LocationId, reservation.Date);
        }
        else
        {
            await ValidateModificationPermissions(reservation, user);
        }
        
        return await _reservationRepository.UpsertReservationAsync(reservation);
    }
    
    public async Task<List<Reservation>> GetReservationsAsync(ReservationsQueryParameters queryParams, string userId, string email, Roles role)
    {
        if (role == Roles.Customer)
        {
            return await _reservationRepository.GetCustomerReservationsAsync(email);
        }
        else
        {
            return await _reservationRepository.GetWaiterReservationsAsync(queryParams, userId);
        }   
    }

    public async Task CancelReservationAsync(string reservationId)
    {
        await _reservationRepository.CancelReservation(reservationId);
    }
    
    private async Task<string> GetLeastBusyWaiter(string locationId, string date) // Change parameter to locationId
    {
        var waiters = await _waiterRepository.GetWaitersByLocationAsync(locationId);
        
        var reservationCounts = new Dictionary<string, int>();

        foreach (var waiter in waiters)
        {
            var count = await _reservationRepository.GetWaiterReservationCountAsync(waiter.Id, date);
            reservationCounts[waiter.Id] = count;
        }

        return reservationCounts
            .OrderBy(x => x.Value)
            .FirstOrDefault().Key ?? throw new Exception($"No waiters available for location ID: {locationId} after counting reservations");
    }

    private async Task ValidateModificationPermissions(Reservation newReservation, User user)
    {
        var existingReservation = await _reservationRepository.GetReservationByIdAsync(newReservation.Id);

        // Check if user is either the customer or assigned waiter
        if (user.Email != existingReservation.UserEmail && user.Id != existingReservation.WaiterId)
        {
            throw new UnauthorizedException("Only the customer or assigned waiter can modify this reservation");
        }

        newReservation.WaiterId = existingReservation.WaiterId;
        newReservation.UserEmail = existingReservation.UserEmail;

        // Check 30-minute lock
        var reservationDateTime = DateTime.ParseExact(
            existingReservation.Date,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture
        ).Add(TimeSpan.Parse(existingReservation.TimeFrom));

        if (DateTime.UtcNow > reservationDateTime.AddMinutes(-30))
        {
            throw new ArgumentException("Reservations cannot be modified within 30 minutes of start time");
        }
    }
    public async Task<bool> CompleteReservationAsync(string reservationId)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
        reservation.Status = Status.Finished.ToString();

        await _reservationRepository.UpsertReservationAsync(reservation);
        Console.WriteLine($"Reservation Waiter Id before SQS message: {reservation.WaiterId}");
        await SendEventToSQS("reservation", reservation);

        return true;
    }


    private async Task SendEventToSQS<T>(string eventType, T payload)
    {
        var getQueueUrlRequest = new GetQueueUrlRequest
        {
            QueueName = _sqsQueueName
        };
        var getQueueUrlResponse = await _amazonSqsClient.GetQueueUrlAsync(getQueueUrlRequest);
        var queueUrl = getQueueUrlResponse.QueueUrl;

        var messageBody = JsonSerializer.Serialize(new
        {
            eventType,
            payload
        });

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = messageBody
        };

        await _amazonSqsClient.SendMessageAsync(sendMessageRequest);
    } 
}