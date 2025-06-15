namespace SRS_TravelDesk.Models.DTO
{
    public class TravelHrBookingDto
    {
        public int TravelRequestId { get; set; }

        public List<DocumentUploadDto> Documents { get; set; } = new();

        public string? Comment { get; set; }
    }
}
