using System.Collections.Generic;
using Function.Models;
using Amazon.DynamoDBv2.DocumentModel;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Function.Models.Responses;
using System;
using System.Globalization;
using Function.Models.Dishes;
using Function.Models.Reservations;
using Function.Models.User;

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

    public static List<ExactDishResponse> MapDocumentsToExactDishResponseDtos(List<Document> documentList)
    {
        var result = documentList.Select(doc => new ExactDishResponse
        {
            Id = doc.TryGetValue("id", out var id) ? id : "",
            ImageUrl = doc.TryGetValue("imageUrl", out var imageUrl) ? imageUrl : "",
            Name = doc.TryGetValue("name", out var name) ? name : "",
            Price = doc.TryGetValue("price", out var price) ? $"${price}" : "",
            Weight = doc.TryGetValue("weight", out var weight) ? weight : "",
            DishType = doc.TryGetValue("dishType", out var dishType) ? dishType : "",
            State = doc.TryGetValue("state", out var state) ? state : "",
            Description = doc.TryGetValue("description", out var description) ? description : "",
            Calories = doc.TryGetValue("calories", out var calories) ? calories : "",
            Carbohydrates = doc.TryGetValue("carbohydrates", out var carbohydrates) ? carbohydrates : "",
            Fats = doc.TryGetValue("fats", out var fats) ? fats : "",
            Proteins = doc.TryGetValue("proteins", out var proteins) ? proteins : "",
            Vitamins = doc.TryGetValue("vitamins", out var vitamins) ? vitamins : ""
        }).ToList();

        return result;
    }

    public static List<AllDishResponse> MapDocumentsToDishesResponseDtos(List<Document> documentList)
    {
        return documentList.Select(doc => new AllDishResponse
        {
            Id = doc.TryGetValue("id", out var id) ? id : "",
            PreviewImageUrl = doc.TryGetValue("imageUrl", out var imageUrl) ? imageUrl : "",
            Name = doc.TryGetValue("name", out var name) ? name : "",
            Price = doc.TryGetValue("price", out var price) ? price : "",
            Weight = doc.TryGetValue("weight", out var weight) ? weight : "",
            DishType = doc.TryGetValue("dishType", out var dishType) ? dishType : "",
            State = doc.TryGetValue("state", out var state) ? state : "",
            IsPopular = doc.TryGetValue("isPopular", out var isPopular) && isPopular.AsBoolean()
        }).ToList();
    }

    public static List<Reservation> MapItemsToReservations(List<Dictionary<string, AttributeValue>> items)
    {
        return items.Select(item => new Reservation
        {
            Id = item.TryGetValue("id", out var idValue) ? idValue.S : string.Empty,
            Date = item.TryGetValue("date", out var dateValue) ? dateValue.S : string.Empty,
            GuestsNumber = item.TryGetValue("guestsNumber", out var guestsNumberValue)
                ? guestsNumberValue.S
                : string.Empty,
            LocationAddress = item.TryGetValue("locationAddress", out var locationAddressValue)
                ? locationAddressValue.S
                : string.Empty,
            LocationId = item.TryGetValue("locationId", out var locationIdValue) ? locationIdValue.S : string.Empty,
            TableId = item.TryGetValue("tableId", out var tableIdValue) ? tableIdValue.S : string.Empty,
            TableNumber = item.TryGetValue("tableNumber", out var tableNumberValue) ? tableNumberValue.S : string.Empty,
            TimeFrom = item.TryGetValue("timeFrom", out var timeFromValue) ? timeFromValue.S : string.Empty,
            TimeTo = item.TryGetValue("timeTo", out var timeToValue) ? timeToValue.S : string.Empty,
            TimeSlot = item.TryGetValue("timeSlot", out var timeSlotValue) ? timeSlotValue.S : string.Empty,
            Status = item.TryGetValue("status", out var statusValue) ? statusValue.S : string.Empty,
            PreOrder = item.TryGetValue("preOrder", out var preOrderValue) ? preOrderValue.S : string.Empty,
            UserInfo = item.TryGetValue("userInfo", out var userInfoValue) ? userInfoValue.S : string.Empty,
            CreatedAt = item.TryGetValue("createdAt", out var createAtValue) ? createAtValue.S : string.Empty,
            UserEmail = item.TryGetValue("userEmail", out var userEmailValue) ? userEmailValue.S : string.Empty,
            TableCapacity = item.TryGetValue("tableCapacity", out var tableCapacityValue)
                ? tableCapacityValue.S
                : string.Empty,
        }).ToList();
    }

    public static List<Reservation> MapDocumentsToReservations(List<Document> documents)
    {
        return documents.Select(doc => new Reservation
        {
            Id = doc.TryGetValue("id", out var id) ? id : string.Empty,
            Date = doc.TryGetValue("date", out var date) ? date : string.Empty,
            GuestsNumber = doc.TryGetValue("guestsNumber", out var guestsNumber) ? guestsNumber : string.Empty,
            LocationAddress = doc.TryGetValue("locationAddress", out var locationAddress)
                ? locationAddress
                : string.Empty,
            LocationId = doc.TryGetValue("locationId", out var locationId) ? locationId : string.Empty,
            TableId = doc.TryGetValue("tableId", out var tableId) ? tableId : string.Empty,
            TableNumber = doc.TryGetValue("tableNumber", out var tableNumber) ? tableNumber : string.Empty,
            TimeFrom = doc.TryGetValue("timeFrom", out var timeFrom) ? timeFrom : string.Empty,
            TimeTo = doc.TryGetValue("timeTo", out var timeTo) ? timeTo : string.Empty,
            TimeSlot = doc.TryGetValue("timeSlot", out var timeSlot) ? timeSlot : string.Empty,
            Status = doc.TryGetValue("status", out var status) ? status : string.Empty,
            PreOrder = doc.TryGetValue("preOrder", out var preOrder) ? preOrder : string.Empty,
            UserInfo = doc.TryGetValue("userInfo", out var userInfo) ? userInfo : string.Empty,
            CreatedAt = doc.TryGetValue("createdAt", out var createAtValue) ? createAtValue : string.Empty,
            WaiterId = doc.TryGetValue("waiterId", out var waiterId) ? waiterId : string.Empty,
            UserEmail = doc.TryGetValue("userEmail", out var userEmail) ? userEmail : string.Empty,
            ClientType = doc.TryGetValue("clientType", out var clientType) && 
                         Enum.TryParse<ClientType>(clientType, out var parsedClientType) 
                ? parsedClientType 
                : ClientType.VISITOR,
            TableCapacity = doc.TryGetValue("tableCapacity", out var tableCapacity) ? tableCapacity : string.Empty,
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
            LocationId = reservation.LocationId,
            TableId = reservation.TableId,
            TableNumber = reservation.TableNumber,
            TableCapacity = reservation.TableCapacity,
            TimeSlot = reservation.TimeSlot,
            TimeFrom = reservation.TimeFrom,
            Status = reservation.Status,
            PreOrder = reservation.PreOrder,
            UserInfo = reservation.UserInfo,
            EditableTill = CalculateEditableTill(reservation)
        }).ToList();
    }

    private static string CalculateEditableTill(Reservation reservation)
    {
        var editableTillTime = "";

        if (DateTime.TryParseExact(reservation.TimeFrom, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime parsedTime))
        {
            // Subtract 30 minutes
            DateTime newTime = parsedTime.AddMinutes(-30);
            // Return the result in the same "HH:mm" format
            editableTillTime = newTime.ToString("HH:mm");
        }

        var editableTill = $"{reservation.Date} {editableTillTime}";

        return editableTill;
    }

    public static LocationFeedback MapToLocationFeedbackResponse(Dictionary<string, AttributeValue> item)
    {
        var feedback = new LocationFeedback
        {
            Id = item.TryGetValue("id", out var id) ? id.S : "",
            Rate = item.TryGetValue("rate", out var rate) ? rate.N.ToString() : "",
            Comment = item.TryGetValue("comment", out var comment) ? comment.S : "",
            UserName = item.TryGetValue("userName", out var userName) ? userName.S : "",
            UserAvatarUrl = item.TryGetValue("userAvatarUrl", out var userAvatarUrl) ? userAvatarUrl.S : "",
            Date = item.TryGetValue("date", out var date) ? date.S : "",
            Type = item.TryGetValue("type", out var type) ? type.S : "",
            LocationId = item.TryGetValue("locationId", out var locationId) ? locationId.S : "",
            ReservationId = item.TryGetValue("reservationId", out var reservationId) ? reservationId.S : ""
        };
        return feedback;
    }

    public static List<User> MapDocumentsToUsers(List<Document> documentList)
    {
        return documentList.Select(doc =>
        {
            doc.TryGetValue("id", out var id);
            doc.TryGetValue("firstName", out var firstName);
            doc.TryGetValue("lastName", out var lastName);
            doc.TryGetValue("email", out var email);
            doc.TryGetValue("role", out var roleStr);
            doc.TryGetValue("locationId", out var locationId);
            doc.TryGetValue("createdAt", out var createdAt);
            doc.TryGetValue("imageUrl", out var imageUrl);

            Enum.TryParse<Roles>(roleStr, out var parsedRole);

            return new User
            {
                Id = id ?? "",
                FirstName = firstName ?? "",
                LastName = lastName ?? "",
                Email = email ?? "",
                Role = parsedRole,
                LocationId = locationId ?? "",
                CreatedAt = createdAt ?? "",
                ImageUrl = imageUrl ?? ""
            };
        }).ToList();
    }
}