using ChatApp.Entities;

namespace ServerSide.Entities
{
    public class ChatUser
    {
        public int DisplayIndex { get; set; }
        public bool IsVisible { get; set; } = true;
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
