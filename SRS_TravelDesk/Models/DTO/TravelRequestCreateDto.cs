using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Models.DTO
{
    public class TravelRequestCreateDto
    {
        public int UserId { get; set; }
        public string ProjectName { get; set; }
        public string DepartmentName { get; set; }
        public string ReasonForTravelling { get; set; }

        public BookingType BookingType { get; set; }

        public DateTime TravelDate { get; set; }
        public string AadharCardNumber { get; set; }
        public string? PassportNumber { get; set; }

        public int? DaysOfStay { get; set; }
        public string? MealRequired { get; set; }      // Lunch, Dinner, Both
        public string? MealPreference { get; set; }    // Veg / Non-Veg

        public List<DocumentUploadDto> Documents { get; set; } = new();
    }


    
}
