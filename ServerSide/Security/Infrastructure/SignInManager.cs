using ChatApp.Entities;
using ChatApp.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServerSide.Security.Core;

namespace ServerSide.Security.Infrastructure
{
    public class SignInManager : ISignInManager<User>
    {
        private readonly ChatAppDbContext dbContext;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IJwtTokenGenerator<User> jwtTokenGenerator;

        public SignInManager(ChatAppDbContext dbContext, IPasswordHasher<User> passwordHasher, IJwtTokenGenerator<User> jwtTokenGenerator)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }
        public Task<string> PasswordSignInAsync(string username, string password)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Login == username);

            if (user is null)
            {
                throw new BadRequestException("Invaild username or password");
            }
            var result = passwordHasher.VerifyHashedPassword(user, user.HPasswd, password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invaild username or password");
            }
            return Task.FromResult(jwtTokenGenerator.Generate(user));
        }
    }
}
