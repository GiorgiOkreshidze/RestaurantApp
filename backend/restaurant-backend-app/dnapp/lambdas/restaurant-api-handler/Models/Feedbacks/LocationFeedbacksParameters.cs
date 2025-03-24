namespace Function.Models.Feedbacks
{
    class LocationFeedbacksParameters
    {
        public required string LocationId { get; set; }
        public LocationFeedbackType? Type { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? NextPageToken { get; set; }
    }
}
