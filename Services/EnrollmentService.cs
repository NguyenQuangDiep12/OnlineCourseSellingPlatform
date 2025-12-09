using Microsoft.EntityFrameworkCore;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;

namespace OnlineCourseSellingPlatform.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }   

        public async Task<ApiResponse<EnrollmentResponseDto>> EnrollInCourseAsync(int UserId, int CourseId)
        {
            var course = await _context.Courses.FindAsync(CourseId);

            if(course == null)
            {
                return new ApiResponse<EnrollmentResponseDto>
                {
                    Success = false,
                    Message = "Course not found"
                };
            }

            if (!course.IsPublished)
            {
                return new ApiResponse<EnrollmentResponseDto>
                {
                    Success = false,
                    Message = "Course is not published"
                };
            }

            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == UserId && e.CourseId == CourseId);

            if (existingEnrollment != null)
            {
                return new ApiResponse<EnrollmentResponseDto>
                {
                    Success = false,
                    Message = "Already enrolled in this course"
                };
            }

            var enrollment = new Enrollment
            {
                UserId = UserId,
                CourseId = CourseId,
                EnrolledAt = DateTime.UtcNow,
                PricePaid = course.Price,
                Status = EnrollmentStatus.InProgress,
                Progress = 0
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            await _context.Entry(enrollment).Reference(e => e.Course).LoadAsync();

            return new ApiResponse<EnrollmentResponseDto>
            {
                Success = true,
                Message = "Enrolled successfully",
                Data = MapToEnrollmentResponseDto(enrollment)
            };
        }

        public async Task<ApiResponse<List<EnrollmentResponseDto>>> GetUserEnrollmentsAsync(int UserId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == UserId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            return new ApiResponse<List<EnrollmentResponseDto>>
            {
                Success = true,
                Data = enrollments.Select(MapToEnrollmentResponseDto).ToList()
            };
        }

        public async Task<bool> IsUserEnrolledAsync(int UserId, int CourseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.UserId == UserId &&  e.CourseId == CourseId);
        }

        public async Task<ApiResponse<bool>> UpdateProgressAsync(int EnrollmentId, int Progress)
        {
            var enrollment = await _context.Enrollments.FindAsync(EnrollmentId);
            if(enrollment == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Enrollment not found"
                };
            }

            enrollment.Progress = Math.Clamp(Progress, 0, 100);
            if(enrollment.Progress == 100)
            {
                enrollment.Status = EnrollmentStatus.Completed;
            }

            await _context.SaveChangesAsync();
            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Progress updated successfully",
                Data = true,
            };
        }

        private EnrollmentResponseDto MapToEnrollmentResponseDto(Enrollment enrollment)
        {
            return new EnrollmentResponseDto
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                CourseTitle = enrollment.Course.Title ?? "",
                EnrolledAt = enrollment.EnrolledAt,
                Progress = enrollment.Progress,
                Status = enrollment.Status.ToString()
            };
        }
    }
}
