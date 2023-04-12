using ChatApp.Entities;
using ChatApp.Models;
using ServerSide.Models;
using System.Security.Claims;

namespace ServerSide.Services.Core
{
    public interface IChatService
    {

        public int GetChatIdOrCreateChat(ClaimsPrincipal sender, string user2Name);
        public int CreateMessage(MessageDTO dto, ClaimsPrincipal user);
        public IEnumerable<SentMessageDTO> GetAllMessages(int chatId);
        public bool IsUserChatMember(ClaimsPrincipal user, int chatId);
        public OnLoginDataDTO OnLoginData(ClaimsPrincipal user);
        public void ReceiveMessage(int chatId, ClaimsPrincipal user);
    }
}
