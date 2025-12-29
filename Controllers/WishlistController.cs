using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourseSellingPlatform.DTOs;
using OnlineCourseSellingPlatform.Interfaces;

namespace OnlineCourseSellingPlatform.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly IUserContextService _userContextService;

        public WishlistController(IWishlistService wishlistService, IUserContextService userContextService)
        {
            _wishlistService = wishlistService;
            _userContextService = userContextService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDto dto)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _wishlistService.AddToWishlistAsync(userId, dto.CourseId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{courseId}")]
        public async Task<IActionResult> RemoveFromWishlist(int courseId)
        {
            var userId = _userContextService.GetCurrentUserId();
            var result = await _wishlistService.RemoveFromWishlistAsync(userId, courseId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("check/{courseId}")]
        public async Task<IActionResult> CheckIfInWishlist(int courseId)
        {
            var userId = _userContextService.GetCurrentUserId();
            var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, courseId);
            return Ok(new { isInWishlist });
        }
    }
}