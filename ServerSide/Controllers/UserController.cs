using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using ServerSide.Models;
using ServerSide.Services.Core;

namespace ChatApp.Controllers
{
    [Route("chatApp/user")]
    [ApiController]
    public class UserController : Controller
    {
  
        private IUserService userService;
        private CookieOptions jwtCookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Path = "/"
        };
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (ModelState.IsValid)
            {
                string token = await userService.LoginUser(dto);

                Response.Cookies.Append("JwtToken", token, jwtCookieOptions);
                return Ok(token);
            }
            else return BadRequest(ModelState);
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            userService.RegisterUser(dto);
            return Created($"user", null);
        }
        [Authorize]
        [HttpPost("logout")]
        public ActionResult Logout()
        {
            CookieOptions cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/"
            };
            Response.Cookies.Delete("JwtToken", jwtCookieOptions);
            OnlineUsers.onlineUsersIds.Remove(User.Identity.Name);
            return NoContent();
        }
    }
}
