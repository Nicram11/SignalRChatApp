namespace ChatApp.Models
{
    public class SentMessageDTO
    {
        public string MessageValue { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public bool received { get; set; }
        public string SenderName { get; set; }
        public DateTime SendingTime { get; set; }
    }
}
