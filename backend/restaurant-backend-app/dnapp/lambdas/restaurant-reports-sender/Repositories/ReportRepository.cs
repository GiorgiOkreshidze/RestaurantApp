using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Mappers;
using Function.Models;
using Function.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string? _tableName = Environment.GetEnvironmentVariable("REPORTS_TABLE_NAME");
        private readonly string? _dateIndex = Environment.GetEnvironmentVariable("REPORTS_TABLE_DATE_INDEX");

        public ReportRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<List<Report>> RetrieveReports(DateTime startDate, DateTime endDate)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                IndexName = _dateIndex,
                KeyConditionExpression = "#date BETWEEN :startDate AND :endDate",
                ExpressionAttributeNames = new Dictionary<string, string> { { "#date", "date" } },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":startDate", new AttributeValue { S = startDate.ToString("yyyy-MM-dd") } },
                    { ":endDate", new AttributeValue { S = endDate.ToString("yyyy-MM-dd") } }
                }
            };

            var response = await _dynamoDbClient.QueryAsync(request);

            var reports = Mapper.MapItemsToReports(response.Items);

            return reports;
        }
    }
}
