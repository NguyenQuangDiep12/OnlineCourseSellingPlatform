namespace OnlineCourseSellingPlatform.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; } // Position of the lesson within the course example: Lesson 1 -> OrderIndex = 1, Reward Lesson -> OrderIndex = 1,1
        public int DurationInMinutes { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
