using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Models.DTO
{
    public class TravelRequestDetailDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }

        public string RequestNumber { get; set; }
        public string ProjectName { get; set; }
        public string DepartmentName { get; set; }
        public string ReasonForTravelling { get; set; }

        public BookingType BookingType { get; set; }
        public TravelStatus Status { get; set; }

        public DateTime TravelDate { get; set; }
        public string AadharCardNumber { get; set; }
        public string? PassportNumber { get; set; }

        public int? DaysOfStay { get; set; }
        public string? MealRequired { get; set; }      // Lunch, Dinner, Both
        public string? MealPreference { get; set; }    // Veg / Non-Veg

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<CommentDto> Comments { get; set; }
        public List<DocumentDto> Documents { get; set; }
    }
}
