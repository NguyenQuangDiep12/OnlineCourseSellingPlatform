using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface ILessonService
    {
        Task<ApiResponseDto<LessonResponseDto>> CreateLessonAsync(int CourseId, CreateLessonDto dto, int InstructorId);
        Task<ApiResponseDto<LessonResponseDto>> UpdateLessonAsync(int LessonId, UpdateLessonDto dto, int InstructorId);
        Task<ApiResponseDto<bool>> DeleteLessonAsync(int LessonId, int InstructorId);
        Task<ApiResponseDto<LessonResponseDto>> GetCourseLessonsAsync(int CourseId);
    }
}
