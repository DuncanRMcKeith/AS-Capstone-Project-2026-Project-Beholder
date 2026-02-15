namespace CapstoneProject.Models
{
    public class PostsModel 
    {
        public int Post_ID { get; set; }
        public string? Title { get; set; }
        public string? Post_text { get; set; }
        public int Creator_ID { get; set; }
        public int? Comm_ID { get; set; }
        public DateTime Created_At { get; set; }
    }
}
