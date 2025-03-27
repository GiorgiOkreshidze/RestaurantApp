using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Function.Exceptions;
using Function.Helper;
using Function.Mappers;
using Function.Models.Dishes;
using Function.Models.Requests;
using Function.Models.Responses;
using Function.Repository.Interfaces;
using AmazonDynamoDBClient = Amazon.DynamoDBv2.AmazonDynamoDBClient;

namespace Function.Repository;

public class DishRepository : IDishRepository
{
    private readonly AmazonDynamoDBClient _dynamoDbClient;

    private readonly string? _dishesTableName = Environment.GetEnvironmentVariable("DYNAMODB_DISHES_TABLE_NAME");

    public DishRepository()
    {
        _dynamoDbClient = new AmazonDynamoDBClient();
    }

    public async Task<ExactDishResponseDto> GetDishByIdAsync(string dishId)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDbClient, _dishesTableName);

        if (documentList.Count == 0) throw new ResourceNotFoundException("Dish not found.");

        var filteredDocuments = documentList
            .Where(doc =>
                doc.TryGetValue("id", out var docLocationId) &&
                docLocationId.ToString() == dishId)
            .ToList();

        if (filteredDocuments.Count == 0) throw new ResourceNotFoundException("Dish not found.");

        var dishes = Mapper.MapDocumentsToExactDishResponseDtos(filteredDocuments).FirstOrDefault()
                     ?? throw new ResourceNotFoundException("Dish not found.");
        
        return dishes;
    }

    public async Task<IEnumerable<AllDishResponseDto>> GetAllDishAsync(GetallDishRequest getallDishRequest)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDbClient, _dishesTableName);

        if (documentList.Count == 0) return [];

        var dishes = Mapper.MapDocumentsToDishesResponseDtos(documentList);

        // FilterByTypeOfDish
        if (!string.IsNullOrWhiteSpace(getallDishRequest.DishTypeEnum))
        {
            dishes = FilterDishByType(dishes, getallDishRequest.DishTypeEnum);
        }

        // SortBy
        if (!string.IsNullOrEmpty(getallDishRequest.SortBy))
        {
            dishes = SortDishesByInput(dishes, getallDishRequest.SortBy);
        }

        ConvertPriceToCurrency(dishes);
        
        return dishes;
    }

    #region GetAllDishAsync

    private List<AllDishResponseDto> SortDishesByInput(List<AllDishResponseDto> dishes, string sortInput)
    {
        return sortInput switch
        {
            SortInputs.PopularityAsc => dishes.OrderBy(dish => ConvertBoolToInt(dish.IsPopular)).ToList(),
            SortInputs.PopularityDesc => dishes.OrderByDescending(dish => ConvertBoolToInt(dish.IsPopular)).ToList(),
            SortInputs.PriceAsc => dishes.OrderBy(dish => ConvertPrice(dish.Price)).ToList(),
            SortInputs.PriceDesc => dishes.OrderByDescending(dish => ConvertPrice(dish.Price)).ToList(),
            _ => dishes
        };
    }

    private List<AllDishResponseDto> FilterDishByType(List<AllDishResponseDto> dishes, string? dishTypeEnum)
    {
        return dishTypeEnum switch
        {
            FilterInputs.Appetizers => dishes
                .Where(dish => dish.DishType == FilterInputs.Appetizers).ToList(),
            FilterInputs.Desserts => dishes
                .Where(dish => dish.DishType == FilterInputs.Desserts).ToList(),
            FilterInputs.MainCourse => dishes
                .Where(dish => dish.DishType == FilterInputs.MainCourse).ToList(),
            _ => dishes
        };
    }

    private decimal ConvertPrice(string price)
    {
        return decimal.TryParse(price, out var parsedPrice) ? parsedPrice : 0;
    }

    private int ConvertBoolToInt(bool isPopular)
    {
        return isPopular ? 1 : 0;
    }

    private void ConvertPriceToCurrency(List<AllDishResponseDto> dishes)
    {
        foreach (var dish in dishes)
        {
            dish.Price = $"{ConvertPrice(dish.Price):C}";
        }
    }
    private void ConvertPriceToCurrency(List<ExactDishResponseDto> dishes)
    {
        foreach (var dish in dishes)
        {
            dish.Price = $"{ConvertPrice(dish.Price):C}";
        }
    }
    #endregion

    public async Task<List<Dish>> GetListOfPopularDishesAsync()
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDbClient, _dishesTableName);
        var filteredDishes = documentList
            .Where(doc =>
                doc.TryGetValue("isPopular", out var isPopular) &&
                isPopular.AsBoolean())
            .ToList();

        return Mapper.MapDocumentsToDishes(filteredDishes);
    }

    public async Task<List<Dish>> GetListOfSpecialityDishesAsync(string locationId)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDbClient, _dishesTableName);
        var filteredDocuments = documentList
            .Where(doc =>
                doc.TryGetValue("locationId", out var docLocationId) &&
                docLocationId.ToString() == locationId)
            .ToList();

        return Mapper.MapDocumentsToDishes(filteredDocuments);
    }
}