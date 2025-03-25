using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Function.Mappers;
using Function.Models.User;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class WaiterRepository : IWaiterRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    private readonly string? _usersTableName = Environment.GetEnvironmentVariable("DYNAMODB_USERS_TABLE_NAME");
    
    public WaiterRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }
    
    public async Task<List<User>> GetWaitersByLocationAsync(string locationId)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _usersTableName);
        var filteredDocuments = documentList
            .Where(doc =>
                doc.TryGetValue("locationId", out var docLocationId) &&
                docLocationId.ToString() == locationId &&
                doc.TryGetValue("role", out var role) &&
                role.ToString() == "Waiter") // Filter for waiters only
            .ToList();

        return Mapper.MapDocumentsToUsers(filteredDocuments);
    }
}