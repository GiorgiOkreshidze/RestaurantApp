using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Actions;

namespace Function.Actions;

public class GetSpecialityDishesAction
{
    private readonly IDynamoDBService _dynamoDBService;
    
    public GetSpecialityDishesAction()
    {
        _dynamoDBService = new DynamoDBService();
    }
    
    public async Task<APIGatewayProxyResponse> GetSpecialityDishes(APIGatewayProxyRequest request)
    {
        try
        {
            var locationId = request.PathParameters["id"];
            var popularDishes = await _dynamoDBService.GetListOfSpecialityDishes(locationId);
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