using AutoMapper;
using ChatApp;
using ChatApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServerSide.Security.Core;
using ServerSide.Security.Models;
using System.Security.Claims;

namespace ServerSide.Security.Infrastructure
{
    public class UserManager : IUserManager<User>
    {
        private readonly ChatAppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly AuthenticationSettings authenticationSettings;

        public UserManager( ChatAppDbContext dbContext,IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.passwordHasher = passwordHasher;
            this.authenticationSettings = authenticationSettings;
        }

        public async Task<CreateUserResult> CreateAsync(User user, string password)
        {
            user.HPasswd = passwordHasher.HashPassword(user, password);
            dbContext.Users.Add(user);
            var result = await dbContext.SaveChangesAsync();

            if (result == 0)
                return new CreateUserResult(CreateUserResult.FAILED);

            return new CreateUserResult(CreateUserResult.SUCCESSED);
        }

        public async Task<User> FindByLoginAsync(string login)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
            
        }

        public async Task<User> FindByNameAsync(string username)
        {

            return await dbContext.Users.FirstOrDefaultAsync(u => u.Name == username);
        }

        public Task<List<Claim>> GetClaimsAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
