using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Responses;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;

namespace Function.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;
    
    public FeedbackService()
    {
        _feedbackRepository = new FeedbackRepository();
    }
    
    public async Task<(List<LocationFeedbackResponse>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters queryParameters)
    {
        return await _feedbackRepository.GetLocationFeedbacksAsync(queryParameters);
    }
}