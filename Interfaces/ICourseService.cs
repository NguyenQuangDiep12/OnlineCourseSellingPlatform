using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface ICourseService
    {
        Task<ApiResponse<CourseResponseDto>> CreateCourseAsync(CreateCourseDto dto, int InstructorId);
        Task<ApiResponse<CourseResponseDto>> UpdateCourseAsync(int CourseId, UpdateCourseDto dto, int InstructorId);
        Task<ApiResponse<bool>> DeleteCourseAsync(int CourseId, int InstructorId);
        Task<ApiResponse<CourseDetailDto>> GetCourseByIdAsync(int CourseId);
        Task<ApiResponse<PaginatedResponse<CourseResponseDto>>> GetCourseAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<ApiResponse<List<CourseResponseDto>>> GetCoursesByInstructorAsync(int InstructorId);
    }
}
