using System.Threading.Tasks;
using Function.Models.Requests;
using Function.Models.Reservations;
using Function.Models.Responses;
using Function.Models.User;

namespace Function.Services.Interfaces;

public interface ILocationFeedbackFactory
{
    Task<LocationFeedback[]> CreateFeedbacksAsync(ReservationFeedbackRequest request, User user, Reservation reservation);
}