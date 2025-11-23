using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace OnlineCourseSellingPlatform.DTOs
{

    public class CreateCourseDto 
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
    }
    public class UpdateCourseDto 
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Level { get; set; }
        public bool? IsPublished { get; set; }
    }

    public class CourseResponseDto 
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public int LessonCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CourseDetailDto : CourseResponseDto 
    {
        public List<LessonResponseDto> Lessons { get; set; } = new List<LessonResponseDto>();
    }
}
