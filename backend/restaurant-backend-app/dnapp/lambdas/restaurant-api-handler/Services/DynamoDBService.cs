using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Function.Models;
using Function.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Function.Mappers;

namespace Function.Services
{
    public class DynamoDBService : IDynamoDBService
    {
        private readonly IDynamoDBContext _dynamoDBContext;
        private readonly AmazonDynamoDBClient _dynamoDBClient;
        private readonly string? _waitersTableName = Environment.GetEnvironmentVariable("DYNAMODB_WAITERS_TABLE_NAME");
        private readonly string? _emailIndexName = Environment.GetEnvironmentVariable("WAITERS_TABLE_EMAIL_INDEX_NAME");
        private readonly string? _locationsTableName = Environment.GetEnvironmentVariable("DYNAMODB_LOCATIONS_TABLE_NAME");
        private readonly string? _dishesTableName = Environment.GetEnvironmentVariable("DYNAMODB_DISHES_TABLE_NAME");

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
            return Mapper.MapDocumentsToLocations(documentList);
        }

        public async Task<List<Dish>> GetListOfPopularDishes()
        {
            var documentList = await ScanDynamoDBTableAsync(_dishesTableName);
            var filteredDishes = documentList
                .Where(doc =>
                    doc.TryGetValue("isPopular", out var isPopular) && 
                    isPopular.AsBoolean())
                .ToList();
            return Mapper.MapDocumentsToDishes(filteredDishes);
        }
        
        public async Task<List<Dish>> GetListOfSpecialityDishes(string locationId)
        {
            var documentList = await ScanDynamoDBTableAsync(_dishesTableName);
            var filteredDocuments = documentList
                .Where(doc => 
                    doc.TryGetValue("locationId", out var docLocationId) && 
                    docLocationId.ToString() == locationId)
                .ToList();
            
            return Mapper.MapDocumentsToDishes(filteredDocuments);
        }

        public async Task<List<LocationOptions>> GetOptionsOfLocations()
        {
            var request = new ScanRequest
            {
                TableName = _locationsTableName,
                ProjectionExpression = "id, address"
            };
            var locations = await _dynamoDBClient.ScanAsync(request);

            return locations.Items
               .Select(item => new LocationOptions
                {
                   Id = item.ContainsKey("id") ? item["id"].S : "",
                   Address = item.ContainsKey("address") ? item["address"].S : ""
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
