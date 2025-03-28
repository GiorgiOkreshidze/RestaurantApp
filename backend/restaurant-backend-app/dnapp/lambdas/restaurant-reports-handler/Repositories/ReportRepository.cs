using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Models;
using Function.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Repositories
{
    public class ReportRepository : IReportRepository
    {
        public readonly IAmazonDynamoDB _dynamoDbClient;
        public readonly string? _tableName = System.Environment.GetEnvironmentVariable("REPORTS_TABLE_NAME");
        public ReportRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task SaveReportAsync(Report report)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = report.Id } },
                { "location", new AttributeValue { S = report.Location } },
                { "date", new AttributeValue { S = report.Date } },
                { "waiter", new AttributeValue { S = report.Waiter } },
                { "waiterEmail", new AttributeValue { S = report.WaiterEmail } },
                { "hoursWorked", new AttributeValue { N = report.HoursWorked.ToString() } }
            };

            await _dynamoDbClient.PutItemAsync(new PutItemRequest
            {
                TableName = _tableName,
                Item = item
            });
        }
    }
}
