using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSU360.Models.DTOs;
using TSU360.Services.Interfaces;

namespace TSU360.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadgeController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        public BadgeController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        // GET: api/Badge
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BadgeDTO>>> GetBadges()
        {
            var badges = await _badgeService.GetAllBadgesAsync();
            return Ok(badges);
        }

        // GET: api/Badge/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BadgeDTO>> GetBadge(Guid id)
        {
            var badge = await _badgeService.GetBadgeByIdAsync(id);

            if (badge == null)
            {
                return NotFound();
            }

            return badge;
        }

        // POST: api/Badge
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BadgeDTO>> PostBadge(CreateBadgeDTO badgeDto)
        {
            var createdBadge = await _badgeService.CreateBadgeAsync(badgeDto);
            return CreatedAtAction(nameof(GetBadge), new { id = createdBadge.Id }, createdBadge);
        }

        // PUT: api/Badge/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutBadge(Guid id, UpdateBadgeDTO badgeDto)
        {
            var updatedBadge = await _badgeService.UpdateBadgeAsync(id, badgeDto);

            if (updatedBadge == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Badge/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBadge(Guid id)
        {
            var result = await _badgeService.DeleteBadgeAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}