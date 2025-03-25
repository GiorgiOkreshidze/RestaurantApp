using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Mappers;
using Function.Models.User;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class UserRepository : IUserRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    private readonly string? _usersTableName = Environment.GetEnvironmentVariable("DYNAMODB_USERS_TABLE_NAME");
    
    public UserRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        var request = new PutItemRequest
        {
            TableName = _usersTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = user.Id }},
                { "firstName", new AttributeValue { S = user.FirstName }},
                { "lastName", new AttributeValue { S = user.LastName }},
                { "email", new AttributeValue { S = user.Email }},
                { "role", new AttributeValue { S = user.Role.ToString() }},
                { "locationId", new AttributeValue { S = user.LocationId ?? string.Empty }},
                { "createdAt", new AttributeValue { S = user.CreatedAt }},
            }
        };
        
        
        if (!string.IsNullOrEmpty(user.LocationId))
        {
            request.Item.Add("locationId", new AttributeValue { S = user.LocationId });
        }
        
        await _dynamoDBClient.PutItemAsync(request);
        return user;
    }

    public async Task<User> GetUserByIdAsync(string userId)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _usersTableName);
        var users = Mapper.MapDocumentsToUsers(documentList);
        var result = users.FirstOrDefault(user => user.Id == userId);

        if (result == null) throw new ResourceNotFoundException($"The location with {userId} id is not found");

        return result;
    }

    public async Task<List<User>> GetAllCustomersAsync()
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _usersTableName);

        return Mapper.MapDocumentsToUsers(documentList)
            .Where(user => user.Role == Roles.Customer)
            .ToList();
    }
}