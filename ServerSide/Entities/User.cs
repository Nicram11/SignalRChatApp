using System.Reflection.Metadata;

namespace ChatApp.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Login { get; set; }
        public string HPasswd { get; set; }
        public virtual List<Chat> Chats{ get; set; } = new List<Chat>();


    }
}
