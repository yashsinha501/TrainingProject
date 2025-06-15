using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Models.DTO
{
    public class TravelRequestResponseDto
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string ProjectName { get; set; }
        public string DepartmentName { get; set; }
        public string ReasonForTravelling { get; set; }
        public BookingType BookingType { get; set; }
        public TravelStatus Status { get; set; }
        public String CreatedDate { get; set; }
        public List<CommentDto> Comments { get; set; }
        public List<DocumentDto> Documents { get; set; }
    }

   
}
