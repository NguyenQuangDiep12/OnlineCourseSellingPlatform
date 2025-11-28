using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface ILessonService
    {
        Task<ApiResponse<LessonResponseDto>> CreateLessonAsync(int CourseId, CreateLessonDto dto, int InstructorId);
        Task<ApiResponse<LessonResponseDto>> UpdateLessonAsync(int LessonId, UpdateLessonDto dto, int InstructorId);
        Task<ApiResponse<bool>> DeleteLessonAsync(int LessonId, int InstructorId);
        Task<ApiResponse<List<LessonResponseDto>>> GetCourseLessonsAsync(int CourseId);
    }
}
