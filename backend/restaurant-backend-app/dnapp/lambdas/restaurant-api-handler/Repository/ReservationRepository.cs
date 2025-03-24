using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Function.Mappers;
using Function.Models;
using Function.Repository.Interfaces;

namespace Function.Repository;

public class ReservationRepository : IReservationRepository
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;

    private readonly string? _reservationsTableName =
        Environment.GetEnvironmentVariable("DYNAMODB_RESERVATIONS_TABLE_NAME");

    public ReservationRepository()
    {
        _dynamoDBClient = new AmazonDynamoDBClient();
    }

    public async Task<Reservation> UpsertReservation(Reservation reservation)
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
            UpdateExpression = "SET #date = :date, #feedbackId = :feedbackId, #guestsNumber = :guestsNumber, " +
                               "#locationAddress = :locationAddress, #preOrder = :preOrder, #status = :status, " +
                               "#tableNumber = :tableNumber, #timeFrom = :timeFrom, #timeTo = :timeTo, " +
                               "#timeSlot = :timeSlot, #userInfo = :userInfo",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#date", "date" },
                { "#feedbackId", "feedbackId" },
                { "#guestsNumber", "guestsNumber" },
                { "#locationAddress", "locationAddress" },
                { "#preOrder", "preOrder" },
                { "#status", "status" },
                { "#tableNumber", "tableNumber" },
                { "#timeFrom", "timeFrom" },
                { "#timeTo", "timeTo" },
                { "#timeSlot", "timeSlot" },
                { "#userInfo", "userInfo" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":date", new AttributeValue { S = reservation.Date } },
                { ":feedbackId", new AttributeValue { S = reservation.FeedbackId } },
                { ":guestsNumber", new AttributeValue { S = reservation.GuestsNumber } },
                { ":locationAddress", new AttributeValue { S = reservation.LocationAddress } },
                { ":preOrder", new AttributeValue { S = reservation.PreOrder } },
                { ":status", new AttributeValue { S = reservation.Status } },
                { ":tableNumber", new AttributeValue { S = reservation.TableNumber } },
                { ":timeFrom", new AttributeValue { S = reservation.TimeFrom } },
                { ":timeTo", new AttributeValue { S = reservation.TimeTo } },
                { ":timeSlot", new AttributeValue { S = reservation.TimeSlot } },
                { ":userInfo", new AttributeValue { S = reservation.UserInfo } }
            }
        };

        await _dynamoDBClient.UpdateItemAsync(updateItemRequest);
        return reservation;
    }

    public async Task<List<Reservation>> GetReservationsByDateLocationTable(
        string date,
        string locationAddress,
        string tableNumber
    )
    {
        var request = new ScanRequest
        {
            TableName = _reservationsTableName,
            FilterExpression = "#d = :date AND locationAddress = :locationAddress AND tableNumber = :tableNumber",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#d", "date" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":date", new AttributeValue { S = date } },
                { ":locationAddress", new AttributeValue { S = locationAddress } },
                { ":tableNumber", new AttributeValue { S = tableNumber } }
            }
        };
        var response = await _dynamoDBClient.ScanAsync(request);
        var reservations = Mapper.MapItemsToReservations(response.Items);

        return reservations;
    }

    public async Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationAddress)
    {
        var reservations = new List<ReservationInfo>();
        var table = Table.LoadTable(_dynamoDBClient, _reservationsTableName);
        var scanFilter = new ScanFilter();

        scanFilter.AddCondition("date", ScanOperator.Equal, date);
        scanFilter.AddCondition("locationAddress", ScanOperator.Equal, locationAddress);
        
        var search = table.Scan(scanFilter);
        
        do
        {
            var documents = await search.GetNextSetAsync();
            reservations.AddRange(from document in documents 
                where document["status"].AsString() != "cancelled" 
                select new ReservationInfo
                {
                    TableNumber = document["tableNumber"].AsString(),
                    Date = document["date"].AsString(),
                    TimeFrom = document["timeFrom"].AsString(),
                    TimeTo = document["timeTo"].AsString(),
                    GuestsNumber = document["guestsNumber"].AsString()
                });
        } while (!search.IsDone);

        return reservations;
    }

    public async Task<List<Reservation>> GetCustomerReservationsAsync(string info)
    {
        var request = new ScanRequest
        {
            TableName = _reservationsTableName,
            FilterExpression = "userInfo = :info",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":info", new AttributeValue { S = info } }
            }
        };
        var response = await _dynamoDBClient.ScanAsync(request);

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
}