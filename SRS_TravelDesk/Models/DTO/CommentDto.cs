namespace SRS_TravelDesk.Models.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CommentedByUserId { get; set; }
        public string CommentedByName { get; set; }
    }
}
