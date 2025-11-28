using Microsoft.EntityFrameworkCore;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;

namespace OnlineCourseSellingPlatform.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        public CourseService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<ApiResponse<CourseResponseDto>> CreateCourseAsync(CreateCourseDto dto, int InstructorId)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                ThumbnailUrl = dto.ThumbnailUrl,
                Level = Enum.Parse<CourseLevel>(dto.Level),
                InstructorId = InstructorId,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return new ApiResponse<CourseResponseDto>
            {
                Success = true,
                Message = "Course created successfully",
                Data = await MapToCourseResponseDto(course)
            };
        }

        public async Task<ApiResponse<CourseResponseDto>> UpdateCourseAsync(int CourseId, UpdateCourseDto dto, int InstructorId)
        {
            var course = _context.Courses.Include(c => c.Instructor).FirstOrDefault(c => c.Id == CourseId);

            if (course == null)
            {
                return new ApiResponse<CourseResponseDto>
                {
                    Success = false,
                    Message = "Course not found",
                };
            }
            if (course.InstructorId != InstructorId)
            {
                return new ApiResponse<CourseResponseDto>
                {
                    Success = false,
                    Message = "You are not authorized to update this course",
                };
            }

            // Update fields if provided
            if (dto.Title != null) course.Title = dto.Title;
            if (dto.Description != null) course.Description = dto.Description;
            if (dto.Price.HasValue) course.Price = dto.Price.Value;
            if (dto.ThumbnailUrl != null) course.ThumbnailUrl = dto.ThumbnailUrl;
            if (dto.Level != null) course.Level = Enum.Parse<CourseLevel>(dto.Level);
            if (dto.IsPublished.HasValue) course.IsPublished = dto.IsPublished.Value;

            course.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ApiResponse<CourseResponseDto>
            {
                Success = true,
                Message = "Course updated successfully",
                Data = await MapToCourseResponseDto(course)
            };
        }

        public async Task<ApiResponse<bool>> DeleteCourseAsync(int CourseId, int InstructorId)
        {
            var course = await _context.Courses.FindAsync(CourseId);

            if (course == null) 
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Course not found",
                    Data = false
                };
            }

            if (course.InstructorId != InstructorId) 
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized to delete this course",
                    Data = false
                };
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Course deleted successfully",
                Data = true
            };
        }

        public async Task<ApiResponse<PaginatedResponse<CourseResponseDto>>> GetCourseAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Where(c => c.IsPublished);

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(c => c.Title.Contains(SearchTerm) || c.Description.Contains(SearchTerm));
            }

            var totalCount = await query.CountAsync();

            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((PageNumber - 1) * PageSize) // 1 PageNumber starts from page index 0 , PageSize is number of items per page example (1 * 10pageSize = 10 items per page)
                .Take(PageSize)
                .ToListAsync();

            var courseDtos = new List<CourseResponseDto>();

            foreach (var course in courses)
            {
                courseDtos.Add(await MapToCourseResponseDto(course));
            }

            return new ApiResponse<PaginatedResponse<CourseResponseDto>>
            {
                Success = true,
                Message = "Courses retrieved successfully",
                Data = new PaginatedResponse<CourseResponseDto>
                {
                    Items = courseDtos,
                    TotalCount = totalCount,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                }
            };
        }

        public async Task<ApiResponse<CourseDetailDto>> GetCourseByIdAsync(int CourseId)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == CourseId);
            if(course == null)
            {
                return new ApiResponse<CourseDetailDto>
                {
                    Success = false,
                    Message = "Course not found",
                };
            }

            var courseDto = new CourseDetailDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                ThumbnailUrl = course.ThumbnailUrl,
                Level = course.Level.ToString(),
                IsPublished = course.IsPublished,
                InstructorName = course.Instructor.FullName,
                LessonCount = course.Lessons.Count,
                CreatedAt = course.CreatedAt,
                Lessons = course.Lessons.Select(lesson => new LessonResponseDto
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    Content = lesson.Content,
                    OrderIndex = lesson.OrderIndex,
                    VideoUrl = lesson.VideoUrl,
                    DurationMinutes = lesson.DurationInMinutes
                }).ToList()
            };
            return new ApiResponse<CourseDetailDto>
            {
                Success = true,
                Message = "Course retrieved successfully",
                Data = courseDto
            };
        }

        public async Task<ApiResponse<List<CourseResponseDto>>> GetCoursesByInstructorAsync(int InstructorId)
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Where(c => c.InstructorId == InstructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var courseDtos = new List<CourseResponseDto>();

            foreach (var course in courses)
            {
                courseDtos.Add(await MapToCourseResponseDto(course));
            }

            return new ApiResponse<List<CourseResponseDto>>
            {
                Success = true,
                Message = "Courses retrieved successfully",
                Data = courseDtos
            };
        }


        private async Task<CourseResponseDto> MapToCourseResponseDto(Course course)
        {
            // If not loaded name of instructor, load it
            if (course.Instructor == null)
            {
                await _context.Entry(course).Reference(e => e.Instructor).LoadAsync();
            }
            // If not loaded lessons or not any lessons, load them
            if (course.Lessons == null || !course.Lessons.Any()) 
            { 
                await _context.Entry(course).Collection(e => e.Lessons).LoadAsync();
            }

            // Return mapped dto
            return new CourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                ThumbnailUrl = course.ThumbnailUrl,
                Level = course.Level.ToString(),
                IsPublished = course.IsPublished,
                InstructorName = course.Instructor?.FullName ?? "",
                LessonCount = course.Lessons?.Count ?? 0,
                CreatedAt = course.CreatedAt
            };
        }
    }
}
