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
    public class FeedbacksRepository : IFeedbacksRepository
    {
        public readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string? _locationFeedbacksTableName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TABLE");
        private readonly string? _locationFeedbacksRatingByTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TYPE_RATING_INDEX");
        private readonly string? _locationFeedbacksReservationTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_RESERVATION_TYPE_INDEX");
        private readonly string? _locationFeedbacksDateIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_DATE_INDEX");
        private readonly string? _locationFeedbacksRatingIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_RATING_INDEX");
        private readonly string? _locationFeedbacksDateByTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TYPE_DATE_INDEX");

        public FeedbacksRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<List<Feedback>> GetServiceFeedbacks(string reservationId)
        {
            var request = new QueryRequest
            {
                TableName = _locationFeedbacksTableName,
                IndexName = _locationFeedbacksReservationTypeIndexName,
                KeyConditionExpression = "#reservationKey = :reservationKeyValue",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#reservationKey", "reservationId#type" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":reservationKeyValue", new AttributeValue { S = $"{reservationId}#SERVICE_QUALITY" } }
                },
            };

            var response = await _dynamoDbClient.QueryAsync(request);

            return Mapper.MapItemsToFeedbacks(response.Items);
        }
    }
}
