using ChatApp.Entities;
using ServerSide.Security.Models;
using System.Security.Claims;

namespace ServerSide.Security.Core
{
    public interface IUserManager<TUser>
    {
        Task<List<Claim>> GetClaimsAsync(TUser user);
        Task<TUser> FindByNameAsync(string username);
        Task<TUser> FindByLoginAsync(string login);
        Task<CreateUserResult> CreateAsync(TUser user, string password);
    }
}
