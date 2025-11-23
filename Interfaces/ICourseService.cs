using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface ICourseService
    {
        Task<ApiResponseDto<CourseResponseDto>> CreateCourseAsync(CreateCourseDto dto, int InstructorId);
        Task<ApiResponseDto<CourseResponseDto>> UpdateCourseAsync(int CourseId, UpdateCourseDto dto, int InstructorId);
        Task<ApiResponseDto<bool>> DeleteCourseAsync(int CourseId, int InstructorId);
        Task<ApiResponseDto<CourseDetailDto>> GetCourseByIdAsync(int CourseId);
        Task<ApiResponseDto<PaginatedResponse<CourseResponseDto>>> GetCourseAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<ApiResponseDto<List<CourseResponseDto>>> GetCoursesByInstructorAsync(int InstructorId);
    }
}
