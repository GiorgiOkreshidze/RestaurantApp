using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Actions;

namespace Function.Actions;

public class GetPopularDishesAction
{
    private readonly IDynamoDBService _dynamoDBService;
    
    public GetPopularDishesAction()
    {
        _dynamoDBService = new DynamoDBService();
    }

    public async Task<APIGatewayProxyResponse> GetPopularDishes(APIGatewayProxyRequest request)
    {
        try
        {
            var popularDishes = await _dynamoDBService.GetListOfPopularDishes();
            return ActionUtils.FormatResponse(200, popularDishes);
        }
        catch (Exception e)
        {
            return ActionUtils.FormatResponse(500, new
            {
                message = e.Message
            });
        }
    }
}