using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Function.Mappers;
using Function.Models;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class DishRepository : IDishRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    
    private readonly string? _dishesTableName = Environment.GetEnvironmentVariable("DYNAMODB_DISHES_TABLE_NAME");

    public DishRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }
    
    public async Task<List<Dish>> GetListOfPopularDishesAsync()
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _dishesTableName);
        var filteredDishes = documentList
            .Where(doc =>
                doc.TryGetValue("isPopular", out var isPopular) &&
                isPopular.AsBoolean())
            .ToList();
        
        return Mapper.MapDocumentsToDishes(filteredDishes);
    }

    public async Task<List<Dish>> GetListOfSpecialityDishesAsync(string locationId)
    {
        var documentList =await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient,_dishesTableName);
        var filteredDocuments = documentList
            .Where(doc =>
                doc.TryGetValue("locationId", out var docLocationId) &&
                docLocationId.ToString() == locationId)
            .ToList();

        return Mapper.MapDocumentsToDishes(filteredDocuments);
    }
}