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

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task<ApiResponse<UserResponseDto>> RegisterAsync(RegisterDto dto)
        {
            if(await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return new ApiResponse<UserResponseDto>
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

            _context.Users.Add(user); // Mark the user entity for addition
            await _context.SaveChangesAsync(); // Persist changes to the database

            return new ApiResponse<UserResponseDto>
            {
                Success = true,
                Message = "User registered Successfully",
                Data = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToString()
                }
            };
        }
        public async Task<ApiResponse<string>> LoginAsync(LoginDto dto)
        {
            var user = await GetUserByEmail(dto.Email);
            if(user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Login Successful",
                Data = token
            };
        }
    }
}
