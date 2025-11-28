using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Models;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<UserResponseDto>> RegisterAsync(RegisterDto dto);
        Task<ApiResponse<string>> LoginAsync(LoginDto dto);
        Task<User?> GetUserByEmail(string email);
    }
}
