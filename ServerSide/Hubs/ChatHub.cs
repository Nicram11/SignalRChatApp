using ChatApp.Entities;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServerSide.Models;
using System.Security.Claims;

namespace ChatApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private ChatService _chatService;
      

      //  public List<string> onlineUsersIds = new List<string>();
        public ChatHub( ChatService chatService)
        {
            _chatService = chatService;
        }
        //   public async Task SendMessage(MessageDTO message, string username /* ,string connId */) => await Clients.Users(_chatService.GetUserIdFromUsername(username)).SendAsync("receiveMessage", message);
        public async Task SendMessageToGroup(SentMessageDTO message, int roomId)
        {
            

            if (_chatService.isUserChatMember(Context.User, roomId))  //Sprawdzanie czy użytkownik jest członkiem chatu
                await Clients.Group(roomId.ToString()).SendAsync("receiveMessage", message);
            else throw new UnauthorizedAccessException("You cannot perform this Action!");
        }
        public Task JoinRoom(int roomId)
        {
            if (_chatService.isUserChatMember(Context.User,roomId))  //Sprawdzanie czy użytkownik rzeczywiście jest członkiem chatu do którego próbuje się dostać
                return Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            else throw new UnauthorizedAccessException("You cannot perform this Action!");


          //  Clients.Group(roomId).SendMessage( username, username + " Online");
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
        /*
        //BaD NAME!!!!!
        public Task SendGroupId(int userId,int groupId, string username)
        {
            
            return Clients.User(userId.ToString()).SendAsync("receiveGroupId", groupId, username);
            
        }
        */
     
    }

}
