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
using Function.Models.Requests.Base;

namespace Function.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IWaiterRepository _waiterRepository;

    public ReservationService()
    {
        _reservationRepository = new ReservationRepository();
        _userRepository = new UserRepository();
        _locationRepository = new LocationRepository();
        _tableRepository = new TableRepository();
        _waiterRepository = new WaiterRepository();
    }

    public async Task<Reservation> UpsertReservationAsync(BaseReservationRequest reservationRequest, string userId)
    {
        var user = await ValidateUser(userId);
        ValidateTimeSlot(reservationRequest);
      
        var location = await _locationRepository.GetLocationByIdAsync(reservationRequest.LocationId);
        var table = await GetAndValidateTable(reservationRequest.TableId, reservationRequest.GuestsNumber);
        await CheckForConflictingReservations(reservationRequest, location.Address, user.Email);
        
        var reservationId = Guid.NewGuid().ToString();

        if (reservationRequest.Id != null)
        {
            reservationId = reservationRequest.Id;
        }

        var reservation = new Reservation
        {
            Id = reservationId,
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
            UserEmail = user.Email,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };
        
        switch (reservationRequest)
        {
            case WaiterReservationRequest waiterRequest:
                 HandleWaiterReservation(waiterRequest, reservation, user);
                 break;
            case ClientReservationRequest:
                HandleClientReservation(reservation, user);
                break;
            default:
                throw new UnsupportedOperationException("Unsupported ReservationRequest type");
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

    private void HandleWaiterReservation(
    WaiterReservationRequest request,
    Reservation reservation,
    User user)
    {
        if (user.Role != Roles.Waiter)
        {
            throw new UnauthorizedException("Only waiters can create or modify waiter reservations");
        }

        reservation.ClientType = request.ClientType;
        reservation.UserInfo = request.ClientType switch
        {
            ClientType.CUSTOMER => $"Customer {request.CustomerName}",
            ClientType.VISITOR => $"Waiter {user.GetFullName()} (Visitor)",
            _ => reservation.UserInfo
        };
        reservation.WaiterId = user.Id;
    }

    private void HandleClientReservation(Reservation reservation, User user)
    {
        reservation.UserEmail = user.Email;
        reservation.UserInfo = user.GetFullName();
        reservation.ClientType = ClientType.CUSTOMER;
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
}