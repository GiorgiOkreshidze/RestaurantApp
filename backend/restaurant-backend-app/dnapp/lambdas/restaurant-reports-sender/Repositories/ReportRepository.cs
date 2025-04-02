using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Mappers;
using Function.Models;
using Function.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string? _tableName = Environment.GetEnvironmentVariable("REPORTS_TABLE_NAME");
        private const string FixedPartitionKey = "weekly";

        public ReportRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<List<Dictionary<string, AttributeValue>>> RetrieveReports(DateTime startDate, DateTime endDate)
        {
            Console.WriteLine($"Retrieving reports from {startDate} to {endDate}");

            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "#partition = :partition AND #date BETWEEN :startDate AND :endDate",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#partition", "partition" },
                    { "#date", "date#id" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":partition", new AttributeValue { S = FixedPartitionKey } },
                    { ":startDate", new AttributeValue { S = startDate.ToString("yyyy-MM-dd") } },
                    { ":endDate", new AttributeValue { S = endDate.ToString("yyyy-MM-dd") + "#z" } }
                }
            };

            var response = await _dynamoDbClient.QueryAsync(request);
            Console.WriteLine($"Retrieved {response.Items.Count} items from DynamoDB");
            return response.Items;
        }
    }
}
