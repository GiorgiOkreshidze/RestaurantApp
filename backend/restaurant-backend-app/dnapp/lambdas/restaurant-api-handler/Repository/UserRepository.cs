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
                { "createdAt", new AttributeValue { S = user.CreatedAt }},
                { "imageUrl", new AttributeValue { S = user.ImageUrl }},
            }
        };
        
        if (!string.IsNullOrEmpty(user.LocationId))
        {
            request.Item.Add("locationId", new AttributeValue { S = user.LocationId });
        }
        
        await _dynamoDBClient.PutItemAsync(request);
        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _usersTableName);
        var users = Mapper.MapDocumentsToUsers(documentList);
        var result = users.FirstOrDefault(user => user.Email == email);

        if (result == null) throw new ResourceNotFoundException($"The user with email: {email} is not found");

        return result;
    }

    public async Task<List<User>> GetAllCustomersAsync()
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _usersTableName);

        return Mapper.MapDocumentsToUsers(documentList)
            .Where(user => user.Role == Roles.Customer)
            .ToList();
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        var request = new GetItemRequest
        {
            TableName = _usersTableName,
            Key = new Dictionary<string, AttributeValue>
        {
            { "id", new AttributeValue { S = id } }
        }
        };

        var response = await _dynamoDBClient.GetItemAsync(request);

        if (response.Item == null || response.Item.Count == 0)
        {
            return null; // User not found
        }

        return new User
        {
            Id = response.Item["id"].S,
            FirstName = response.Item["firstName"].S,
            LastName = response.Item["lastName"].S,
            Email = response.Item["email"].S,
            Role = Enum.Parse<Roles>(response.Item["role"].S),
            CreatedAt = response.Item["createdAt"].S,
            ImageUrl = response.Item["imageUrl"].S,
            LocationId = response.Item.ContainsKey("locationId") ? response.Item["locationId"].S : null
        };
    }
}