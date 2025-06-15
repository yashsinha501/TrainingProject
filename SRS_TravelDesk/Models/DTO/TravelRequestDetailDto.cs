using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Models.DTO
{
    public class TravelRequestDetailDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
   
        public string EmployeeName { get; set; }
        public string Department { get; set; }        // from user
        public string ProjectName { get; set; }       // from travel request

        public string ReasonForTravelling { get; set; }
        public string BookingType { get; set; }
        public string Status { get; set; }
       
        public string AadharCardNumber { get; set; }
        public string? PassportNumber { get; set; }
        public int? DaysOfStay { get; set; }
        public string? MealRequired { get; set; }
        public string? MealPreference { get; set; }

        public string TravelDate { get; set; }
        public string CreatedDate { get; set; }
        public string? UpdatedDate { get; set; }

        public List<CommentDto> Comments { get; set; }
        public List<DocumentDto> Documents { get; set; }
    }
}
