using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Function.Mappers;
using Function.Models;
using Function.Models.Reservations;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class ReservationRepository : IReservationRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;

    private readonly string? _reservationsTableName =
        Environment.GetEnvironmentVariable("DYNAMODB_RESERVATIONS_TABLE_NAME");
    private readonly string? _reservationsTableLocationIndexName =
        Environment.GetEnvironmentVariable("DYNAMODB_RESERVATIONS_TABLE_LOCATION_INDEX");
    private readonly string? _reservationsTableUserEmailIndex = Environment.GetEnvironmentVariable("DYNAMODB_RESERVATIONS_TABLE_USER_EMAIL_INDEX");
    private readonly string? _reservationsTableWaiterIdIndex = Environment.GetEnvironmentVariable("DYNAMODB_RESERVATIONS_TABLE_WAITER_ID_INDEX");

    public ReservationRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }

    public async Task<Reservation> UpsertReservationAsync(Reservation reservation)
    {
        var updateItemRequest = new UpdateItemRequest
        {
            TableName = _reservationsTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {
                    "id", new AttributeValue { S = reservation.Id }
                }
            },
            UpdateExpression = "SET #createdAt = :createdAt, #date = :date, #feedbackId = :feedbackId, #guestsNumber = :guestsNumber, " +
                               "#locationAddress = :locationAddress, #locationId = :locationId, #preOrder = :preOrder, #status = :status, " +
                               "#tableNumber = :tableNumber, #tableId = :tableId, #timeFrom = :timeFrom, #timeTo = :timeTo, " +
                               "#timeSlot = :timeSlot, #userInfo = :userInfo, #userEmail = :userEmail, #waiterId = :waiterId," +
                               "#clientType = :clientType",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#createdAt", "createdAt" },
                { "#date", "date" },
                { "#feedbackId", "feedbackId" },
                { "#guestsNumber", "guestsNumber" },
                { "#locationAddress", "locationAddress" },
                { "#locationId", "locationId" },
                { "#preOrder", "preOrder" },
                { "#status", "status" },
                { "#tableNumber", "tableNumber" },
                { "#tableId", "tableId" },
                { "#timeFrom", "timeFrom" },
                { "#timeTo", "timeTo" },
                { "#timeSlot", "timeSlot" },
                { "#userInfo", "userInfo" },
                { "#waiterId", "waiterId" },
                { "#userEmail", "userEmail" },
                { "#clientType", "clientType"},
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":createdAt", new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") } },
                { ":date", new AttributeValue { S = reservation.Date } },
                { ":feedbackId", new AttributeValue { S = reservation.FeedbackId } },
                { ":guestsNumber", new AttributeValue { S = reservation.GuestsNumber } },
                { ":locationAddress", new AttributeValue { S = reservation.LocationAddress } },
                { ":locationId", new AttributeValue { S = reservation.LocationId } },
                { ":preOrder", new AttributeValue { S = reservation.PreOrder } },
                { ":status", new AttributeValue { S = reservation.Status } },
                { ":tableNumber", new AttributeValue { S = reservation.TableNumber } },
                { ":tableId", new AttributeValue { S = reservation.TableId } },
                { ":timeFrom", new AttributeValue { S = reservation.TimeFrom } },
                { ":timeTo", new AttributeValue { S = reservation.TimeTo } },
                { ":timeSlot", new AttributeValue { S = reservation.TimeSlot } },
                { ":userInfo", new AttributeValue { S = reservation.UserInfo } },
                { ":waiterId", new AttributeValue { S = reservation.WaiterId } },
                { ":userEmail", new AttributeValue  { S = reservation.UserEmail } },
                { ":clientType", new AttributeValue  { S = reservation.ClientType.ToString() } },
            }
        };

        await _dynamoDBClient.UpdateItemAsync(updateItemRequest);
        return reservation;
    }

    public async Task<List<Reservation>> GetReservationsByDateLocationTable(
        string date,
        string locationAddress,
        string tableId
    )
    {
        var request = new ScanRequest
        {
            TableName = _reservationsTableName,
            FilterExpression = "#d = :date AND locationAddress = :locationAddress AND tableId = :tableId",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#d", "date" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":date", new AttributeValue { S = date } },
                { ":locationAddress", new AttributeValue { S = locationAddress } },
                { ":tableId", new AttributeValue { S = tableId } }
            }
        };
        var response = await _dynamoDBClient.ScanAsync(request);
        var reservations = Mapper.MapItemsToReservations(response.Items);

        return reservations;
    }

    public async Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationId)
    {
        var reservations = new List<ReservationInfo>();
        var queryRequest = new QueryRequest
        {
            TableName = _reservationsTableName,
            IndexName = _reservationsTableLocationIndexName,
            KeyConditionExpression = "locationId = :locationId AND #date = :date",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#date", "date" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":locationId", new AttributeValue { S = locationId } },
            { ":date", new AttributeValue { S = date } }
        }
        };

        var response = await _dynamoDBClient.QueryAsync(queryRequest);

        reservations.AddRange(from document in response.Items
                              where document["status"].S != "cancelled"
                              select new ReservationInfo
                              {
                                  TableId = document["tableId"].S,
                                  TableNumber = document["tableNumber"].S,
                                  Date = document["date"].S,
                                  TimeFrom = document["timeFrom"].S,
                                  TimeTo = document["timeTo"].S,
                                  GuestsNumber = document["guestsNumber"].S
                              });

        return reservations;
    }

    public async Task<List<Reservation>> GetCustomerReservationsAsync(string email)
    {
        var queryRequest = new QueryRequest
        {
            TableName = _reservationsTableName,
            IndexName = _reservationsTableUserEmailIndex,
            KeyConditionExpression = "userEmail = :userEmail",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":userEmail", new AttributeValue { S = email } },
            }
        };

        var response = await _dynamoDBClient.QueryAsync(queryRequest);

        return Mapper.MapItemsToReservations(response.Items);
    }

    public async Task<List<Reservation>> GetWaiterReservationsAsync(ReservationsQueryParameters queryParams, string waiterId)
    {
        var keyConditionExpression = "waiterId = :waiterId";
        var expressionAttributeValues = new Dictionary<string, AttributeValue>
    {
        { ":waiterId", new AttributeValue { S = waiterId } }
    };
        var expressionAttributeNames = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(queryParams.Date))
        {
            keyConditionExpression += " AND #dt = :date";
            expressionAttributeValues.Add(":date", new AttributeValue { S = queryParams.Date });
            expressionAttributeNames.Add("#dt", "date");
        }

        var filterConditions = new List<string>();

        if (!string.IsNullOrEmpty(queryParams.TimeFrom))
        {
            filterConditions.Add("timeFrom = :timeFrom");
            expressionAttributeValues.Add(":timeFrom", new AttributeValue { S = queryParams.TimeFrom });
        }

        if (!string.IsNullOrEmpty(queryParams.TableId))
        {
            filterConditions.Add("tableId = :tableId");
            expressionAttributeValues.Add(":tableId", new AttributeValue { S = queryParams.TableId });
        }

        var queryRequest = new QueryRequest
        {
            TableName = _reservationsTableName,
            IndexName = _reservationsTableWaiterIdIndex,
            KeyConditionExpression = keyConditionExpression,
            ExpressionAttributeValues = expressionAttributeValues,
            ExpressionAttributeNames = expressionAttributeNames
        };

        if (filterConditions.Count > 0)
        {
            queryRequest.FilterExpression = string.Join(" AND ", filterConditions);
        }

        var response = await _dynamoDBClient.QueryAsync(queryRequest);
        return Mapper.MapItemsToReservations(response.Items);
    }

    public async Task CancelReservation(string reservationId)
    {
        var request = new UpdateItemRequest
        {
            TableName = _reservationsTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = reservationId } }
            },
            UpdateExpression = "SET #status = :status",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#status", "status" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":status", new AttributeValue { S = "Cancelled" } }
            },
            ConditionExpression = "attribute_exists(id)"
        };
        
        try
        {
            await _dynamoDBClient.UpdateItemAsync(request);
        }
        catch (ResourceNotFoundException)
        {
            throw new Exceptions.ResourceNotFoundException("Reservation not found");
        }
    }

    public async Task<int> GetWaiterReservationCountAsync(string waiterId, string date)
    {
        if (string.IsNullOrEmpty(waiterId))
            throw new ArgumentException("Waiter ID cannot be null or empty");
        
        var request = new ScanRequest
        {
            TableName = _reservationsTableName,
            FilterExpression = "waiterId = :waiterId AND #date = :date",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#date", "date" } // "Date" is a reserved keyword in DynamoDB
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":waiterId", new AttributeValue { S = waiterId } },
                { ":date", new AttributeValue { S = date } }
            }
        };

        var response = await _dynamoDBClient.ScanAsync(request);
        return response.Items.Count;
    }

    public async Task<bool> ReservationExistsAsync(string reservationId)
    {
        if (string.IsNullOrEmpty(reservationId))
            return false;

        var request = new GetItemRequest
        {
            TableName = _reservationsTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = reservationId } }
            },
            ProjectionExpression = "id" // We only need to check existence
        };

        var response = await _dynamoDBClient.GetItemAsync(request);
        return response.Item != null && response.Item.Count > 0;
    }

    public async Task<Reservation> GetReservationByIdAsync(string reservationId)
    {
        var documentList = await DynamoDbUtils.ScanDynamoDbTableAsync(_dynamoDBClient, _reservationsTableName);
        var reservations = Mapper.MapDocumentsToReservations(documentList);
        var result = reservations.FirstOrDefault(loc => loc.Id == reservationId);

        if (result == null) throw new ResourceNotFoundException($"The location with {reservationId} id is not found");

        return result;
    }
}