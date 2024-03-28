using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromRoute] string id)
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
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateUser(UpdateUserRequestModel model)
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
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }
    }
}
