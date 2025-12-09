using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;

namespace OnlineCourseSellingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IUserContextService _userContextService;

        public CoursesController(
            ICourseService courseService,
            IUserContextService userContextService)
        {
            _courseService = courseService;
            _userContextService = userContextService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _courseService.GetCourseAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var result = await _courseService.GetCourseByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "Instructor, Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _courseService.CreateCourseAsync(dto, userId);

            return CreatedAtAction(
                    nameof(GetCourse),
                    new { id = result.Data?.Id },
                    result
            );
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _courseService.UpdateCourseAsync(id, dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "Instructor, Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _courseService.DeleteCourseAsync(id, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Instructor, Admin")]
        [HttpGet("my-course")]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _courseService.GetCoursesByInstructorAsync(userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
