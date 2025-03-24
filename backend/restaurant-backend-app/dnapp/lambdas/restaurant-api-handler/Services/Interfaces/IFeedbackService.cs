using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Responses;

namespace Function.Services.Interfaces;

public interface IFeedbackService
{
    Task<(List<LocationFeedbackResponse>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters queryParameters);
}