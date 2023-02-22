using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/chatapp/chat")]
    public class ChatController : Controller
    {
        private readonly ChatService chatService;

        public ChatController(ChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpPost("newchat")]
        public ActionResult CreateNewChat([FromBody]string userNameToConnect)
        {

            var chatId = chatService.CreateChat(User.FindFirst(c=>c.Type == ClaimTypes.NameIdentifier).Value, userNameToConnect);
            if (chatId == -1)
            {
              
                return BadRequest("User not found, please check the name");
            }
            //   return Created($"chatapp/chat/{chatId}", null);
            return Ok(chatId);

        }
        [HttpPost("message")]
        public ActionResult SendMessage([FromBody] MessageDTO message)
        {
            if(ModelState.IsValid)
            {
                chatService.CreateMessage(message, User);
                return Created($"chatapp/chat/message", null);
                
            }
            return BadRequest();
        }

        [HttpGet("message/{username}")]
        public ActionResult GetAllMessagesFromChat([FromRoute] String username)
        {

            var chatId = chatService.CreateChat(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, username);
            if(chatId == -1)
            {
                return BadRequest("User not found, please check the name");
            }    
            var messages = chatService.GetAllMessages(chatId);

            return Ok(new { chatId,  messages});
          
        }


        [HttpGet("chatIds")]
        public ActionResult GetAllUserChatIds()
        {
            List<int> ids = chatService.GetAllUserChatIds(User);
            return Ok(ids);

        }

    }
}
