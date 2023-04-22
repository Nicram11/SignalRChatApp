﻿using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ServerSide.Services.Core;
using System.Security.Claims;

namespace ChatApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/chatapp/chat")]
    public class ChatController : Controller
    {
        private readonly IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpPost("message")]
        public ActionResult SendMessage([FromBody] MessageDTO message)
        {
            if(ModelState.IsValid)
            {
                int result = chatService.CreateMessage(message, User);

                if (result == ChatService.BAD_REQUEST)
                    return BadRequest("Chat does not exist");

                return Created($"chatapp/chat/message", null);
                
            }
            return BadRequest();
        }

        [HttpGet("message/{username}")]
        public ActionResult GetAllMessagesFromChat([FromRoute] String username)
        {

            var chatId = chatService.GetChatIdOrCreateChat(User, username);
            if(chatId == -1)
            {
                return BadRequest("User not found, please check the name");
            }    
            var messages = chatService.GetAllMessages(chatId);
            chatService.ReceiveMessage(chatId, User);
            return Ok(new { chatId,  messages});

        }

        [HttpGet("onLogin")]
        public ActionResult OnLogin()
        {
            var result = chatService.OnLoginData(User);
            return Ok(result);

        }
      
    }
}
