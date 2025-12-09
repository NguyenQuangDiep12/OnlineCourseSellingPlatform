using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;

namespace OnlineCourseSellingPlatform.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUserContextService _userContextService;
        public EnrollmentsController(
            IEnrollmentService enrollmentService, 
            IUserContextService userContextService)
        {
            _enrollmentService = enrollmentService;
            _userContextService = userContextService;
        }
        [HttpPost]
        public async Task<IActionResult> EnrollInCourse([FromBody] EnrollmentCourseDto dto)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _enrollmentService.EnrollInCourseAsync(userId, dto.CourseId);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("my-enrollments")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _enrollmentService.GetUserEnrollmentsAsync(userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{enrollmentId}/progress")]
        public async Task<IActionResult> UpdateProgress(int enrollmentId, [FromBody] int progress)
        {
            var result = await _enrollmentService.UpdateProgressAsync(enrollmentId, progress);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
