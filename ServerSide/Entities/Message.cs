namespace ChatApp.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string MessageValue{ get; set; }
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public DateTime SendingTime { get; set; }
        public bool Received { get; set; } = false;
        public virtual Chat Chat { get; set; }
        public virtual User Sender { get; set; }

    }
}