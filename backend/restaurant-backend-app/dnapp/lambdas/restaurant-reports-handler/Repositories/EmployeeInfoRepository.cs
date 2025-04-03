using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Repositories
{
    public class EmployeeInfoRepository : IEmployeeInfoRepository
    {
        public readonly IAmazonDynamoDB _dynamoDbClient;
        public readonly string? _tableName = System.Environment.GetEnvironmentVariable("EMPLOYEE_TABLE_NAME");

        public EmployeeInfoRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<string> GetEmployeeEmail(string employeeId)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "id = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":id", new AttributeValue { S = employeeId } }
                },
                Limit = 1
            };

            var response = await _dynamoDbClient.QueryAsync(request);

            return response.Items[0].TryGetValue("email", out var email) ? email.S : "";
        }
    }
}
