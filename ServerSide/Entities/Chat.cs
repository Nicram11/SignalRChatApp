namespace ChatApp.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public int ChatUser1Id { get; set; }
        public int ChatUser2Id { get; set; }
        public virtual List<Message> Messages { get; set; }
        public List<User> ChatUsers { get; set; } = new List<User>();

    }
}
