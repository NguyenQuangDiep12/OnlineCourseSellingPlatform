using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<EnrollmentResponseDto>> EnrollInCourseAsync(int UserId, int CourseId);
        Task<ApiResponse<List<EnrollmentResponseDto>>> GetUserEnrollmentsAsync(int UserId);
        Task<ApiResponse<bool>> UpdateProgressAsync(int EnrollmentId, int Progress);
        Task<bool> IsUserEnrolledAsync(int UserId, int CourseId);
    }
}
