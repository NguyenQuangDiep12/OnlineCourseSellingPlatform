using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;

namespace OnlineCourseSellingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IUserContextService _userContextService;

        public ReviewsController(IReviewService reviewService, IUserContextService userContextService)
        {
            _reviewService = reviewService;
            _userContextService = userContextService;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseReviews(
            int courseId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _reviewService.GetCourseReviewsAsync(courseId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("course/{courseId}/stats")]
        public async Task<IActionResult> GetCourseRatingStats(int courseId)
        {
            var result = await _reviewService.GetCourseRatingStatsAsync(courseId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("my-review/course/{courseId}")]
        public async Task<IActionResult> GetMyReviewForCourse(int courseId)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _reviewService.GetUserReviewForCourseAsync(userId, courseId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _reviewService.CreateReviewAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto dto)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _reviewService.UpdateReviewAsync(reviewId, userId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _reviewService.DeleteReviewAsync(reviewId, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}