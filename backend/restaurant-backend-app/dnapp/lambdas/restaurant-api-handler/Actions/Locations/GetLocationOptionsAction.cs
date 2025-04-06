using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Locations;

public class GetLocationOptionsAction
{
    private readonly ILocationService _locationService;

    public GetLocationOptionsAction()
    {
        _locationService = new LocationService();
    }

    public async Task<APIGatewayProxyResponse> GetOptionsAsync(APIGatewayProxyRequest request)
    {
        var locationsOptions = await _locationService.GetLocationDropdownOptionsAsync();
        
        return ActionUtils.FormatResponse(200, locationsOptions);
    }
}