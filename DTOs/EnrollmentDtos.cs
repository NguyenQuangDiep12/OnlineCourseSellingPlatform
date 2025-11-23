namespace OnlineCourseSellingPlatform.DTOs
{
    public class EnrollmentCourseDto
    {
        public int CourseId { get; set; }
    }
    public class EnrollmentResponseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
        public int Progress { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
