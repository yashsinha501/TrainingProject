namespace SRS_TravelDesk.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int TravelRequestId { get; set; }
        public int CommentedByUserId { get; set; } 
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public  User CommentedBy { get; set; }
        public  TravelRequest TravelRequest { get; set; }
    }
}
