using AutoMapper;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Hubs;
using ChatApp.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServerSide.Models;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ChatApp.Services
{
    public class ChatService
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

        //temp
        public IEnumerable<Chat> GetAllChats()
        {
            var chats = dbContext.Chats.ToList();
            return chats;

        }

        public Chat GetFromId(int id)
        {
            var chat = dbContext.Chats.FirstOrDefault(r => r.Id == id);

            return chat;
        }



        public int CreateChat(string user1Id, string user2Name)
        {



            var user1 = dbContext.Users.FirstOrDefault(x => x.Id == int.Parse(user1Id));

            var user2 = dbContext.Users.FirstOrDefault(x => x.Login == user2Name);

            if (user2 is null)
            {
                return -1;

            }
            Chat chat;
            if (dbContext.Chats.Any(c => c.ChatUsers.Contains(user1) && c.ChatUsers.Contains(user2)))
            {
                chat = dbContext.Chats.FirstOrDefault((c => c.ChatUsers.Contains(user1) && c.ChatUsers.Contains(user2)));
            }
            /* if (dbContext.Chats.Any(x => x.ChatUser2Id == user2.Id && x.ChatUser1Id == int.Parse(user1Id)))
             {
                 chat = dbContext.Chats.FirstOrDefault(x => x.ChatUser2Id == user2.Id && x.ChatUser1Id == int.Parse(user1Id));
             }
             else if (dbContext.Chats.Any(x => x.ChatUser1Id == user2.Id && x.ChatUser2Id == int.Parse(user1Id)))
             {
                 chat = dbContext.Chats.FirstOrDefault(x => x.ChatUser1Id == user2.Id && x.ChatUser2Id == int.Parse(user1Id));
             }*/
            else
            {
                chat = new Chat();
                chat.ChatUser1Id = user1.Id;
                chat.ChatUser2Id = user2.Id;
                chat.ChatUsers.Add(user1);
                chat.ChatUsers.Add(user2);


                dbContext.Entry(user1).State = EntityState.Modified;
                dbContext.Entry(user2).State = EntityState.Modified;
                user1.Chats.Add(chat);
                user2.Chats.Add(chat);
                dbContext.Chats.Add(chat);
                /* dbContext.Update(user1);
                 dbContext.Update(user2);*/
                dbContext.SaveChanges();

                chat = user1.Chats.FirstOrDefault(c => c.ChatUsers.Contains(user2));
                hubContext.Clients.User(user1.Id.ToString()).SendAsync("receiveGroupId", chat.Id);
                hubContext.Clients.User(user2.Id.ToString()).SendAsync("receiveGroupId", chat.Id);
            }

            return chat.Id;
        }
        public IEnumerable<Message> GetAllMess()
        {
            var messages = dbContext.Messages.ToList();
            return messages;

        }
        public int CreateMessage(MessageDTO dto, ClaimsPrincipal user)
        {

            if (dto.ChatId == 0)
                return BAD_REQUEST;
            Message message = mapper.Map<Message>(dto);

           

            message.SendingTime = DateTime.Now;
            message.SenderId = int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //   message.SenderName = user.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            message.Received = false;
    /*        message.Sender = dbContext.Users.FirstOrDefault(u => u.Id == 1);
            message.Chat = dbContext.Chats.FirstOrDefault(c => c.Id == message.ChatId);*/
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();
            return SUCCESS;
        }

        public Message GetMessage(int idMess)
        {
            var message = dbContext.Messages.FirstOrDefault(x => x.Id == idMess);
            return message;

        }

        public IEnumerable<SentMessageDTO> GetAllMessages(int chatId)
        {
            var messages = dbContext.Messages.Include(m => m.Sender).Where(x => x.ChatId == chatId).ToList();
            //  var messages = dbContext.Chats.FirstOrDefault(x => x.Id == chatId).Messages.ToList();
            var messagesDTO = mapper.Map<IEnumerable<SentMessageDTO>>(messages).ToList();
           for(int i = 0; i< messagesDTO.Count(); i++)
            {
                messagesDTO[i].SenderName = messages[i].Sender.Name;

            }
            return messagesDTO;
        }

        public string GetUserIdFromUsername(string username)
        {
            return (dbContext.Users.FirstOrDefault(x => x.Login == username).Id).ToString();
        }

        public bool isUserChatMember(ClaimsPrincipal user, int chatId)
        {
            Chat chat = dbContext.Chats.FirstOrDefault(x => x.Id == chatId);
            int userId = int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            if (chat == null)
                return false;
            else if ((chat.ChatUser1Id == userId) || (chat.ChatUser2Id == userId)) return true;

            return false;

        }

        public Object[] GetAllUserChatIds(ClaimsPrincipal user)
        {
            int userId = int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            User user1 = dbContext.Users.Include(x => x.Chats).ThenInclude(x => x.Messages).FirstOrDefault(u => u.Id == userId);
            List<Chat> chats = user1.Chats.ToList();
            List<int> ids = new();
            List<string> usernames = new();
            List<bool> hasNewMessage = new();
            chats = dbContext.Chats.Include(x => x.ChatUsers).Where(c => c.ChatUsers.Contains(user1)).ToList();
            foreach (Chat chat in chats)
            {
                ids.Add(chat.Id);
                var secondChatUser = chat.ChatUsers.FirstOrDefault(c => c.Login != user1.Login);

                if (secondChatUser != null)
                {
                    usernames.Add(secondChatUser.Login);
                }

                Message lastReceivedMessage = chat.Messages.LastOrDefault(m => m.SenderId != userId);
                if (lastReceivedMessage is null)
                    hasNewMessage.Add(false);
                else hasNewMessage.Add(chat.Messages.LastOrDefault(m => m.SenderId != userId).Received ? false : true);
                
            }
          
            Object[] result = new Object[3];
            result[0] = ids;
            result[1] = usernames;
            result[2] = hasNewMessage;
            return result;
        }
        public IEnumerable<Chat> GetAllUserChats(ClaimsPrincipal user)
        {
            int userId = int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            User user1 = dbContext.Users.Include(x => x.Chats).FirstOrDefault(u => u.Id == userId);
            var chats = user1.Chats.ToList();
            List<int> ids = new();
            foreach (Chat chat in chats)
            {
                ids.Add(chat.Id);
            }

            return chats;
        }

        public bool isUserOnline(ClaimsPrincipal user, int chatId)
        {
         //   Chat chat = dbContext.Chats.Include(c => c.ChatUsers).FirstOrDefault(c => c.Id == message.ChatId);
           

            Chat chat = dbContext.Chats.Include(c => c.ChatUsers).FirstOrDefault(x => x.Id == chatId);
            int userId = int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            User recipient = chat.ChatUsers.FirstOrDefault(u => u.Id != userId);
            if (OnlineUsers.onlineUsersIds.Contains(recipient.Id.ToString()))
            {
                return true;
            }
            return false;
        }


        public void ReceiveMessage(int chatId, ClaimsPrincipal user)
        {

            int userId = int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
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
