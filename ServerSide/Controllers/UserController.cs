using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using ServerSide.Models;

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

        [HttpGet]
        public ActionResult<IEnumerable<RegisterUserDTO>> GetAll()
        {
            var users = userService.GetAll();
            return Ok(users); ;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDTO dto)
        {
            if (ModelState.IsValid)
            {
                string token = userService.LoginUser(dto);

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
           /* foreach (var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }*/
            CookieOptions cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/"
            };
            Response.Cookies.Delete("JwtToken", jwtCookieOptions);
            OnlineUsers.onlineUsersIds.Remove(User.Identity.Name);
            //HttpContext.Response.Cookies.Append("JwtToken", " ",cookieOptions );
            //HttpContext.Response.Cookies.Append("username", "");
            return NoContent();
        }
    }
}
