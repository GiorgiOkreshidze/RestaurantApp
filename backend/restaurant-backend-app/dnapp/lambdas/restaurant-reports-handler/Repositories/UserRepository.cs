using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly IAmazonDynamoDB _dynamoDbClient;
        public readonly string? _tableName = Environment.GetEnvironmentVariable("USER_TABLE_NAME");

        public UserRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<string> GetUserFullName(string id)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "id = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":id", new AttributeValue { S = id } }
                },
                Limit = 1
            };

            var response = await _dynamoDbClient.QueryAsync(request);
            var firstName = response.Items[0].TryGetValue("firstName", out var firstNameValue) ? firstNameValue.S : "";
            var lastName = response.Items[0].TryGetValue("lastName", out var lastNameValue) ? lastNameValue.S : "";

            return $"{firstName} {lastName}";
        }
    }
}
