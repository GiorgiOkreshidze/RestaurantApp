using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Dishes;

public class GetDishByIdAction
{
    private readonly IDishService _dishService;

    public GetDishByIdAction()
    {
        _dishService = new DishService();
    }
    
    public async Task<APIGatewayProxyResponse> GetDishByIdAsync(APIGatewayProxyRequest request)
    {
        string dishId;

        try
        {
            dishId = request.PathParameters["id"];
        }
        catch (Exception)
        {
            throw new ArgumentException("Dish id is required.");
        }

        var dish = await _dishService.GetDishByIdAsync(dishId);
            
        return ActionUtils.FormatResponse(200, dish);
    }
}