namespace ChatApp.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string MessageValue{ get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public DateTime SendingTime { get; set; }
        public string SenderName { get; set; }
        //public virtual User User { get; set; }

    }
}