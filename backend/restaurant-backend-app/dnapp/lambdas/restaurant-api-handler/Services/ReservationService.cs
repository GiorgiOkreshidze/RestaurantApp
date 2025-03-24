using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Requests;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Repository;

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
        var location = await _locationRepository.GetLocationByIdAsync(reservationRequest.LocationId);

        var existingReservations = await _reservationRepository.GetReservationsByDateLocationTable(
            reservationRequest.Date,
            location.Address,
            reservationRequest.TableId);

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
// here add if table is null then throw resoursenot found exception
        var table = await _tableRepository.GetTableById(reservationRequest.TableId);

        var reservation = new Reservation
        {
            Id = reservationId,
            Date = reservationRequest.Date,
            FeedbackId = "NOT IMPLEMENTED YET AND ISN'T REQUIRED",
            GuestsNumber = reservationRequest.GuestsNumber,
            LocationAddress = location.Address,
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

    public async Task<List<Reservation>> GetReservationsByDateLocationTable(string date, string locationAddress,
        string tableNumber)
    {
        return await _reservationRepository.GetReservationsByDateLocationTable(date, locationAddress, tableNumber);
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