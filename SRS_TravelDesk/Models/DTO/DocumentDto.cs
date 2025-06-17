namespace SRS_TravelDesk.Models.DTO
{
    public class DocumentDto
    {
        public int Id { get; set; }                   
        public string FileName { get; set; }
        public string DocumentType { get; set; }

        public string FileContentBase64 { get; set; }
    }

}
