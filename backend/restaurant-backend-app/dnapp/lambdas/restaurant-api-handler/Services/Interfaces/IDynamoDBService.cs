using Function.Models;
using Function.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Services.Interfaces
{
    public interface IDynamoDBService
    {
        public Task<bool> CheckIfEmailExistsInWaitersTable(string email);

        public Task<List<Location>> GetListOfLocations();

        Task<List<LocationOptions>> GetLocationDropdownOptions();

        public Task<List<Dish>> GetListOfPopularDishes();
        
        public Task<List<Dish>> GetListOfSpecialityDishes(string locationId);

        public Task<Reservation> UpsertReservation(Reservation reservationRequest);
        
        public Task<Location> GetLocationById(string locationId);
        
        public Task<List<Reservation>> GetReservationsByDateLocationTable(string date, string locationAddress, string tableNumber);

        public Task<(List<LocationFeedbackResponse>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters qeuryParameters);

        Task<List<RestaurantTable>> GetTablesForLocation(string locationId, int guests);

        Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationAddress);

        Task<LocationInfo?> GetLocationDetails(string locationId);

        public Task<List<Reservation>> GetCustomerReservationsAsync(string info);
    }
}
