using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Mappers;
using Function.Models;
using Function.Repository;
using Function.Repository.Interfaces;
using static System.String;
using ResourceNotFoundException = Function.Exceptions.ResourceNotFoundException;

namespace SimpleLambdaFunction.Repository;

public class LocationRepository : ILocationRepository
{
    private readonly AmazonDynamoDBClient _dynamoDbClient;

    private readonly string? _locationsTableName = Environment.GetEnvironmentVariable("DYNAMODB_LOCATIONS_TABLE_NAME");

    public LocationRepository()
    {
        _dynamoDbClient = new AmazonDynamoDBClient();
    }

    public async Task<List<Location>> GetListOfLocationsAsync()
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDbClient, _locationsTableName);

        return Mapper.MapDocumentsToLocations(documentList);
    }

    public async Task<List<LocationOptions>> GetLocationDropdownOptionsAsync()
    {
        var request = new ScanRequest
        {
            TableName = _locationsTableName,
            ProjectionExpression = "id, address"
        };
        var locations = await _dynamoDbClient.ScanAsync(request);

        return locations.Items
            .Select(item => new LocationOptions
            {
                Id = item.TryGetValue("id", out var id) ? id.S : "",
                Address = item.TryGetValue("address", out var address) ? address.S : ""
            }).ToList();
    }

    public async Task<Location> GetLocationByIdAsync(string locationId)
    {
        var request = new GetItemRequest
        {
            TableName = _locationsTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = locationId } }
            }
        };

        var response = await _dynamoDbClient.GetItemAsync(request);

        if (response.Item == null || response.Item.Count == 0)
        {
            throw new ResourceNotFoundException($"The location with {locationId} id is not found");
        }

        var location = new Location
        {
            Id = response.Item["id"].S ?? Empty,
            Address = response.Item["address"].S ?? Empty,
            Description = response.Item["description"].S ?? Empty,
            TotalCapacity = response.Item["totalCapacity"].S ?? Empty,
            AverageOccupancy = response.Item["averageOccupancy"].S ?? Empty,
            ImageUrl = response.Item["imageUrl"].S ?? Empty,
            Rating = response.Item["rating"].S ?? Empty,
        };

        return location;
    }
}