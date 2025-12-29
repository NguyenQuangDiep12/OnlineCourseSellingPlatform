using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface IWishlistService
    {
        Task<ApiResponse<WishlistItemDto>> AddToWishlistAsync(int userId, int courseId);
        Task<ApiResponse<bool>> RemoveFromWishlistAsync(int userId, int courseId);
        Task<ApiResponse<List<WishlistItemDto>>> GetUserWishlistAsync(int userId);
        Task<bool> IsInWishlistAsync(int userId, int courseId);
    }
}