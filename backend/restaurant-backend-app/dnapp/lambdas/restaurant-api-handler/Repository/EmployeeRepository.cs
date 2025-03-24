using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    private readonly string? _waitersTableName = Environment.GetEnvironmentVariable("DYNAMODB_WAITERS_TABLE_NAME");
    private readonly string? _emailIndexName = Environment.GetEnvironmentVariable("WAITERS_TABLE_EMAIL_INDEX_NAME");
    
    public EmployeeRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
       
    }

    public async Task<bool> CheckIfEmailExistsInWaitersTableAsync(string email)
    {
        var request = new QueryRequest
        {
            TableName = _waitersTableName,
            IndexName = _emailIndexName,
            KeyConditionExpression = "email = :email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":email", new AttributeValue { S = email } }
            },
            Limit = 1
        };

        var response = await _dynamoDBClient.QueryAsync(request);
        return response.Count > 0;
    }
}