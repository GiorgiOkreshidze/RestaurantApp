using System.Collections.Generic;
using Function.Models;
using Amazon.DynamoDBv2.DocumentModel;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Function.Models.Responses;

using Amazon.DynamoDBv2.Model;
using System;
using System.Globalization;

namespace Function.Mappers;

public class Mapper
{
    public static List<Location> MapDocumentsToLocations(List<Document> documentList)
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

    public static List<Dish> MapDocumentsToDishes(List<Document> documentList)
    {
        return documentList.Select(doc => new Dish
        {
            Id = doc.TryGetValue("id", out var id) ? id : "",
            Name = doc.TryGetValue("name", out var name) ? name : "",
            Price = doc.TryGetValue("price", out var price) ? price : "",
            Weight = doc.TryGetValue("weight", out var weight) ? weight : "",
            ImageUrl = doc.TryGetValue("imageUrl", out var imageUrl) ? imageUrl : "",
            IsPopular = doc.TryGetValue("isPopular", out var isPopular) && isPopular.AsBoolean(),
            LocationId = doc.TryGetValue("locationId", out var locationId) ? locationId : "",
        }).ToList();
    }
    
    public static List<Reservation> MapItemsToReservations(List<Dictionary<string, AttributeValue>> items)
    {
        return items.Select(item => new Reservation
        {
            Id = item.TryGetValue("id", out var idValue) ? idValue.S : string.Empty,
            Date = item.TryGetValue("date", out var dateValue) ? dateValue.S : string.Empty,
            GuestsNumber = item.TryGetValue("guestsNumber", out var guestsNumberValue) ? guestsNumberValue.S : string.Empty,
            LocationAddress = item.TryGetValue("locationAddress", out var locationAddressValue) ? locationAddressValue.S : string.Empty,
            TableNumber = item.TryGetValue("tableNumber", out var tableNumberValue) ? tableNumberValue.S : string.Empty,
            TimeFrom = item.TryGetValue("timeFrom", out var timeFromValue) ? timeFromValue.S : string.Empty,
            TimeTo = item.TryGetValue("timeTo", out var timeToValue) ? timeToValue.S : string.Empty,
            TimeSlot = item.TryGetValue("timeSlot", out var timeSlotValue) ? timeSlotValue.S : string.Empty,
            Status = item.TryGetValue("status", out var statusValue) ? statusValue.S : string.Empty,
            FeedbackId = item.TryGetValue("feedbackId", out var feedbackIdValue) ? feedbackIdValue.S : string.Empty,
            PreOrder = item.TryGetValue("preOrder", out var preOrderValue) ? preOrderValue.S : string.Empty,
            UserInfo = item.TryGetValue("userInfo", out var userInfoValue) ? userInfoValue.S : string.Empty
        }).ToList();
    }

    public static List<Reservation> MapDocumentsToReservations(List<Document> documents)
    {
        return documents.Select(doc => new Reservation
        {
            Id = doc.TryGetValue("id", out var id) ? id : string.Empty,
            Date = doc.TryGetValue("date", out var date) ? date : string.Empty,
            GuestsNumber = doc.TryGetValue("guestsNumber", out var guestsNumber) ? guestsNumber : string.Empty,
            LocationAddress = doc.TryGetValue("locationAddress", out var locationAddress) ? locationAddress : string.Empty,
            TableNumber = doc.TryGetValue("tableNumber", out var tableNumber) ? tableNumber : string.Empty,
            TimeFrom = doc.TryGetValue("timeFrom", out var timeFrom) ? timeFrom : string.Empty,
            TimeTo = doc.TryGetValue("timeTo", out var timeTo) ? timeTo : string.Empty,
            TimeSlot = doc.TryGetValue("timeSlot", out var timeSlot) ? timeSlot : string.Empty,
            Status = doc.TryGetValue("status", out var status) ? status : string.Empty,
            FeedbackId = doc.TryGetValue("feedbackId", out var feedbackId) ? feedbackId : string.Empty,
            PreOrder = doc.TryGetValue("preOrder", out var preOrder) ? preOrder : string.Empty,
            UserInfo = doc.TryGetValue("userInfo", out var userInfo) ? userInfo : string.Empty
        }).ToList();
    }

    public static List<ReservationResponse> MapReservationsToReservationResponses(List<Reservation> reservations)
    {
        return reservations.Select(reservation => new ReservationResponse
        {
            Id = reservation.Id,
            Date = reservation.Date,
            GuestsNumber = reservation.GuestsNumber,
            LocationAddress = reservation.LocationAddress,
            TableNumber = reservation.TableNumber,
            TimeSlot = reservation.TimeSlot,
            Status = reservation.Status,
            FeedbackId = reservation.FeedbackId,
            PreOrder = reservation.PreOrder,
            UserInfo = reservation.UserInfo,
            EditableTill = CalculateEditableTill(reservation)
        }).ToList();
    }
    private static string CalculateEditableTill(Reservation reservation)
    {
        var editableTillTime = "";

        if (DateTime.TryParseExact(reservation.TimeFrom, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
        {
            // Subtract 30 minutes
            DateTime newTime = parsedTime.AddMinutes(-30);
            // Return the result in the same "HH:mm" format
            editableTillTime = newTime.ToString("HH:mm");
        }
        
        var editableTill = $"{reservation.Date} {editableTillTime}";

        return editableTill;
    }

    public static LocationFeedbackResponse MapToLocationFeedbackResponse(Dictionary<string, AttributeValue> item)
    {
        var feedback = new LocationFeedbackResponse
        {
            Id = item.TryGetValue("id", out var id) ? id.S : "",
            Rate = item.TryGetValue("rate", out var rate) ? rate.N.ToString() : "",
            Comment = item.TryGetValue("comment", out var comment) ? comment.S : "",
            UserName = item.TryGetValue("userName", out var userName) ? userName.S : "",
            UserAvatarUrl = item.TryGetValue("userAvatarUrl", out var userAvatarUrl) ? userAvatarUrl.S : "",
            Date = item.TryGetValue("date", out var date) ? date.S : "",
            Type = item.TryGetValue("type", out var type) ? type.S : "",
            LocationId = item.TryGetValue("locationId", out var locationId) ? locationId.S : ""
        };
        return feedback;
    }
}