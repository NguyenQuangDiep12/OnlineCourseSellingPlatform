using Microsoft.EntityFrameworkCore;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;
using System.Diagnostics;

namespace OnlineCourseSellingPlatform.Services
{
    public class LessonService : ILessonService
    {
        private readonly ApplicationDbContext _context;
        public LessonService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<LessonResponseDto>> CreateLessonAsync(int CourseId, CreateLessonDto dto, int InstructorId)
        {
            var course = await _context.Courses
                .FindAsync(CourseId);

            if(course == null)
            {
                return new ApiResponse<LessonResponseDto>
                {
                    Success = false,
                    Message = "Course not found"
                };
            }
            
            if(course.InstructorId != InstructorId)
            {
                return new ApiResponse<LessonResponseDto>
                {
                    Success = false,
                    Message = "Unauthorized to add lesson to this course"
                };
            }

            var lesson = new Lesson
            {
                Title = dto.Title,
                Content = dto.Content,
                VideoUrl = dto.VideoUrl,
                OrderIndex = dto.OrderIndex,
                DurationInMinutes = dto.DurationMinutes,
                CourseId = CourseId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return new ApiResponse<LessonResponseDto>
            {
                Success = true,
                Message = "Lesson created successfully",
                Data = await MapToLessonResponseDto(lesson)
            };
        }

        public async Task<ApiResponse<bool>> DeleteLessonAsync(int LessonId, int InstructorId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == LessonId);

            if(lesson == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Lesson not found"
                };
            }

            if(lesson.Course.InstructorId != InstructorId)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized to Delete this lesson"
                };
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Lesson Deleted Successfully",
                Data = true
            };

        }

        public async Task<ApiResponse<List<LessonResponseDto>>> GetCourseLessonsAsync(int CourseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == CourseId)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync();

            if(!lessons.Any())
            {
                return new ApiResponse<List<LessonResponseDto>>
                {
                    Success = false,
                    Message = "No Lessons found for the given course"
                };
            }

            var lessonDtos = new List<LessonResponseDto>();
            foreach (var lesson in lessons)
            {
                lessonDtos.Add(await MapToLessonResponseDto(lesson));
            }

            return new ApiResponse<List<LessonResponseDto>>
            {
                Success = true,
                Data = lessonDtos
            };

        }

        public async Task<ApiResponse<LessonResponseDto>> UpdateLessonAsync(int LessonId, UpdateLessonDto dto, int InstructorId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == LessonId);

            if(lesson == null)
            {
                return new ApiResponse<LessonResponseDto>
                {
                    Success = false,
                    Message = "Lesson not found"
                };
            }

            if(lesson.Course.InstructorId != InstructorId)
            {
                return new ApiResponse<LessonResponseDto>
                {
                    Success = false,
                    Message = "Unauthorized to update this lesson"
                };
            }

            if (dto.Title != null) lesson.Title = dto.Title;
            if (dto.Content != null) lesson.Content = dto.Content;
            if (dto.VideoUrl != null) lesson.VideoUrl = dto.VideoUrl;
            if (dto.OrderIndex.HasValue) lesson.OrderIndex = dto.OrderIndex.Value;
            if (dto.DurationMinutes.HasValue) lesson.DurationInMinutes = dto.DurationMinutes.Value;

            await _context.SaveChangesAsync();

            return new ApiResponse<LessonResponseDto>
            {
                Success = true,
                Message = "Lesson updated successfully",
                Data = await MapToLessonResponseDto(lesson)
            };
            
        }
        private async Task<LessonResponseDto> MapToLessonResponseDto(Lesson lesson)
        {
            return new LessonResponseDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                OrderIndex = lesson.OrderIndex,
                DurationMinutes = lesson.DurationInMinutes
            };
        }
    }
}
