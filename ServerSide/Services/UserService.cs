using AutoMapper;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Services
{

    public interface IUserService
    {
        public void RegisterUser(RegisterUserDTO dto);
        public IEnumerable<RegisterUserDTO> GetAll();
        public string LoginUser(LoginDTO dto);
        public string GenerateJwt(User user);
    }



    public class UserService : IUserService
    {

        private readonly ChatAppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly AuthenticationSettings authenticationSettings;


        public UserService(ChatAppDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.passwordHasher = passwordHasher;
            this.authenticationSettings = authenticationSettings;
        }

        public void RegisterUser(RegisterUserDTO dto)
        {
            var user = mapper.Map<User>(dto);
            user.HPasswd = passwordHasher.HashPassword(user, dto.Passwd);
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        public IEnumerable<RegisterUserDTO> GetAll()
        {
            var users = dbContext.Users.ToList();
            return mapper.Map<List<RegisterUserDTO>>(users);
        }

        public string LoginUser(LoginDTO dto)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Login == dto.Login);

            if (user is null)
            {
                throw new BadRequestException("Invaild username or password");
            }
            var result = passwordHasher.VerifyHashedPassword(user, user.HPasswd, dto.Passwd);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invaild username or password");
            }
            return GenerateJwt(user);
        }

        public string GenerateJwt(User user)
        {

            var claims = new List<Claim>()
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.Name),
               new Claim("Login", user.Login),
            };
            /*if (!string.IsNullOrEmpty(user.Nationality))
            {
                claims.Add(new Claim("Nationality", user.Nationality));
            }
            if (user.DateOfBirth.HasValue)
            {
                claims.Add(new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
            }*/

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(authenticationSettings.JwtIssuer,
                authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }


        public static int GetUserIdFromClaimsPrincipal(ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}

