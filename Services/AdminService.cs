using Microsoft.EntityFrameworkCore;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;

namespace OnlineCourseSellingPlatform.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalInstructors = await _context.Users.CountAsync(u => u.Role == UserRole.Instructor);
            var totalStudents = await _context.Users.CountAsync(u => u.Role == UserRole.Student);
            var totalCourses = await _context.Courses.CountAsync();
            var publishedCourses = await _context.Courses.CountAsync(c => c.IsPublished);
            var totalEnrollments = await _context.Enrollments.CountAsync();
            var totalRevenue = await _context.Enrollments.SumAsync(e => e.PricePaid);
            var totalReviews = await _context.Reviews.CountAsync();
            var averageRating = await _context.Reviews.AnyAsync()
                ? Math.Round(await _context.Reviews.AverageAsync(r => r.Rating), 2)
                : 0;

            var stats = new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalInstructors = totalInstructors,
                TotalStudents = totalStudents,
                TotalCourses = totalCourses,
                PublishedCourses = publishedCourses,
                TotalEnrollments = totalEnrollments,
                TotalRevenue = totalRevenue,
                TotalReviews = totalReviews,
                AverageRating = averageRating
            };

            return new ApiResponse<DashboardStatsDto>
            {
                Success = true,
                Message = "Dashboard stats retrieved successfully",
                Data = stats
            };
        }

        public async Task<ApiResponse<List<TopCourseDto>>> GetTopCoursesAsync(int count = 10)
        {
            var topCourses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .Where(c => c.IsPublished)
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(count)
                .Select(c => new TopCourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    EnrollmentCount = c.Enrollments.Count,
                    Revenue = c.Enrollments.Sum(e => e.PricePaid),
                    AverageRating = c.Reviews.Any() ? Math.Round(c.Reviews.Average(r => r.Rating), 2) : 0,
                    InstructorName = c.Instructor.FullName
                })
                .ToListAsync();

            return new ApiResponse<List<TopCourseDto>>
            {
                Success = true,
                Message = "Top courses retrieved successfully",
                Data = topCourses
            };
        }

        public async Task<ApiResponse<List<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 20)
        {
            var activities = new List<RecentActivityDto>();

            // Recent enrollments
            var recentEnrollments = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrolledAt)
                .Take(count / 4)
                .ToListAsync();

            activities.AddRange(recentEnrollments.Select(e => new RecentActivityDto
            {
                ActivityType = "enrollment",
                Description = $"{e.User.FullName} enrolled in {e.Course.Title}",
                Timestamp = e.EnrolledAt
            }));

            // Recent reviews
            var recentReviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Course)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count / 4)
                .ToListAsync();

            activities.AddRange(recentReviews.Select(r => new RecentActivityDto
            {
                ActivityType = "review",
                Description = $"{r.User.FullName} reviewed {r.Course.Title} ({r.Rating} stars)",
                Timestamp = r.CreatedAt
            }));

            // Recent courses
            var recentCourses = await _context.Courses
                .Include(c => c.Instructor)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count / 4)
                .ToListAsync();

            activities.AddRange(recentCourses.Select(c => new RecentActivityDto
            {
                ActivityType = "course_created",
                Description = $"{c.Instructor.FullName} created course: {c.Title}",
                Timestamp = c.CreatedAt
            }));

            // Recent user registrations
            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(count / 4)
                .ToListAsync();

            activities.AddRange(recentUsers.Select(u => new RecentActivityDto
            {
                ActivityType = "user_registered",
                Description = $"{u.FullName} registered as {u.Role}",
                Timestamp = u.CreatedAt
            }));

            // Sort all activities by timestamp and take requested count
            var sortedActivities = activities
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToList();

            return new ApiResponse<List<RecentActivityDto>>
            {
                Success = true,
                Message = "Recent activities retrieved successfully",
                Data = sortedActivities
            };
        }

        public async Task<ApiResponse<List<UserResponseDto>>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role.ToString()
            }).ToList();

            return new ApiResponse<List<UserResponseDto>>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = userDtos
            };
        }

        public async Task<ApiResponse<bool>> ChangeUserRoleAsync(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            if (!Enum.TryParse<UserRole>(newRole, true, out var role))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid role"
                };
            }

            user.Role = role;
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "User role updated successfully",
                Data = true
            };
        }

        public async Task<ApiResponse<bool>> ToggleCoursePublishStatusAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Course not found"
                };
            }

            course.IsPublished = !course.IsPublished;
            course.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = $"Course {(course.IsPublished ? "published" : "unpublished")} successfully",
                Data = true
            };
        }
    }
}