using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Function.Exceptions;
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

    public async Task<ExactDishResponse> GetDishByIdAsync(string dishId)
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

    public async Task<List<AllDishResponse>> GetAllDishesAsync(GetAllDishesRequest getAllDishesRequest)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDbClient, _dishesTableName);

        if (documentList.Count == 0) return [];

        var dishes = Mapper.MapDocumentsToDishesResponseDtos(documentList);

        // FilterByTypeOfDish
        if (getAllDishesRequest.DishTypeEnum is not null)
        {
            dishes = FilterDishByType(dishes, getAllDishesRequest.DishTypeEnum);
        }

        // SortBy
        if (getAllDishesRequest.SortBy is not null)
        {
            dishes = SortDishesByInput(dishes, getAllDishesRequest.SortBy);
        }

        ConvertPriceToCurrency(dishes);

        return dishes;
    }

    private List<AllDishResponse> SortDishesByInput(List<AllDishResponse> dishes, SortEnum? sortInput)
    {
        return sortInput switch
        {
            SortEnum.PopularityAsc => dishes.OrderBy(dish => ConvertBoolToInt(dish.IsPopular)).ToList(),
            SortEnum.PopularityDesc => dishes.OrderByDescending(dish => ConvertBoolToInt(dish.IsPopular)).ToList(),
            SortEnum.PriceAsc => dishes.OrderBy(dish => ConvertPrice(dish.Price)).ToList(),
            SortEnum.PriceDesc => dishes.OrderByDescending(dish => ConvertPrice(dish.Price)).ToList(),
            _ => dishes
        };
    }

    private List<AllDishResponse> FilterDishByType(List<AllDishResponse> dishes, FilterEnum? dishTypeEnum)
    {
        return dishTypeEnum switch
        {
            FilterEnum.Appetizers => dishes
                .Where(dish => dish.DishType == nameof(FilterEnum.Appetizers)).ToList(),
            FilterEnum.Desserts => dishes
                .Where(dish => dish.DishType == nameof(FilterEnum.Desserts)).ToList(),
            FilterEnum.MainCourse => dishes
                .Where(dish => dish.DishType == nameof(FilterEnum.MainCourse)).ToList(),
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

    private void ConvertPriceToCurrency(List<AllDishResponse> dishes)
    {
        foreach (var dish in dishes)
        {
            dish.Price = $"{ConvertPrice(dish.Price):C}";
        }
    }

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