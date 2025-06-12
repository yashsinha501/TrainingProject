namespace SRS_TravelDesk.Models.Entities
{
    public class TravelRequest
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public int UserId { get; set; }

        public string ProjectName { get; set; }
        public string DepartmentName { get; set; }
        public string ReasonForTravelling { get; set; }

        public BookingType BookingType { get; set; }
        public TravelStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        
        public DateTime TravelDate { get; set; }
        public string AadharCardNumber { get; set; }
        public string? PassportNumber { get; set; }
        public int? DaysOfStay { get; set; }
        public string? MealRequired { get; set; }
        public string? MealPreference { get; set; }

        public required User RequestedBy { get; set; }

        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
