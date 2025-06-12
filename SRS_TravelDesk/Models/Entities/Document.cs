using System.Xml.Linq;

namespace SRS_TravelDesk.Models.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public int TravelRequestId { get; set; }
        public string FileName { get; set; }

        public byte[] FileContent { get; set; } 
        public DocumentType DocumentType { get; set; }

        public TravelRequest TravelRequest { get; set; }
    }

}
