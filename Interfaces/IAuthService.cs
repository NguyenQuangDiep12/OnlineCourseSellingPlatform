using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Models;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDto<UserResponseDto>> RegisterAsync(RegisterDto dto);
        Task<ApiResponseDto<string>> LoginAsync(LoginDto dto);
        Task<User?> GetUserByEmail(string email);
    }
}
