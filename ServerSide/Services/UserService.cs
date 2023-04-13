using AutoMapper;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerSide.Security.Core;
using ServerSide.Security.Models;
using ServerSide.Services.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Services
{




    public class UserService : IUserService
    {
        private readonly IMapper mapper;
        private readonly ISignInManager<User> signInManager;
        private readonly IUserManager<User> userManager;

        public UserService(ISignInManager<User> signInManager, IUserManager<User> userManager, IMapper mapper)
        {
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task<CreateUserResult> RegisterUser(RegisterUserDTO dto)
        {
            var user = mapper.Map<User>(dto);

            return await userManager.CreateAsync(user, dto.Passwd);
        }

        public async Task<string> LoginUser(LoginDTO dto)
        {
            var token = await signInManager.PasswordSignInAsync(dto.Login, dto.Passwd);
            return token;        
        }

        public static int GetUserIdFromClaimsPrincipal(ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}

