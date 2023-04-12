using ChatApp.Entities;
using ChatApp.Models;

namespace ServerSide.Services.Core
{
    public interface IUserService
    {
        public void RegisterUser(RegisterUserDTO dto);
        public IEnumerable<RegisterUserDTO> GetAll();
        public string LoginUser(LoginDTO dto);
        public string GenerateJwt(User user);
    }

}
