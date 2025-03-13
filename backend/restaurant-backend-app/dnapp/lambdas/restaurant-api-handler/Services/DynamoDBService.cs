using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Function.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Services
{
    public class DynamoDBService : IDynamoDBService
    {
        private readonly IDynamoDBContext _dynamoDBContext;
        private readonly AmazonDynamoDBClient _dynamoDBClient;
        private readonly string? _waitersTableName = Environment.GetEnvironmentVariable("DYNAMODB_WAITERS_TABLE_NAME");
        private readonly string? _emailIndexName = Environment.GetEnvironmentVariable("WAITERS_TABLE_EMAIL_INDEX_NAME");

        public DynamoDBService()
        {
            _dynamoDBClient = new AmazonDynamoDBClient();
            _dynamoDBContext = new DynamoDBContext(_dynamoDBClient);
        }

        public async Task<bool> CheckIfEmailExistsInWaitersTable(string email)
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
}
