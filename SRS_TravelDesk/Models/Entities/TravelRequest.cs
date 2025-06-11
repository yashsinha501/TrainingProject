using System.Reflection.Metadata;

namespace SRS_TravelDesk.Models.Entities
{
    public class TravelRequest
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } // auto-generated
        public int UserId { get; set; }
        public string ProjectName { get; set; }
        public string DepartmentName { get; set; }
        public string ReasonForTravelling { get; set; }
        public BookingType BookingType { get; set; }
        public TravelStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public User User { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

}
