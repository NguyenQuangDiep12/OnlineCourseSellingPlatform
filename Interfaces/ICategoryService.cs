using OnlineCourseSellingPlatform.DTOs;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto);
        Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId);
        Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId);
        Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoriesAsync();
        Task<ApiResponse<List<CourseResponseDto>>> GetCoursesByCategoryAsync(int categoryId, int pageNumber, int pageSize);
    }
}