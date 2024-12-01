using FCMS.Auth;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
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
        [HttpPost("AddChat")]
        public async Task<IActionResult> CreateChat([FromForm] CreateChatRequestModel model)
        {
            //string token = Request.Headers["Authorization"];
            //string extractedToken = token.Substring(7);
            //var isValid = JWTManager.IsTokenValid(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), extractedToken);
            //if (!isValid)
            //{
            //    return Unauthorized();
            //}
            var chat = await _chatService.CreateChat(model);
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
        [HttpGet("GetChats")]
        public async Task<IActionResult> GetChats(string clickedUser, string loggedinUser)
         
      {
            try
            {
                var response = await _chatService.GetChatFromASenderAsync(clickedUser, loggedinUser);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetMyChats")]
        public async Task<IActionResult> MyChats(string myId)
        {
            try
            {
                var myChats = await _chatService.MyChats(myId);
                return Ok(myChats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
