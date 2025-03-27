using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Dishes;

public class GetAllDishesAction
{
    private readonly IDishService _dishService;

    public GetAllDishesAction()
    {
        _dishService = new DishService();
    }

    public async Task<APIGatewayProxyResponse> GetAllDishesAsync(APIGatewayProxyRequest request)
    {
        var dishType =  string.Empty;
        
        if (request.QueryStringParameters != null && 
            request.QueryStringParameters.TryGetValue("dishType", out var dishtypeValue))
        {
            dishType = dishtypeValue;
        }
        
        var sortBy = string.Empty;
        
        if (request.QueryStringParameters != null && 
            request.QueryStringParameters.TryGetValue("sort", out var sortByValue))
        {
            sortBy = sortByValue;
        }

        var getDishRequest = new GetallDishRequest
        {
            DishTypeEnum = dishType,
            SortBy = sortBy 
        };

        var popularDishes = await _dishService.GetAllDishAsync(getDishRequest);
        return ActionUtils.FormatResponse(200, popularDishes);
    }
}