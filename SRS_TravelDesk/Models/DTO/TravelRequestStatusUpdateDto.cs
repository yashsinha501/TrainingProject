using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Models.DTO
{
    public class TravelRequestStatusUpdateDto
    {
        public int RequestId { get; set; }
        public TravelStatus NewStatus { get; set; }
        public string? Comment { get; set; }
        public int UpdatedByUserId { get; set; } // Manager or HR UserId
    }
}
