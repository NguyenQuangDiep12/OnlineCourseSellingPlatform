using Microsoft.EntityFrameworkCore;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;
using System.Xml;

namespace OnlineCourseSellingPlatform.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public Task<User?> GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponseDto<string>> LoginAsync(LoginDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponseDto<UserResponseDto>> RegisterAsync(RegisterDto dto)
        {
            if(await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return new ApiResponseDto<UserResponseDto>
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                FullName = dto.FullName,
                Role = UserRole.Student,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
