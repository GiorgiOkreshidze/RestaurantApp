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
using System.Text.Json;
using System.Text;
using Function.Models.Responses;

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
        private readonly string? _reservationTableName = Environment.GetEnvironmentVariable("DYNAMODB_RESERVATIONS_TABLE_NAME");
        private readonly string? _locationFeedbacksTableName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TABLE");
        private readonly string? _locationFeedbacksRatingByTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TYPE_RATING_INDEX");
        private readonly string? _locationFeedbacksDateIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_DATE_INDEX");
        private readonly string? _locationFeedbacksRatingIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_RATING_INDEX");
        private readonly string? _locationFeedbacksDateByTypeIndexName = Environment.GetEnvironmentVariable("DDB_LOCATION_FEEDBACKS_TYPE_DATE_INDEX");
        private readonly string? _tablesTableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLES_TABLE_NAME");
        private readonly string? _reservationsTableName = Environment.GetEnvironmentVariable("DYNAMODB_RESERVATIONS_TABLE_NAME");

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

        public async Task<Reservation> UpsertReservation(Reservation reservation)
        {
            try
            {
                var updateItemRequest = new UpdateItemRequest
                {
                    TableName = _reservationTableName,
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Location> GetLocationById(string locationId)
        {
            var documentList = await ScanDynamoDBTableAsync(_locationsTableName);
            var locations = Mapper.MapDocumentsToLocations(documentList);
            var result = locations.FirstOrDefault(loc => loc.Id == locationId);
            
            if (result == null)
            {
                throw new ArgumentException($"The location with {locationId} id is not found");
            }

            return result;
        }

        public async Task<List<LocationOptions>> GetLocationDropdownOptions()
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

        public async Task<List<Reservation>> GetReservationsByDateLocationTable(
            string date,
            string locationAddress,
            string tableNumber
        )
        {
            var request = new ScanRequest
            {
                TableName = _reservationTableName,
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

            List<Reservation> reservations = Mapper.MapItemsToReservations(response.Items);

            return reservations;
        }


        public async Task<(List<LocationFeedbackResponse>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters queryParameters)
        {
            var request = BuildQueryRequest(queryParameters);

            var response = await _dynamoDBClient.QueryAsync(request);

            var nextToken = GetNextToken(response.LastEvaluatedKey);
            
            var feedbacks = response.Items.Select(item => Mapper.MapToLocationFeedbackResponse(item)).ToList();

            return (feedbacks, nextToken);
        }

        private QueryRequest BuildQueryRequest(LocationFeedbackQueryParameters queryParameters)
        {
            var request = new QueryRequest
            {
                TableName = _locationFeedbacksTableName,
                Limit = queryParameters.PageSize,
                ScanIndexForward = queryParameters.SortDirection.ToLower() == "asc",
                ExclusiveStartKey = queryParameters.NextPageToken != null ? DecodeToken(queryParameters.NextPageToken) : null
            };

            if (queryParameters.SortProperty.ToLower() == "date")
            {
                if (queryParameters.Type.HasValue)
                {
                    request.IndexName = _locationFeedbacksDateByTypeIndexName;
                    request.KeyConditionExpression = "#pk = :locIdType";
                    request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":locIdType", new AttributeValue { S = $"{queryParameters.LocationId}#{queryParameters.Type.Value.ToDynamoDBType()}" } }
                    };
                    request.ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {"#pk", "locationId#type"}
                    };
                }
                else
                {
                    request.IndexName = _locationFeedbacksDateIndexName;
                    request.KeyConditionExpression = "locationId = :locId";
                    request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":locId", new AttributeValue { S = queryParameters.LocationId }}
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
                        { ":locIdType", new AttributeValue { S = $"{queryParameters.LocationId}#{queryParameters.Type.Value.ToDynamoDBType()}" } }
                    };
                    request.ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {"#pk", "locationId#type"}
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

        public async Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationAddress)
        {
            var table = Table.LoadTable(_dynamoDBClient, _reservationsTableName);

            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("date", ScanOperator.Equal, date);

            scanFilter.AddCondition("locationAddress", ScanOperator.Equal, locationAddress);
            var search = table.Scan(scanFilter);
            var reservations = new List<ReservationInfo>();

            do
            {
                var documents = await search.GetNextSetAsync();
                foreach (var document in documents)
                {
                    if (document["status"].AsString() != "cancelled")
                    {
                        reservations.Add(new ReservationInfo
                        {
                            TableNumber = document["tableNumber"].AsString(),
                            Date = document["date"].AsString(),
                            TimeFrom = document["timeFrom"].AsString(),
                            TimeTo = document["timeTo"].AsString(),
                            GuestsNumber = document["guestsNumber"].AsString()
                        });
                    }
                }
            } while (!search.IsDone);

            return reservations;
        }

        public async Task<List<RestaurantTable>> GetTablesForLocation(string locationId, int guests)
        {
            var table = Table.LoadTable(_dynamoDBClient, _tablesTableName);
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("locationId", ScanOperator.Equal, locationId);
            scanFilter.AddCondition("capacity", ScanOperator.GreaterThanOrEqual, guests);
            var search = table.Scan(scanFilter);
            var tables = new List<RestaurantTable>();

            do
            {
                var documents = await search.GetNextSetAsync();
                foreach (var document in documents)
                {
                    tables.Add(new RestaurantTable
                    {
                        Id = document["id"].AsString(),
                        TableNumber = document["tableNumber"].AsString(),
                        Capacity = document["capacity"].AsInt().ToString(),
                        LocationId = document["locationId"].AsString(),
                        LocationAddress = document["locationAddress"].AsString(),
                    });
                }
            } while (!search.IsDone) ;

            return tables;
        }
        public async Task<LocationInfo?> GetLocationDetails(string locationId)
        {
            try
            {
                // Create table reference
                var table = Table.LoadTable(_dynamoDBClient, _locationsTableName);

                // Perform get operation
                var document = await table.GetItemAsync(locationId);
                if (document == null)
                    return null;

                return new LocationInfo
                {
                    LocationId = locationId,
                    Address = document["address"].AsString(),
                    TotalCapacity = document["totalCapacity"].AsString(),
                    AverageOccupancy = document["averageOccupancy"].AsString()
                };
            }
            catch
            {
                return null;
            }
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