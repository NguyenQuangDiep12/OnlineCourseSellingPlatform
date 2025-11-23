namespace OnlineCourseSellingPlatform.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    }

     
    public enum UserRole
    {
        Student,
        Instructor,
        Admin
    }
}
