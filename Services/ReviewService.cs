using Microsoft.EntityFrameworkCore;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;

namespace OnlineCourseSellingPlatform.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEnrollmentService _enrollmentService;

        public ReviewService(ApplicationDbContext context, IEnrollmentService enrollmentService)
        {
            _context = context;
            _enrollmentService = enrollmentService;
        }

        public async Task<ApiResponse<ReviewResponseDto>> CreateReviewAsync(int userId, CreateReviewDto dto)
        {
            // Check if course exists
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
            {
                return new ApiResponse<ReviewResponseDto>
                {
                    Success = false,
                    Message = "Course not found"
                };
            }

            // Check if user is enrolled in the course
            var isEnrolled = await _enrollmentService.IsUserEnrolledAsync(userId, dto.CourseId);
            if (!isEnrolled)
            {
                return new ApiResponse<ReviewResponseDto>
                {
                    Success = false,
                    Message = "You must be enrolled in this course to review it"
                };
            }

            // Check if user has already reviewed this course
            if (await HasUserReviewedCourseAsync(userId, dto.CourseId))
            {
                return new ApiResponse<ReviewResponseDto>
                {
                    Success = false,
                    Message = "You have already reviewed this course"
                };
            }

            // Validate rating
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return new ApiResponse<ReviewResponseDto>
                {
                    Success = false,
                    Message = "Rating must be between 1 and 5"
                };
            }

            var review = new Review
            {
                UserId = userId,
                CourseId = dto.CourseId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await _context.Entry(review).Reference(r => r.User).LoadAsync();
            await _context.Entry(review).Reference(r => r.Course).LoadAsync();

            return new ApiResponse<ReviewResponseDto>
            {
                Success = true,
                Message = "Review created successfully",
                Data = MapToReviewResponseDto(review)
            };
        }

        public async Task<ApiResponse<ReviewResponseDto>> UpdateReviewAsync(int reviewId, int userId, UpdateReviewDto dto)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
            {
                return new ApiResponse<ReviewResponseDto>
                {
                    Success = false,
                    Message = "Review not found"
                };
            }

            if (review.UserId != userId)
            {
                return new ApiResponse<ReviewResponseDto>
                {
                    Success = false,
                    Message = "Unauthorized to update this review"
                };
            }

            if (dto.Rating.HasValue)
            {
                if (dto.Rating.Value < 1 || dto.Rating.Value > 5)
                {
                    return new ApiResponse<ReviewResponseDto>
                    {
                        Success = false,
                        Message = "Rating must be between 1 and 5"
                    };
                }
                review.Rating = dto.Rating.Value;
            }

            if (dto.Comment != null)
            {
                review.Comment = dto.Comment;
            }

            review.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ApiResponse<ReviewResponseDto>
            {
                Success = true,
                Message = "Review updated successfully",
                Data = MapToReviewResponseDto(review)
            };
        }

        public async Task<ApiResponse<bool>> DeleteReviewAsync(int reviewId, int userId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);

            if (review == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Review not found"
                };
            }

            if (review.UserId != userId)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized to delete this review"
                };
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Review deleted successfully",
                Data = true
            };
        }

        public async Task<ApiResponse<List<ReviewResponseDto>>> GetCourseReviewsAsync(int courseId, int pageNumber, int pageSize)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Course)
                .Where(r => r.CourseId == courseId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var reviewDtos = reviews.Select(MapToReviewResponseDto).ToList();

            return new ApiResponse<List<ReviewResponseDto>>
            {
                Success = true,
                Message = "Reviews retrieved successfully",
                Data = reviewDtos
            };
        }

        public async Task<ApiResponse<ReviewResponseDto>> GetUserReviewForCourseAsync(int userId, int courseId)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);

            if (review == null)
            {
                return new ApiResponse<ReviewResponseDto>
                {
                    Success = false,
                    Message = "Review not found"
                };
            }

            return new ApiResponse<ReviewResponseDto>
            {
                Success = true,
                Data = MapToReviewResponseDto(review)
            };
        }

        public async Task<ApiResponse<CourseRatingStatsDto>> GetCourseRatingStatsAsync(int courseId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .ToListAsync();

            if (!reviews.Any())
            {
                return new ApiResponse<CourseRatingStatsDto>
                {
                    Success = true,
                    Data = new CourseRatingStatsDto
                    {
                        AverageRating = 0,
                        TotalReviews = 0,
                        FiveStarCount = 0,
                        FourStarCount = 0,
                        ThreeStarCount = 0,
                        TwoStarCount = 0,
                        OneStarCount = 0
                    }
                };
            }

            var stats = new CourseRatingStatsDto
            {
                AverageRating = Math.Round(reviews.Average(r => r.Rating), 2),
                TotalReviews = reviews.Count,
                FiveStarCount = reviews.Count(r => r.Rating == 5),
                FourStarCount = reviews.Count(r => r.Rating == 4),
                ThreeStarCount = reviews.Count(r => r.Rating == 3),
                TwoStarCount = reviews.Count(r => r.Rating == 2),
                OneStarCount = reviews.Count(r => r.Rating == 1)
            };

            return new ApiResponse<CourseRatingStatsDto>
            {
                Success = true,
                Data = stats
            };
        }

        public async Task<bool> HasUserReviewedCourseAsync(int userId, int courseId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.CourseId == courseId);
        }

        private ReviewResponseDto MapToReviewResponseDto(Review review)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User.FullName,
                UserProfilePicture = review.User.ProfilePictureUrl,
                CourseId = review.CourseId,
                CourseTitle = review.Course.Title,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }
    }
}