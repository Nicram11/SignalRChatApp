using ChatApp.Entities;
using ChatApp.Models;
using ServerSide.Security.Models;

namespace ServerSide.Services.Core
{
    public interface IUserService
    {
        public Task<CreateUserResult> RegisterUser(RegisterUserDTO dto);
        public Task<string> LoginUser(LoginDTO dto);
    }

}
