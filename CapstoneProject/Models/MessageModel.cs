namespace CapstoneProject.Models
{
    public class MessageModel
    {
        public int Message_ID { get; set; }
        public int Sending_User { get; set; }
        public int Receiving_User { get; set; }
        public string Content { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string SenderUsername { get; set; } = "";
    }
}
