using System.Collections.Generic;
using Function.Models;
using Amazon.DynamoDBv2.DocumentModel;
using System.Linq;


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
}