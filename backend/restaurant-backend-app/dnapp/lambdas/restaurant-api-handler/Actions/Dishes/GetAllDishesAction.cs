using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Dishes;
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
        var getDishRequest = new GetAllDishesRequest
        {
            DishTypeEnum = InputParser<FilterEnum>(request, "dishType"),
            SortBy = InputParser<SortEnum>(request, "sort")
        };

        var popularDishes = await _dishService.GetAllDishesAsync(getDishRequest);
        return ActionUtils.FormatResponse(200, popularDishes);
    }
    
    private TEnum? InputParser<TEnum>(APIGatewayProxyRequest request, string key) where TEnum : struct
    {
        var returnKey = string.Empty;
        
        if (request.QueryStringParameters != null && 
            request.QueryStringParameters.TryGetValue(key, out var parsedValue))
        {
            returnKey = parsedValue;
        }
        
        var resultEnum = Enum.TryParse(returnKey, ignoreCase: true, out TEnum result) ? result : default;
        
        return resultEnum;
    }
}