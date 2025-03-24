using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Function.Models;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class TableRepository : ITableRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    private readonly string? _tablesTableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLES_TABLE_NAME");
    
    public TableRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }

    public async Task<List<RestaurantTable>> GetTablesForLocationAsync(string locationId, int guests)
    {
        var tables = new List<RestaurantTable>();
        var table = Table.LoadTable(_dynamoDBClient, _tablesTableName);
        var scanFilter = new ScanFilter();
        
        scanFilter.AddCondition("locationId", ScanOperator.Equal, locationId);
        scanFilter.AddCondition("capacity", ScanOperator.GreaterThanOrEqual, guests);
        var search = table.Scan(scanFilter);

        do
        {
            var documents = await search.GetNextSetAsync();
            
            tables.AddRange(documents.Select(document => new RestaurantTable
            {
                Id = document["id"].AsString(),
                TableNumber = document["tableNumber"].AsString(),
                Capacity = document["capacity"].AsInt().ToString(),
                LocationId = document["locationId"].AsString(),
                LocationAddress = document["locationAddress"].AsString(),
            }));
        } while (!search.IsDone);

        return tables;
    }
}