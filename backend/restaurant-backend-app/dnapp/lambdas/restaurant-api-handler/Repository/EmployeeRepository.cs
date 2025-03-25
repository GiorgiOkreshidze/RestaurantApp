using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Models.User;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    private readonly string? _employeeInfoTableName = Environment.GetEnvironmentVariable("DYNAMODB_EMPLOYEE_INFO_TABLE_NAME");
    private readonly string? _emailIndexName = Environment.GetEnvironmentVariable("EMPLOYEE_INFO_TABLE_EMAIL_INDEX_NAME");
    
    public EmployeeRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
       
    }

    public async Task<EmployeeInfo?> GetEmployeeInfoByEmailAsync(string email)
    {
        var request = new QueryRequest
        {
            TableName = _employeeInfoTableName,
            IndexName = _emailIndexName,
            KeyConditionExpression = "email = :email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":email", new AttributeValue { S = email } }
            },
            Limit = 1
        };

        var response = await _dynamoDBClient.QueryAsync(request);
        
        return response.Items
            .Select(item => new EmployeeInfo
            {
                Email = item.TryGetValue("email", out var employeeEmail) ? employeeEmail.S : "",
                LocationId = item.TryGetValue("locationId", out var locationId) ? locationId.S : "",
                Role = item.TryGetValue("role", out var role) ? ParseRole(role.S) : Roles.Unknown
            }).FirstOrDefault();;
    }

    private Roles ParseRole(string role)
    {
        return Enum.TryParse<Roles>(role, out var parsedRole) ? parsedRole : Roles.Unknown;
    }
}