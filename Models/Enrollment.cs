namespace OnlineCourseSellingPlatform.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User user { get; set; } = null!;
        public int CourseId { get; set; }
        public Course course { get; set; } = null!;
        public DateTime EnrolledAt { get; set; }
        public decimal PricePaid { get; set; } 
        public EnrollmentStatus Status { get; set; }
        public int Progress { get; set; } // percentage of course completed (0 - 100)
    }
    public enum EnrollmentStatus
    {
        InProgress,
        Completed,
        Cancelled
    }
}
