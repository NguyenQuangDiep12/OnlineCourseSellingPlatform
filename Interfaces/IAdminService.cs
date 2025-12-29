using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();
        Task<ApiResponse<List<TopCourseDto>>> GetTopCoursesAsync(int count = 10);
        Task<ApiResponse<List<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 20);
        Task<ApiResponse<List<UserResponseDto>>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<ApiResponse<bool>> ChangeUserRoleAsync(int userId, string newRole);
        Task<ApiResponse<bool>> ToggleCoursePublishStatusAsync(int courseId);
    }
}