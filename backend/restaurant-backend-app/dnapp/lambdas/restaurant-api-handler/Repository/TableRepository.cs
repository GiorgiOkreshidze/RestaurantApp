using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Function.Models;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class TableRepository : ITableRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    private readonly string? _tablesTableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLES_TABLE_NAME");
    private readonly string? _tablesTableLocationIndex = Environment.GetEnvironmentVariable("DYNAMODB_TABLES_TABLE_LOCATION_INDEX");


    public TableRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }

    public async Task<List<RestaurantTable>> GetTablesForLocationAsync(string locationId, int guests)
    {
        var tables = new List<RestaurantTable>();

        var queryRequest = new QueryRequest
        {
            TableName = _tablesTableName,
            IndexName = _tablesTableLocationIndex,
            KeyConditionExpression = "locationId = :locationId AND capacity >= :guests",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":locationId", new AttributeValue { S = locationId } },
            { ":guests", new AttributeValue { N = guests.ToString() } }
        }
        };

        var response = await _dynamoDBClient.QueryAsync(queryRequest);

        tables.AddRange(response.Items.Select(document => new RestaurantTable
        {
            Id = document["id"].S,
            TableNumber = document["tableNumber"].S,
            Capacity = document["capacity"].N,
            LocationId = document["locationId"].S,
            LocationAddress = document["locationAddress"].S,
        }));

        return tables;
    }

    public async Task<RestaurantTable?> GetTableById(string tableId)
    {

        var request = new GetItemRequest
        {
            TableName = _tablesTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = tableId } }
            }
        };

        var response = await _dynamoDBClient.GetItemAsync(request);

        if (response.Item == null || response.Item.Count == 0)
        {
            return null;
        }

        // Convert DynamoDB item to RestaurantTable
        var table = new RestaurantTable
        {
            Id = response.Item["id"].S,
            TableNumber = response.Item["tableNumber"].S,
            Capacity = response.Item["capacity"].S,
            LocationId = response.Item["locationId"].S,
            LocationAddress = response.Item["locationAddress"].S
        };

        return table;
    }
}