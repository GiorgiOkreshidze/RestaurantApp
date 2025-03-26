using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
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
        var popularDishes = await _dishService.GetAllDishAsync();
        return ActionUtils.FormatResponse(200, popularDishes);
    }
}