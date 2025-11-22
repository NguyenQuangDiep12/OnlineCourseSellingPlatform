namespace OnlineCourseSellingPlatform.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}
