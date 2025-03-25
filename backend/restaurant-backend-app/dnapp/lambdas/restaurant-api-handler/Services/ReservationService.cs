using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Requests;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Repository;
using Function.Exceptions;
using Function.Actions;
using Function.Models.Reservations;
using System.Linq;

namespace Function.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ITableRepository _tableRepository;


    public ReservationService()
    {
        _reservationRepository = new ReservationRepository();
        _locationRepository = new LocationRepository();
        _tableRepository = new TableRepository();
    }

    public async Task<Reservation> UpsertReservationAsync(ReservationRequest reservationRequest, string fullName)
    {
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
            if (newTimeFrom < existingTimeTo &&
                newTimeTo > existingTimeFrom &&
                existingReservation.UserInfo != fullName)
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
            UserInfo = fullName,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };

        return await _reservationRepository.UpsertReservation(reservation);
    }


    public async Task<List<Reservation>> GetCustomerReservationsAsync(string info)
    {
        return await _reservationRepository.GetCustomerReservationsAsync(info);
    }

    public async Task CancelReservationAsync(string reservationId)
    {
        await _reservationRepository.CancelReservation(reservationId);
    }
}