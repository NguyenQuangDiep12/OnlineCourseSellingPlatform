using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface IEnrollmentService
    {
        Task<ApiResponseDto<EnrollmentResponseDto>> EnrollInCourseAsync(int UserId, int CourseId);
        Task<ApiResponseDto<List<EnrollmentResponseDto>>> GetUserEnrollmentsAsync(int UserId);
        Task<ApiResponseDto<bool>> UpdateProgressAsync(int EnrollmentId, int Progress);
        Task<bool> IsUserEnrolledAsync(int UserId, int CourseId);
    }
}
