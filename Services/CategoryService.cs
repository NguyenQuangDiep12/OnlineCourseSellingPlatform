using Microsoft.EntityFrameworkCore;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;
using System.Text.RegularExpressions;

namespace OnlineCourseSellingPlatform.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var slug = GenerateSlug(dto.Name);

            if (await _context.Categories.AnyAsync(c => c.Slug == slug))
            {
                return new ApiResponse<CategoryResponseDto>
                {
                    Success = false,
                    Message = "Category with this name already exists"
                };
            }

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Slug = slug,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new ApiResponse<CategoryResponseDto>
            {
                Success = true,
                Message = "Category created successfully",
                Data = MapToCategoryResponseDto(category)
            };
        }

        public async Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return new ApiResponse<CategoryResponseDto>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            if (dto.Name != null)
            {
                category.Name = dto.Name;
                category.Slug = GenerateSlug(dto.Name);
            }

            if (dto.Description != null)
            {
                category.Description = dto.Description;
            }

            await _context.SaveChangesAsync();

            return new ApiResponse<CategoryResponseDto>
            {
                Success = true,
                Message = "Category updated successfully",
                Data = MapToCategoryResponseDto(category)
            };
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            if (category.Courses.Any())
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete category with existing courses"
                };
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Category deleted successfully",
                Data = true
            };
        }

        public async Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return new ApiResponse<CategoryResponseDto>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            return new ApiResponse<CategoryResponseDto>
            {
                Success = true,
                Data = MapToCategoryResponseDto(category)
            };
        }

        public async Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Courses)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var categoryDtos = categories.Select(MapToCategoryResponseDto).ToList();

            return new ApiResponse<List<CategoryResponseDto>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = categoryDtos
            };
        }

        public async Task<ApiResponse<List<CourseResponseDto>>> GetCoursesByCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return new ApiResponse<List<CourseResponseDto>>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Reviews)
                .Where(c => c.CategoryId == categoryId && c.IsPublished)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var courseDtos = courses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Price = c.Price,
                ThumbnailUrl = c.ThumbnailUrl,
                Level = c.Level.ToString(),
                IsPublished = c.IsPublished,
                InstructorName = c.Instructor.FullName,
                LessonCount = c.Lessons.Count,
                CreatedAt = c.CreatedAt
            }).ToList();

            return new ApiResponse<List<CourseResponseDto>>
            {
                Success = true,
                Message = "Courses retrieved successfully",
                Data = courseDtos
            };
        }

        private CategoryResponseDto MapToCategoryResponseDto(Category category)
        {
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                CourseCount = category.Courses?.Count ?? 0,
                CreatedAt = category.CreatedAt
            };
        }

        private string GenerateSlug(string name)
        {
            var slug = name.ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            return slug.Trim('-');
        }
    }
}