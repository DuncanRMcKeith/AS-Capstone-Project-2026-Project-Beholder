namespace CapstoneProject.Models
{
    public class CommentModel
    {
        public string? Profilepic { get; set; }
        public int Comment_ID { get; set; }
        public int Post_ID { get; set; }
        public int User_ID { get; set; }
        public string? Username { get; set; }
        public string? Content { get; set; }
        public DateTime Created_At { get; set; }
    }
}
