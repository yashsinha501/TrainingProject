namespace SRS_TravelDesk.Models.DTO
{
    public class DocumentUploadDto
    {
        public string FileName { get; set; }
        public string FileContentBase64 { get; set; } // ✅ still coming from frontend
        public string DocumentType { get; set; } // To be parsed to enum
    }
}
