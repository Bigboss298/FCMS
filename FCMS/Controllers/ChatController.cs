using FCMS.Auth;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IConfiguration _config;
        private readonly IJWTManager _tokenService;

        public ChatController(IChatService chatService, IConfiguration config, IJWTManager tokenService)
        {
            _chatService = chatService; 
            _config = config;
            _tokenService = tokenService;
        }
        [HttpPost("CreateChat/{id}/{recieverId}")]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequestModel model, [FromRoute] string id, [FromRoute] string recieverId)
        {
            string token = Request.Headers["Authorization"];
            string extractedToken = token.Substring(7);
            var isValid = JWTManager.IsTokenValid(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), extractedToken);
            if (!isValid)
            {
                return Unauthorized();
            }
            var chat = await _chatService.CreateChat(model, id, recieverId);
            if (!chat.Status)
            {
                return BadRequest(chat);
            }
            return Ok(chat);
        }
        [HttpPut("MarkAllAsRead/{senderId}/{recieverId}")]
        public async Task<IActionResult> MarkAllAsRead([FromRoute] string senderId, [FromRoute] string recieverId)
        {
            var response = await _chatService.MarkAllChatsAsReadAsync(senderId, recieverId);
            if (!response.Status)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("GetAllUnSeenChatAsync/{recieverId}/{senderId}")]
        public async Task<IActionResult> GetAllUnseenChats([FromRoute] string recieverId, string senderId)
        {
            var response = await _chatService.GetChatFromASenderAsync(recieverId, senderId);
            if (!response.Status)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
