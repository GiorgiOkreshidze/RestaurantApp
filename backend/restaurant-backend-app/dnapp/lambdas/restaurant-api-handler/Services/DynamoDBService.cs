using Amazon.CognitoIdentityProvider.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Function.Models;
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
        private readonly string? _locationsTableName = Environment.GetEnvironmentVariable("DYNAMODB_LOCATIONS_TABLE_NAME");

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

        public async Task<List<Location>> GetListOfLocations()
        {
            var documentList = await ScanDynamoDBTableAsync(_locationsTableName);
            return MapDocumentsToLocations(documentList);
        }

        private List<Location> MapDocumentsToLocations(List<Document> documentList)
        {
            return documentList.Select(doc => new Location
            {
                Id = doc.TryGetValue("id", out var id) ? id : "",
                Address = doc.TryGetValue("address", out var address) ? address : "",
                Description = doc.TryGetValue("description", out var description) ? description : "",
                TotalCapacity = doc.TryGetValue("totalCapacity", out var totalCapacity) ? totalCapacity : "",
                AverageOccupancy = doc.TryGetValue("averageOccupancy", out var averageOccupancy) ? averageOccupancy : "",
                ImageUrl = doc.TryGetValue("imageUrl", out var imageUrl) ? imageUrl : "",
                Rating = doc.TryGetValue("rating", out var rating) ? rating : ""
            }).ToList();
        }

        private async Task<List<Document>> ScanDynamoDBTableAsync(string? tableName)
        {
            var table = Table.LoadTable(_dynamoDBClient, tableName);
            var scanFilter = new ScanFilter();
            var search = table.Scan(scanFilter);

            List<Document> documentList = new List<Document>();
            do
            {
                documentList.AddRange(await search.GetNextSetAsync());
            } while (!search.IsDone);

            return documentList;
        }
    }
}
