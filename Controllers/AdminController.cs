using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourseSellingPlatform.Interfaces;

namespace OnlineCourseSellingPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _adminService.GetDashboardStatsAsync();
            return Ok(result);
        }

        [HttpGet("top-courses")]
        public async Task<IActionResult> GetTopCourses([FromQuery] int count = 10)
        {
            var result = await _adminService.GetTopCoursesAsync(count);
            return Ok(result);
        }

        [HttpGet("recent-activities")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int count = 20)
        {
            var result = await _adminService.GetRecentActivitiesAsync(count);
            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _adminService.GetAllUsersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("users/{userId}/role")]
        public async Task<IActionResult> ChangeUserRole(int userId, [FromBody] string newRole)
        {
            var result = await _adminService.ChangeUserRoleAsync(userId, newRole);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("courses/{courseId}/toggle-publish")]
        public async Task<IActionResult> ToggleCoursePublishStatus(int courseId)
        {
            var result = await _adminService.ToggleCoursePublishStatusAsync(courseId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}