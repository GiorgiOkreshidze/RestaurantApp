using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Function.Mappers;
using Function.Models;
using Function.Models.Responses;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    
    private readonly string? _locationFeedbacksTableName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TABLE");
    private readonly string? _locationFeedbacksRatingByTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TYPE_RATING_INDEX");
    private readonly string? _locationFeedbacksDateIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_DATE_INDEX");
    private readonly string? _locationFeedbacksRatingIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_RATING_INDEX");
    private readonly string? _locationFeedbacksDateByTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TYPE_DATE_INDEX");
    
    public FeedbackRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }
    
    public async Task<(List<LocationFeedbackResponse>, string?)> GetLocationFeedbacksAsync(
        LocationFeedbackQueryParameters queryParameters)
    {
        var request = BuildQueryRequest(queryParameters);
        var response = await _dynamoDBClient.QueryAsync(request);
        var nextToken = GetNextToken(response.LastEvaluatedKey);
        var feedbacks = response.Items.Select(Mapper.MapToLocationFeedbackResponse).ToList();

        return (feedbacks, nextToken);
    }

    private QueryRequest BuildQueryRequest(LocationFeedbackQueryParameters queryParameters)
    {
        var request = new QueryRequest
        {
            TableName = _locationFeedbacksTableName,
            Limit = queryParameters.PageSize,
            ScanIndexForward = queryParameters.SortDirection.ToLower() == "asc",
            ExclusiveStartKey = queryParameters.NextPageToken != null
                ? DecodeToken(queryParameters.NextPageToken)
                : null
        };

        if (queryParameters.SortProperty.ToLower() == "date")
        {
            if (queryParameters.Type.HasValue)
            {
                request.IndexName = _locationFeedbacksDateByTypeIndexName;
                request.KeyConditionExpression = "#pk = :locIdType";
                request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":locIdType",
                        new AttributeValue
                            { S = $"{queryParameters.LocationId}#{queryParameters.Type.Value.ToDynamoDBType()}" }
                    }
                };
                request.ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#pk", "locationId#type" }
                };
            }
            else
            {
                request.IndexName = _locationFeedbacksDateIndexName;
                request.KeyConditionExpression = "locationId = :locId";
                request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":locId", new AttributeValue { S = queryParameters.LocationId } }
                };
            }
        }
        else // sortProperty == "rating"
        {
            if (queryParameters.Type.HasValue)
            {
                request.IndexName = _locationFeedbacksRatingByTypeIndexName;
                request.KeyConditionExpression = "#pk = :locIdType";
                request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":locIdType",
                        new AttributeValue
                            { S = $"{queryParameters.LocationId}#{queryParameters.Type.Value.ToDynamoDBType()}" }
                    }
                };
                request.ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#pk", "locationId#type" }
                };
            }
            else
            {
                request.IndexName = _locationFeedbacksRatingIndexName;
                request.KeyConditionExpression = "locationId = :locId";
                request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":locId", new AttributeValue { S = queryParameters.LocationId } }
                };
            }
        }

        return request;
    }

    private string? GetNextToken(Dictionary<string, AttributeValue> lastEvaluatedKey)
    {
        if (lastEvaluatedKey != null && lastEvaluatedKey.Count > 0)
        {
            return EncodeToken(lastEvaluatedKey);
        }
        
        return null;
    }

    // Helper methods for token encoding/decoding
    private string EncodeToken(Dictionary<string, AttributeValue> key)
    {
        var doc = Document.FromAttributeMap(key);
        var json = doc.ToJson();
        
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    private Dictionary<string, AttributeValue>? DecodeToken(string token)
    {
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var doc = Document.FromJson(json);
        
        return doc.ToAttributeMap();
    }
}
