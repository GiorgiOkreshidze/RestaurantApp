using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Function.Mappers;
using Function.Models;
using Function.Repository;
using Function.Repository.Interfaces;
using ResourceNotFoundException = Function.Exceptions.ResourceNotFoundException;

namespace SimpleLambdaFunction.Repository;

public class LocationRepository : ILocationRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;

    private readonly string? _locationsTableName = Environment.GetEnvironmentVariable("DYNAMODB_LOCATIONS_TABLE_NAME");

    public LocationRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }

    public async Task<List<Location>> GetListOfLocationsAsync()
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _locationsTableName);

        return Mapper.MapDocumentsToLocations(documentList);
    }

    public async Task<List<LocationOptions>> GetLocationDropdownOptionsAsync()
    {
        var request = new ScanRequest
        {
            TableName = _locationsTableName,
            ProjectionExpression = "id, address"
        };
        var locations = await _dynamoDBClient.ScanAsync(request);

        return locations.Items
            .Select(item => new LocationOptions
            {
                Id = item.TryGetValue("id", out var id) ? id.S : "",
                Address = item.TryGetValue("address", out var address) ? address.S : ""
            }).ToList();
    }

    public async Task<Location> GetLocationByIdAsync(string locationId)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _locationsTableName);
        var locations = Mapper.MapDocumentsToLocations(documentList);
        var result = locations.FirstOrDefault(loc => loc.Id == locationId);

        if (result == null) throw new ResourceNotFoundException($"The location with {locationId} id is not found");

        return result;
    }

    public async Task<LocationInfo> GetLocationDetailsAsync(string locationId)
    {
        var table = Table.LoadTable(_dynamoDBClient, _locationsTableName);
        var document = await table.GetItemAsync(locationId);
            
        if (document == null) throw new ResourceNotFoundException("Location detail not found");

        return new LocationInfo
        {
            LocationId = locationId,
            Address = document["address"].AsString(),
            TotalCapacity = document["totalCapacity"].AsString(),
            AverageOccupancy = document["averageOccupancy"].AsString()
        };
    }
}