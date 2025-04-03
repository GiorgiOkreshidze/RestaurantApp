using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Function.Models
{
    public class Reservation
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("date")]
        public required string Date { get; set; }
        
        [JsonPropertyName("feedbackId")]
        public string FeedbackId { get; set; }
        
        [JsonPropertyName("guestsNumber")]
        public required string GuestsNumber { get; set; }
        
        [JsonPropertyName("locationId")]
        public required string LocationId { get; set; }
        
        [JsonPropertyName("locationAddress")]
        public required string LocationAddress { get; set; }
        
        [JsonPropertyName("preOrder")]
        public required string PreOrder { get; set; }
        
        [JsonPropertyName("status")]
        public required string Status { get; set; }
        
        [JsonPropertyName("tableId")]
        public required string TableId { get; set; }
        
        [JsonPropertyName("tableNumber")]
        public required string TableNumber { get; set; }
        
        [JsonPropertyName("timeFrom")]
        public required string TimeFrom { get; set; }
        
        [JsonPropertyName("timeTo")]
        public required string TimeTo { get; set; }
        
        [JsonPropertyName("timeSlot")]
        public required string TimeSlot { get; set; }
        
        [JsonPropertyName("userInfo")]
        public required string UserInfo { get; set; }
        
        [JsonPropertyName("userEmail")]
        public string? UserEmail { get; set; }
        
        [JsonPropertyName("waiterId")]
        public string? WaiterId { get; set; }
        
        [JsonPropertyName("createdAt")]
        public required string CreatedAt { get; set; }
    }
}
