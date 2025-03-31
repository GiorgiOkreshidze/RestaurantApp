using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Feedbacks;

public class AddFeedbackAction
{
    private readonly IFeedbackService _feedbackService;
    
    public AddFeedbackAction()
    {
        _feedbackService = new FeedbackService();
    }
    
    public async Task<APIGatewayProxyResponse> AddFeedbackAsync(APIGatewayProxyRequest request)
    {
        var feedback = JsonSerializer.Deserialize<FeedbackRequest>(request.Body);
        
        await _feedbackService.AddFeedbackAsync(feedback);
        
        return ActionUtils.FormatResponse(201, new { message = "Feedback added successfully" });
    }
}