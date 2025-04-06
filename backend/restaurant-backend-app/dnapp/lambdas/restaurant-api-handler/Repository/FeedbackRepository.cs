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
    private readonly AmazonDynamoDBClient _dynamoDbClient;
    private readonly string? _locationFeedbacksTableName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TABLE");
    private readonly string? _locationFeedbacksRatingByTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TYPE_RATING_INDEX");
    private readonly string? _locationFeedbacksReservationTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_RESERVATION_TYPE_INDEX");
    private readonly string? _locationFeedbacksDateIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_DATE_INDEX");
    private readonly string? _locationFeedbacksRatingIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_RATING_INDEX");
    private readonly string? _locationFeedbacksDateByTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TYPE_DATE_INDEX");

    public FeedbackRepository()
    {
        _dynamoDbClient = new AmazonDynamoDBClient();
    }
    
    public async Task<(List<LocationFeedback>, string?)> GetLocationFeedbacksAsync(
        LocationFeedbackQueryParameters queryParameters)
    {
        var request = BuildQueryRequest(queryParameters);
        var response = await _dynamoDbClient.QueryAsync(request);
        var nextToken = GetNextToken(response.LastEvaluatedKey);
        var feedbacks = response.Items.Select(Mapper.MapToLocationFeedbackResponse).ToList();

        return (feedbacks, nextToken);
    }
    
    public async Task UpsertFeedbackByReservationAndTypeAsync(LocationFeedback feedback)
    {
        var reservationIdTypeKey = $"{feedback.ReservationId}#{feedback.Type}";
        var queryRequest = new QueryRequest
        {
            TableName = _locationFeedbacksTableName,
            IndexName = _locationFeedbacksReservationTypeIndexName,
            KeyConditionExpression = "#resIdType = :resIdTypeValue",
            ProjectionExpression = "#locId, #typeDate",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#resIdType", "reservationId#type" },
                { "#locId", "locationId" },
                { "#typeDate", "type#date" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":resIdTypeValue", new AttributeValue { S = reservationIdTypeKey } }
            },
            Limit = 1 
        };

        var queryResponse = await _dynamoDbClient.QueryAsync(queryRequest);
        var existingItemKey = queryResponse.Items.FirstOrDefault();

        if (existingItemKey != null && existingItemKey.ContainsKey("locationId") && existingItemKey.ContainsKey("type#date"))
        {
            await UpdateExistingFeedbackRateAndCommentAsync(feedback, existingItemKey);
        }
        else
        {
            await InsertNewFeedbackAsync(feedback);
        }
    }

    private async Task UpdateExistingFeedbackRateAndCommentAsync(LocationFeedback feedback, Dictionary<string, AttributeValue> primaryKey)
    {
        var updateItemRequest = new UpdateItemRequest
        {
            TableName = _locationFeedbacksTableName,
            Key = primaryKey,
            UpdateExpression = "SET #rate = :rate, #comment = :comment",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#rate", "rate" },
                { "#comment", "comment" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":rate", new AttributeValue { N = feedback.Rate } },
                { ":comment", new AttributeValue { S = feedback.Comment } }
            },
            ConditionExpression = "attribute_exists(locationId)",
            ReturnValues = ReturnValue.NONE
        };
        
        await _dynamoDbClient.UpdateItemAsync(updateItemRequest);
    }

    private async Task InsertNewFeedbackAsync(LocationFeedback feedback)
    {
        var typeDateKey = $"{feedback.Type}#{feedback.Date}";
        var locationIdTypeKey = $"{feedback.LocationId}#{feedback.Type}";
        var reservationIdTypeKey = $"{feedback.ReservationId}#{feedback.Type}";
        var feedbackId = feedback.Id;
        var putItemRequest = new PutItemRequest
        {
            TableName = _locationFeedbacksTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "locationId", new AttributeValue { S = feedback.LocationId } },
                { "type#date", new AttributeValue { S = typeDateKey } },
                { "locationId#type", new AttributeValue { S = locationIdTypeKey } },
                { "reservationId#type", new AttributeValue { S = reservationIdTypeKey } },
                { "id", new AttributeValue { S = feedbackId } },
                { "rate", new AttributeValue { N = feedback.Rate } },
                { "comment", new AttributeValue { S = feedback.Comment } },
                { "userName", new AttributeValue { S = feedback.UserName } },
                { "userAvatarUrl", new AttributeValue { S = feedback.UserAvatarUrl } },
                { "date", new AttributeValue { S = feedback.Date } },
                { "type", new AttributeValue { S = feedback.Type } },
                { "reservationId", new AttributeValue { S = feedback.ReservationId } }
            },
            ConditionExpression = "attribute_not_exists(locationId)"
        };
        
        await _dynamoDbClient.PutItemAsync(putItemRequest);
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

    public async Task<int> GetTotalItemCount()
    {
        var request = new ScanRequest
        {
            TableName = _locationFeedbacksTableName,
            Select = "COUNT"
        };

        var response = await _dynamoDbClient.ScanAsync(request);
        return response.Count;
    }
}
