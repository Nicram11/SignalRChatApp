using ChatApp;
using ChatApp.Entities;
using Microsoft.IdentityModel.Tokens;
using ServerSide.Security.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServerSide.Security.Infrastructure
{
    public class JwtGenerator : IJwtTokenGenerator<User>
    {
        private readonly AuthenticationSettings authenticationSettings;

        public JwtGenerator(AuthenticationSettings authenticationSettings)
        {
            this.authenticationSettings = authenticationSettings;
        }
        public string Generate(User user)
        {

            var claims = new List<Claim>()
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.Name),
               new Claim("Login", user.Login),
            };

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
    }
}
