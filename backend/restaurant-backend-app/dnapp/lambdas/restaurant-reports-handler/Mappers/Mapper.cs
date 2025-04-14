using Amazon.DynamoDBv2.Model;
using Function.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Function.Mappers
{
    public class Mapper
    {
        public static List<Feedback> MapItemsToFeedbacks(List<Dictionary<string, AttributeValue>> items)
        {
            return items.Select(item => new Feedback
            {
                Id = item.TryGetValue("id", out var id) ? id.S : "",
                Rate = item.TryGetValue("rate", out var rate) ? int.Parse(rate.N) : 0,
                Comment = item.TryGetValue("comment", out var comment) ? comment.S : "",
                UserName = item.TryGetValue("userName", out var userName) ? userName.S : "",
                UserAvatarUrl = item.TryGetValue("userAvatarUrl", out var userAvatarUrl) ? userAvatarUrl.S : "",
                Date = item.TryGetValue("date", out var date) ? date.S : "",
                Type = item.TryGetValue("type", out var type) ? type.S : "",
                LocationId = item.TryGetValue("locationId", out var locationId) ? locationId.S : "",
                ReservationId = item.TryGetValue("reservationId", out var reservationId) ? reservationId.S : ""
            }).ToList();
        }
    }
}
