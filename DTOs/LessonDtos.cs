namespace OnlineCourseSellingPlatform.DTOs
{
    public class CreateLessonDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public int DurationInMinutes { get; set; }
    }
    public class UpdateLessonDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int? OrderIndex { get; set; }
        public int? DurationMinutes { get; set; }
    }

    public class LessonResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public int DurationMinutes { get; set; }
    }
}
