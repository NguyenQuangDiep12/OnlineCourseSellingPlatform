using OnlineCourseSellingPlatform.Models;

namespace OnlineCourseSellingPlatform.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
