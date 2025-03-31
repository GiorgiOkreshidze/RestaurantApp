using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Locations;

public class GetLocationByIdAction
{
    private readonly ILocationService _locationService;

    public GetLocationByIdAction()
    {
        _locationService = new LocationService();
    }
    
    public async Task<APIGatewayProxyResponse> GetLocationByIdAsync(APIGatewayProxyRequest request)
    {
        var locationId = request.PathParameters["id"];
        
        if (string.IsNullOrEmpty(locationId))
        {
            return ActionUtils.FormatResponse(400, "Location id is required.");
        }
        
        var location = await _locationService.GetLocationByIdAsync(locationId);
        
        return ActionUtils.FormatResponse(200, location);
    }
}