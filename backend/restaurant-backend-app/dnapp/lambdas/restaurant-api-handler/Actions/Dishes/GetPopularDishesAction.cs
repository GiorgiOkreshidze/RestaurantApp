using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Dishes;

public class GetPopularDishesAction
{
    private readonly IDishService _dishService;
    
    public GetPopularDishesAction()
    {
        _dishService = new DishService();
    }

    public async Task<APIGatewayProxyResponse> GetPopularDishesAsync(APIGatewayProxyRequest request)
    {
        var popularDishes = await _dishService.GetListOfPopularDishesAsync();
        return ActionUtils.FormatResponse(200, popularDishes);
    }
}