namespace ServerSide.Models
{
    public class OnLoginDataDTO
    {
        public List<int> ids { get; set; }
        public List<string> usernames { get; set; }
        public List<bool> hasNewMessageList { get; set; }

        public OnLoginDataDTO(List<int> ids, List<string> usernames, List<bool> hasNewMessageList)
        {
            this.ids = ids;
            this.usernames = usernames;
            this.hasNewMessageList = hasNewMessageList;
        }
    }
}
