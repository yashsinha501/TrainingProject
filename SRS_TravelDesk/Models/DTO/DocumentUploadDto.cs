namespace SRS_TravelDesk.Models.DTO
{
    public class DocumentUploadDto
    {
        public string FileName { get; set; }
        public string FileContentBase64 { get; set; } 
        public string DocumentType { get; set; } 
    }
}
