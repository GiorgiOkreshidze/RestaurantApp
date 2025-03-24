using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Repository;

namespace Function.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationService()
    {
        _reservationRepository = new ReservationRepository();
    }

    public async Task<Reservation> UpsertReservationAsync(Reservation reservation)
    {
        if (int.TryParse(reservation.GuestsNumber, out int guests) && guests > 10)
        {
            throw new ArgumentException("The maximum number of guests allowed is 10");
        }

        var existingReservations = await _reservationRepository.GetReservationsByDateLocationTable(
            reservation.Date, reservation.LocationAddress, reservation.TableNumber);

        var newTimeFrom = TimeSpan.Parse(reservation.TimeFrom);
        var newTimeTo = TimeSpan.Parse(reservation.TimeTo);

        foreach (var existingReservation in existingReservations)
        {
            var existingTimeFrom = TimeSpan.Parse(existingReservation.TimeFrom);
            var existingTimeTo = TimeSpan.Parse(existingReservation.TimeTo);

            if (newTimeFrom < existingTimeTo && newTimeTo > existingTimeFrom &&
                existingReservation.UserInfo != reservation.UserInfo)
            {
                throw new ArgumentException(
                    $"Table #{reservation.TableNumber} at location {reservation.LocationAddress} is already booked during the requested time.");
            }
        }

        return await _reservationRepository.UpsertReservation(reservation);
    }

    public async Task<List<Reservation>> GetReservationsByDateLocationTable(string date, string locationAddress,
        string tableNumber)
    {
        return await _reservationRepository.GetReservationsByDateLocationTable(date, locationAddress, tableNumber);
    }

    public async Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationAddress)
    {
        return await _reservationRepository.GetReservationsForDateAndLocation(date, locationAddress);
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