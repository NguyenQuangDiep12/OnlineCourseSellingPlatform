using Microsoft.EntityFrameworkCore;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;

namespace OnlineCourseSellingPlatform.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;

        public WishlistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<WishlistItemDto>> AddToWishlistAsync(int userId, int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return new ApiResponse<WishlistItemDto>
                {
                    Success = false,
                    Message = "Course not found"
                };
            }

            if (await IsInWishlistAsync(userId, courseId))
            {
                return new ApiResponse<WishlistItemDto>
                {
                    Success = false,
                    Message = "Course is already in your wishlist"
                };
            }

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                CourseId = courseId,
                AddedAt = DateTime.UtcNow
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            await _context.Entry(wishlistItem).Reference(w => w.Course).LoadAsync();
            await _context.Entry(wishlistItem.Course).Reference(c => c.Instructor).LoadAsync();
            await _context.Entry(wishlistItem.Course).Collection(c => c.Reviews).LoadAsync();

            return new ApiResponse<WishlistItemDto>
            {
                Success = true,
                Message = "Course added to wishlist successfully",
                Data = MapToWishlistItemDto(wishlistItem)
            };
        }

        public async Task<ApiResponse<bool>> RemoveFromWishlistAsync(int userId, int courseId)
        {
            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.CourseId == courseId);

            if (wishlistItem == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Course not found in wishlist"
                };
            }

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Course removed from wishlist successfully",
                Data = true
            };
        }

        public async Task<ApiResponse<List<WishlistItemDto>>> GetUserWishlistAsync(int userId)
        {
            var wishlistItems = await _context.Wishlists
                .Include(w => w.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(w => w.Course)
                    .ThenInclude(c => c.Reviews)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();

            var wishlistDtos = wishlistItems.Select(MapToWishlistItemDto).ToList();

            return new ApiResponse<List<WishlistItemDto>>
            {
                Success = true,
                Message = "Wishlist retrieved successfully",
                Data = wishlistDtos
            };
        }

        public async Task<bool> IsInWishlistAsync(int userId, int courseId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.CourseId == courseId);
        }

        private WishlistItemDto MapToWishlistItemDto(Wishlist wishlist)
        {
            var averageRating = wishlist.Course.Reviews.Any()
                ? Math.Round(wishlist.Course.Reviews.Average(r => r.Rating), 2)
                : (double?)null;

            return new WishlistItemDto
            {
                Id = wishlist.Id,
                CourseId = wishlist.CourseId,
                CourseTitle = wishlist.Course.Title,
                CourseDescription = wishlist.Course.Description,
                CoursePrice = wishlist.Course.Price,
                CourseThumbnail = wishlist.Course.ThumbnailUrl,
                InstructorName = wishlist.Course.Instructor.FullName,
                AverageRating = averageRating,
                ReviewCount = wishlist.Course.Reviews.Count,
                AddedAt = wishlist.AddedAt
            };
        }
    }
}