using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Locations;

public class GetLocationsAction
{
    private readonly ILocationService _locationService;

    public GetLocationsAction()
    {
        _locationService = new LocationService();
    }

    public async Task<APIGatewayProxyResponse> GetLocationsAsync(APIGatewayProxyRequest request)
    {
        var locations = await _locationService.GetListOfLocationsAsync();
        
        return ActionUtils.FormatResponse(200, locations);
    }
}