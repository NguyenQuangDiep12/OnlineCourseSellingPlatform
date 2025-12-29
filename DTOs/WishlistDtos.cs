namespace OnlineCourseSellingPlatform.DTOs
{
    public class AddToWishlistDto
    {
        public int CourseId { get; set; }
    }

    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseDescription { get; set; } = string.Empty;
        public decimal CoursePrice { get; set; }
        public string CourseThumbnail { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime AddedAt { get; set; }
    }
}