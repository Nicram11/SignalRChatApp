using ChatApp.Entities;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServerSide.Models;
using ServerSide.Services.Core;
using System.Security.Claims;

namespace ChatApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private IChatService _chatService;
      


        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }
        //   public async Task SendMessage(MessageDTO message, string username /* ,string connId */) => await Clients.Users(_chatService.GetUserIdFromUsername(username)).SendAsync("receiveMessage", message);
        public async Task SendMessageToGroup(SentMessageDTO message, int roomId)
        {
            

            if (_chatService.IsUserChatMember(Context.User, roomId)) 
                await Clients.Group(roomId.ToString()).SendAsync("receiveMessage", message);
            else throw new UnauthorizedAccessException("You cannot perform this Action!");
        }
        public Task JoinRoom(int roomId)
        {
            if (_chatService.IsUserChatMember(Context.User,roomId))
                return Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            else throw new UnauthorizedAccessException("You cannot perform this Action!");



        }

      
        public Task LeaveRoom(int roomId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            if (!OnlineUsers.onlineUsersIds.Contains(username))
            OnlineUsers.onlineUsersIds.Add(username);

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {

            OnlineUsers.onlineUsersIds.Remove(Context.User.Identity.Name);

            return base.OnDisconnectedAsync(exception);
        }
     
    }

}
