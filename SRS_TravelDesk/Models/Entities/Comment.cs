namespace SRS_TravelDesk.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public int TravelRequestId { get; set; }
        public int CommentedById { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public required User CommentedBy { get; set; }
        public required TravelRequest TravelRequest { get; set; }
    }
}
