namespace InsureYou_AI.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Detail { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
