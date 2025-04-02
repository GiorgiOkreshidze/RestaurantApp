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
using ResourceNotFoundException = Function.Exceptions.ResourceNotFoundException;
using Amazon.SQS.Model;
using Amazon.SQS;
using System.Text.Json;
using Function.Models.Responses;
using Function.Models.Requests.Base;

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

    public async Task<Reservation> UpsertReservationAsync(BaseReservationRequest reservationRequest, string userId)
    {
        ValidateTimeSlot(reservationRequest);
        var location = await _locationRepository.GetLocationByIdAsync(reservationRequest.LocationId);
        var table = await GetAndValidateTable(reservationRequest.TableId, reservationRequest.GuestsNumber);

        var reservation = new Reservation
        {
            Id = reservationRequest.Id ?? Guid.NewGuid().ToString(),
            Date = reservationRequest.Date,
            GuestsNumber = reservationRequest.GuestsNumber,
            LocationAddress = location.Address,
            LocationId = location.Id,
            PreOrder = "NOT IMPLEMENTED YET",
            Status = ReservationStatus.Reserved.ToString(),
            TableId = reservationRequest.TableId,
            TableNumber = table.TableNumber,
            TimeFrom = reservationRequest.TimeFrom,
            TimeTo = reservationRequest.TimeTo,
            TimeSlot = reservationRequest.TimeFrom + " - " + reservationRequest.TimeTo,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };

        return reservationRequest switch
        {
            WaiterReservationRequest waiterRequest => await ProcessWaiterReservation(waiterRequest, reservation, userId, location.Id),
            ClientReservationRequest clientRequest => await ProcessClientReservation(clientRequest, reservation, userId),
            _ => throw new UnsupportedOperationException("Unsupported ReservationRequest type")
        };
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

    private async Task<Reservation> ProcessWaiterReservation(
    WaiterReservationRequest request,
    Reservation reservation,
    string waiterId, string locationId)
    {
        reservation.ClientType = request.ClientType;
        reservation.WaiterId = waiterId;

        var waiter = await ValidateUser(waiterId);
   
        if (waiter.LocationId != locationId)
        {
            throw new UnauthorizedException("Waiter cannot create reservations for a different location.");
        }

        var reservationExists = await _reservationRepository.ReservationExistsAsync(reservation.Id);

        if (reservationExists)
        {
            await ValidateModificationPermissionsForWaiter(reservation, waiterId);
        }

        if (request.ClientType == ClientType.CUSTOMER && request.CustomerId != null)
        {
            var customer = await ValidateUser(request.CustomerId);
            reservation.UserEmail = customer.Email;
            reservation.UserInfo = $"Customer {customer.FirstName} {customer.LastName}";

            await CheckForConflictingReservations(request, reservation.LocationAddress, customer.Email);
        }
        else
        {
            reservation.UserEmail = waiter.Email;
            reservation.UserInfo = $"Waiter {waiter.GetFullName()} (Visitor)";

            await CheckForConflictingReservations(request, reservation.LocationAddress, waiter.Email);
        }

        return await _reservationRepository.UpsertReservationAsync(reservation);
    }

    private async Task<Reservation> ProcessClientReservation(ClientReservationRequest request, Reservation reservation, string userId)
    {
        var user = await ValidateUser(userId);
        HandleClientReservation(reservation, user);

        await CheckForConflictingReservations(request, reservation.LocationAddress, user.Email);
        var isNewReservation = !await _reservationRepository.ReservationExistsAsync(reservation.Id);

        if (isNewReservation)
        {
            reservation.WaiterId ??= await GetLeastBusyWaiter(reservation.LocationId, reservation.Date);
        }
        else
        {
            await ValidateModificationPermissionsForClient(reservation, user);
        }

        return await _reservationRepository.UpsertReservationAsync(reservation);
    }

    private async Task<User> ValidateUser(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new UnauthorizedException("User is not registered");
        }
        return user;
    }

    private void ValidateTimeSlot(BaseReservationRequest request)
    {
        var predefinedSlots = ActionUtils.GeneratePredefinedTimeSlots();
        var newTimeFrom = TimeSpan.Parse(request.TimeFrom);
        var newTimeTo = TimeSpan.Parse(request.TimeTo);

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
    }

    private async Task<RestaurantTable> GetAndValidateTable(string tableId, string guestsNumber)
    {
        var table = await _tableRepository.GetTableById(tableId);
        if (table is null)
        {
            throw new ResourceNotFoundException($"Table with ID {tableId} not found.");
        }

        if (!int.TryParse(table.Capacity, out int capacity) ||
            !int.TryParse(guestsNumber, out int guests))
        {
            throw new ArgumentException("Invalid number format for Capacity or GuestsNumber.");
        }

        if (capacity < guests)
        {
            throw new ArgumentException(
                $"Table with ID {tableId} cannot accommodate {guestsNumber} guests. " +
                $"Maximum capacity: {table.Capacity}.");
        }

        return table;
    }

    private void HandleClientReservation(Reservation reservation, User user)
    {
        reservation.UserEmail = user.Email;
        reservation.UserInfo = user.GetFullName();
        reservation.ClientType = ClientType.CUSTOMER;
    }

    private async Task CheckForConflictingReservations(
    BaseReservationRequest request,
    string locationAddress,
    string userEmail)
    {
        var existingReservations = await _reservationRepository.GetReservationsByDateLocationTable(
            request.Date,
            locationAddress,
            request.TableId);

        var newTimeFrom = TimeSpan.Parse(request.TimeFrom);
        var newTimeTo = TimeSpan.Parse(request.TimeTo);

        foreach (var existingReservation in existingReservations)
        {
            var existingTimeFrom = TimeSpan.Parse(existingReservation.TimeFrom);
            var existingTimeTo = TimeSpan.Parse(existingReservation.TimeTo);

            if (newTimeFrom <= existingTimeTo && newTimeTo >= existingTimeFrom)
            {
                if (existingReservation.UserEmail == userEmail)
                {
                    throw new ArgumentException(
                        $"You already have reservation booked at location " +
                        $"{locationAddress} during the requested time period.");
                }

                throw new ArgumentException(
                    $"Reservation #{request.Id} at location " +
                    $"{locationAddress} is already booked during the requested time period.");
            }
        }
    }

    private async Task<string> GetLeastBusyWaiter(string locationId, string date)
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

    private async Task ValidateModificationPermissionsForWaiter(Reservation newReservation, string waiterId)
    {
        var existingReservation = await _reservationRepository.GetReservationByIdAsync(newReservation.Id);

        if (waiterId != existingReservation.WaiterId)
        {
            throw new UnauthorizedException("Only assigned waiter can modify this reservation");
        }
        
        ValidateReservationTimeLock(existingReservation);
    }

    private async Task ValidateModificationPermissionsForClient(Reservation newReservation, User user)
    {
        var existingReservation = await _reservationRepository.GetReservationByIdAsync(newReservation.Id);

        if (user.Email != existingReservation.UserEmail)
        {
            throw new UnauthorizedException("Only the customer or assigned waiter can modify this reservation");
        }
        newReservation.WaiterId = existingReservation.WaiterId;
        ValidateReservationTimeLock(existingReservation);
    }

    private void ValidateReservationTimeLock(Reservation reservation)
    {
        var reservationDateTime = DateTime.ParseExact(
            reservation.Date,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture
        ).Add(TimeSpan.Parse(reservation.TimeFrom));

        if (DateTime.UtcNow > reservationDateTime.AddMinutes(-30))
        {
            throw new ArgumentException("Reservations cannot be modified within 30 minutes of start time");
        }
    }
    public async Task<bool> CompleteReservationAsync(string reservationId)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
        reservation.Status = ReservationStatus.Finished.ToString();

        await _reservationRepository.UpsertReservationAsync(reservation);
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