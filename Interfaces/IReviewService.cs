using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface IReviewService
    {
        Task<ApiResponse<ReviewResponseDto>> CreateReviewAsync(int userId, CreateReviewDto dto);
        Task<ApiResponse<ReviewResponseDto>> UpdateReviewAsync(int reviewId, int userId, UpdateReviewDto dto);
        Task<ApiResponse<bool>> DeleteReviewAsync(int reviewId, int userId);
        Task<ApiResponse<List<ReviewResponseDto>>> GetCourseReviewsAsync(int courseId, int pageNumber, int pageSize);
        Task<ApiResponse<ReviewResponseDto>> GetUserReviewForCourseAsync(int userId, int courseId);
        Task<ApiResponse<CourseRatingStatsDto>> GetCourseRatingStatsAsync(int courseId);
        Task<bool> HasUserReviewedCourseAsync(int userId, int courseId);
    }
}