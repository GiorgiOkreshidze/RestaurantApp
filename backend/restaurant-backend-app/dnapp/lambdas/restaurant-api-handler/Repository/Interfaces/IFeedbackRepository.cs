using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Feedbacks;
using Function.Models.Responses;

namespace Function.Repository.Interfaces;

public interface IFeedbackRepository
{
    Task<(List<LocationFeedbackResponse>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters queryParameters);
    
    Task<ReservationFeedback> UpsertReservationFeedbackAsync(ReservationFeedback feedback);
}