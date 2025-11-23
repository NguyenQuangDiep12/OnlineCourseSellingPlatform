using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;
using System.Security.Claims;
using System.Text;

namespace OnlineCourseSellingPlatform.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        // Get values ​​from a configuration file, environment variable, or other source without reading the file directly.

        public JwtTokenGenerator(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretHere123456789012"));
            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Key"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
