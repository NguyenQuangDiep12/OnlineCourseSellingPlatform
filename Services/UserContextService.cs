using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Models;
using System.Security.Claims;

namespace OnlineCourseSellingPlatform.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int GetCurrentUserId()
        {
            var userIdClams = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClams, out int userId) ? userId : 0;
        }
    }
}
