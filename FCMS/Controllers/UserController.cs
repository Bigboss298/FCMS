using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _config;

        public UserController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> Login([FromForm] UserLoginRequestModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var userToLogin = await _userService.LoginAsync(model);
                    return Ok(userToLogin.Data);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            return BadRequest("Invalid Login Parameters");
        }

        //[Authorize]
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var userToDelete = await _userService.DeleteAsync(id);
                return Ok("User Deleted Sucessfully!!!");
            }
            catch (NotFoundException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[Authorize]
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequestModel model)
        {
           if(ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _userService.UpdateUser(model);
                    return Ok(userToUpdate.Data);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            return BadRequest("Invalid parameters");
        }

        //[Authorize]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }
    }
}
