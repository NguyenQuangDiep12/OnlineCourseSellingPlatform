using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;

namespace OnlineCourseSellingPlatform.Controllers
{
    [Route("api/courses/{courseId}/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly IUserContextService _userContextService;
        private readonly ILessonService _lessonService;
        public LessonsController(
            IUserContextService userContextService,
            ILessonService lessonService)
        {
            _userContextService = userContextService;
            _lessonService = lessonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLessons(int courseId)
        {
            var result = await _lessonService.GetCourseLessonsAsync(courseId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "Instructor, Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateLesson(int courseId, [FromBody] CreateLessonDto dto)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _lessonService.CreateLessonAsync(courseId, dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(
                nameof(GetLessons),
                new {courseId = courseId},
                dto
            );
        }

        [Authorize(Roles = "Instructor, Admin")]
        [HttpPut("{lessonId}")]
        public async Task<IActionResult> UpdateLesson(int lessonId, [FromBody] UpdateLessonDto dto)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _lessonService.UpdateLessonAsync(lessonId, dto, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "Instructor, Admin")]
        [HttpDelete("{lessonId}")]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _lessonService.DeleteLessonAsync(lessonId, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
