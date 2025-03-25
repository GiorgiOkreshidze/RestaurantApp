using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Requests;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Repository;
using Function.Actions;
using Amazon.CognitoIdentityProvider.Model;
using Function.Models.User;
using ResourceNotFoundException = Function.Exceptions.ResourceNotFoundException;

namespace Function.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IWaiterRepository _waiterRepository;

    public ReservationService()
    {
        _reservationRepository = new ReservationRepository();
        _locationRepository = new LocationRepository();
        _tableRepository = new TableRepository();
        _waiterRepository = new WaiterRepository();
    }

    public async Task<Reservation> UpsertReservationAsync(ReservationRequest reservationRequest, User user)
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

        // Parse requested times
        var newTimeFrom = TimeSpan.Parse(reservationRequest.TimeFrom);
        var newTimeTo = TimeSpan.Parse(reservationRequest.TimeTo);

        // 1. Check if reservation is within working hours
        var firstSlot = TimeSpan.Parse(predefinedSlots.First().Start);
        var lastSlot = TimeSpan.Parse(predefinedSlots.Last().End);

        if (newTimeFrom < firstSlot || newTimeTo > lastSlot)
        {
            throw new ArgumentException("Reservation must be within restaurant working hours.");
        }

        // 2. Check if reservation exactly matches a predefined slot
        bool isValidSlot = predefinedSlots.Any(slot =>
            TimeSpan.Parse(slot.Start) == newTimeFrom &&
            TimeSpan.Parse(slot.End) == newTimeTo);

        if (!isValidSlot)
        {
            throw new ArgumentException("Reservation must exactly match one of the predefined time slots.");
        }
        
        var location = await _locationRepository.GetLocationByIdAsync(reservationRequest.LocationId);

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
                newTimeTo >= existingTimeFrom &&
                existingReservation.UserEmail != user.Email)
            {
                throw new ArgumentException(
                    $"Table #{reservationRequest.Id} at location " +
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
            UserInfo = user.FirstName + " " + user.LastName,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        var reservationExists = await _reservationRepository.ReservationExistsAsync(reservation.Id);
        
        if (!reservationExists)
        {
            reservation.WaiterId = await GetLeastBusyWaiter(reservation.LocationId, reservation.Date);
        }
        else
        {
            await ValidateModificationPermissions(reservation, user);
        }
        
        return await _reservationRepository.UpsertReservationAsync(reservation);
    }

    public async Task<List<Reservation>> GetCustomerReservationsAsync(string info)
    {
        return await _reservationRepository.GetCustomerReservationsAsync(info);
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
}