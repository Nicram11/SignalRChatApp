using AutoMapper;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Hubs;
using ChatApp.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServerSide.Entities;
using ServerSide.Models;
using ServerSide.Services.Core;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ChatApp.Services
{
    public class ChatService : IChatService
    {
        public static int BAD_REQUEST = -1;
        public static int SUCCESS = 1;

        private readonly ChatAppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IHubContext<ChatHub> hubContext;

        public ChatService(ChatAppDbContext dbContext, IMapper mapper, IHubContext<ChatHub> hubContext)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.hubContext = hubContext;
        }

        public int GetChatIdOrCreateChat(ClaimsPrincipal sender, string user2Name)
        {
            Chat chat;
            var user1 = dbContext.Users.Include(u => u.ChatUsers).FirstOrDefault(x => x.Id == UserService.GetUserIdFromClaimsPrincipal(sender));
            var user2 = dbContext.Users.Include(u => u.ChatUsers).FirstOrDefault(x => x.Login == user2Name);

            if (user2 is null || user1.Equals(user2))
            {
                return BAD_REQUEST;
            }

            if (dbContext.Chats.Any(c => c.Users.Contains(user1) && c.Users.Contains(user2)))
            {
                chat = dbContext.Chats.FirstOrDefault((c => c.Users.Contains(user1) && c.Users.Contains(user2)));
            }
            else 
            { 
                chat = new Chat();
                chat.ChatUsers = new();
                chat.ChatUser1Id = user1.Id;
                chat.ChatUser2Id = user2.Id;

                ChatUser chatUser1 = new ChatUser { User = user1, Chat = chat , DisplayIndex =  user1.ChatUsers.Count};
                ChatUser chatUser2 = new ChatUser { User = user2, Chat = chat, DisplayIndex = user2.ChatUsers.Count };

                chat.ChatUsers.AddRange(new[]{ chatUser1, chatUser2 });

                dbContext.Chats.Add(chat);
                
                dbContext.SaveChanges();

                hubContext.Clients.User(user1.Id.ToString()).SendAsync("receiveGroupId", chat.Id, user2.Login);
                hubContext.Clients.User(user2.Id.ToString()).SendAsync("receiveGroupId", chat.Id, user1.Login);
            }
            return chat.Id;
        }
        public int CreateMessage(MessageDTO dto, ClaimsPrincipal user)
        {
            if (dto.ChatId == 0)
                return BAD_REQUEST;
            Message message = mapper.Map<Message>(dto);
            var senderId = UserService.GetUserIdFromClaimsPrincipal(user);
            message.SendingTime = DateTime.Now;
            message.SenderId = senderId;
            //   message.SenderName = user.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            message.Received = false;
            /*        message.Sender = dbContext.Users.FirstOrDefault(u => u.Id == 1);
                    message.Chat = dbContext.Chats.FirstOrDefault(c => c.Id == message.ChatId);*/

           // var sender = dbContext.Users.Include(u => u.ChatUsers).FirstOrDefault(u => u.Id == senderId);
            
           /* if(sender == null)
            {
                return BAD_REQUEST;
            }*/

            var chat = dbContext.Chats.Include(c => c.Users).ThenInclude(u => u.ChatUsers).FirstOrDefault(c => c.Id == dto.ChatId);

            foreach(User u in chat.Users)
            {
                var chatUsers = u.ChatUsers.OrderBy(cu => cu.DisplayIndex).ToList();
                var chatUser = chatUsers.FirstOrDefault(cu => cu.ChatId == dto.ChatId);
                var chatUserIndex = chatUsers.IndexOf(chatUser);
                if (chatUserIndex != chatUsers.Count - 1)
                {
                    for (int i = chatUsers.Count - 1; i > chatUserIndex; i--)
                    {
                        chatUsers[i].DisplayIndex = chatUsers[i - 1].DisplayIndex;
                        dbContext.Update(chatUsers[i]);
                    }
                    chatUser.DisplayIndex = chatUsers.Count - 1;
                    dbContext.Update(chatUser);
                }
            }
           
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();

            
            return SUCCESS;
        }
        private void UpdateDisplayIndex(User user, int chatId)
        {
            var chatUsers = user.ChatUsers.OrderBy(cu => cu.DisplayIndex).ToList();
            var chatUser = chatUsers.FirstOrDefault(cu => cu.ChatId == chatId);
            var chatUserIndex = chatUsers.IndexOf(chatUser);
            if (chatUserIndex != chatUsers.Count - 1)
            {
                for (int i = chatUsers.Count - 1; i > chatUserIndex; i--)
                {
                    chatUsers[i].DisplayIndex = chatUsers[i - 1].DisplayIndex;
                    dbContext.Update(chatUsers[i]);
                }
                chatUser.DisplayIndex = chatUsers.Count - 1;
                dbContext.Update(chatUser);
            }
        }


        public IEnumerable<SentMessageDTO> GetAllMessages(int chatId)
        {
            
            //var messages = dbContext.Messages.Include(m => m.Sender).Where(x => x.ChatId == chatId).ToList();
            var messages = dbContext.Chats.Include(c => c.Messages).ThenInclude(m => m.Sender).FirstOrDefault(x => x.Id == chatId).Messages.ToList();
            var messagesDTO = mapper.Map<IEnumerable<SentMessageDTO>>(messages).ToList();
            for(int i = 0; i< messagesDTO.Count(); i++)
            {
                messagesDTO[i].SenderName = messages[i].Sender.Name;

            }
            return messagesDTO;
        }

        public bool IsUserChatMember(ClaimsPrincipal user, int chatId)
        {
            Chat chat = dbContext.Chats.Include(c => c.Users).FirstOrDefault(x => x.Id == chatId);
            int userId = UserService.GetUserIdFromClaimsPrincipal(user);

            if (chat is null)
                return false;
            else if (chat.Users.Any(u => u.Id == userId))
                return true;
            //  else if ((chat.ChatUser1Id == userId) || (chat.ChatUser2Id == userId)) return true;
            return false;
        }

        /// <summary>
        /// Zwraca informacje potrzebne aby zalogować użytkownika
        /// </summary>
        /// <param name="user">Claims Principal odpowiadające danemu użytkownikowi</param>
        /// <returns></returns>
        public OnLoginDataDTO OnLoginData(ClaimsPrincipal user)
        {
           int userId = UserService.GetUserIdFromClaimsPrincipal(user);
               User? user1 = dbContext.Users
                   .Include(x => x.ChatUsers).ThenInclude(cu => cu.Chat.Messages).
                   Include( u => u.ChatUsers).ThenInclude(cu => cu.Chat.Users)
                   .FirstOrDefault(u => u.Id == userId);
        

            // List<Chat> chats = user1.Chats.ToList();
            List<int> ids = new();
            List<string> usernames = new();
            List<bool> hasNewMessage = new();
            //  List<Chat> chats = dbContext.Chats.Include(c => c.ChatUsers).Where(c => c..Contains(user1)).ToList();
            List<ChatUser> chatUsers = user1.ChatUsers.OrderBy(cu => cu.DisplayIndex).ToList();
            List<Chat> chats = new List<Chat>();
            foreach (ChatUser chatUser in chatUsers)
            {
                chats.Add(chatUser.Chat);
            }
            //chats = chats.OrderBy(c => c.ChatUsers)
            foreach (Chat chat in chats)
            {
                ids.Add(chat.Id);
                
                var secondChatUser = chat.Users.FirstOrDefault(c => c.Id != userId);
                if (secondChatUser != null)
                {
                    usernames.Add(secondChatUser.Login);
                }
                Message lastReceivedMessage = chat.Messages.LastOrDefault(m => m.SenderId != userId);
                if (lastReceivedMessage is null)
                    hasNewMessage.Add(false);
                else hasNewMessage.Add(chat.Messages.LastOrDefault(m => m.SenderId != userId).Received ? false : true);
                
            }
      
            OnLoginDataDTO onLoginDataDTO = new OnLoginDataDTO(ids, usernames, hasNewMessage);
            return onLoginDataDTO;
        }
   

        public bool isUserOnline(ClaimsPrincipal user, int chatId)
        {
         //   Chat chat = dbContext.Chats.Include(c => c.ChatUsers).FirstOrDefault(c => c.Id == message.ChatId);
           

            Chat chat = dbContext.Chats.Include(c => c.Users).FirstOrDefault(x => x.Id == chatId);
            int userId = UserService.GetUserIdFromClaimsPrincipal(user);

            User recipient = chat.Users.FirstOrDefault(u => u.Id != userId);
            if (OnlineUsers.onlineUsersIds.Contains(recipient.Id.ToString()))
            {
                return true;
            }
            return false;
        }


        public void ReceiveMessage(int chatId, ClaimsPrincipal user)
        {
            int userId = UserService.GetUserIdFromClaimsPrincipal(user);
            Chat chat = dbContext.Chats.Include(c => c.Messages).FirstOrDefault(c => c.Id == chatId);
            var messages = chat.Messages.Where(m => m.SenderId != userId); //Nie chcemy oflagować wiadomości które sami wysłaliśmy
            foreach (var message in messages)
            {
                message.Received = true;
                dbContext.Entry(message).State = EntityState.Modified;
            }
            dbContext.SaveChanges();
        }
    }
}
