using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Models
{
    public class Reservation
    {
        public required string Id { get; set; }
        public required string Date { get; set; }
        public string FeedbackId { get; set; }
        public required string GuestsNumber { get; set; }
        public required string LocationId { get; set; }
        public required string LocationAddress { get; set; }
        public required string PreOrder { get; set; }
        public required string Status { get; set; }
        public required string TableId { get; set; }
        public required string TableNumber { get; set; }
        public required string TimeFrom { get; set; }
        public required string TimeTo { get; set; }
        public required string TimeSlot { get; set; }
        public required string UserInfo { get; set; }
        public string? UserEmail { get; set; }
        public string? WaiterId { get; set; }
        public required string CreatedAt { get; set; }
    }
}
