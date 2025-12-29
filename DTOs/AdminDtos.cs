namespace OnlineCourseSellingPlatform.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
    }

    public class RecentActivityDto
    {
        public string ActivityType { get; set; } = string.Empty; // "enrollment", "review", "course_created", etc.
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class TopCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
        public decimal Revenue { get; set; }
        public double AverageRating { get; set; }
        public string InstructorName { get; set; } = string.Empty;
    }
}