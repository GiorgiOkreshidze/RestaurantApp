using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Dishes;

public class GetSpecialityDishesAction
{
    private readonly IDishService _dishService;

    public GetSpecialityDishesAction()
    {
        _dishService = new DishService();
    }

    public async Task<APIGatewayProxyResponse> GetSpecialityDishesAsync(APIGatewayProxyRequest request)
    {
        string locationId;

        try
        {
            locationId = request.PathParameters["id"];
        }
        catch (Exception)
        {
            throw new ArgumentException("Location id is required.");
        }

        var popularDishes = await _dishService.GetListOfSpecialityDishesAsync(locationId);
            
        return ActionUtils.FormatResponse(200, popularDishes);
    }
}