// EventController.cs (in Controllers folder)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TSU360.DTOs;
using TSU360.Services.Interfaces;
using TSU360.Models.Enums;

namespace TSU360.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;

        public EventController(IEventService eventService, IUserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Curator")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _eventService.CreateEventAsync(createEventDto, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var result = await _eventService.GetEventByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var result = await _eventService.GetAllEventsAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventDto updateEventDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole(UserRole.Admin.ToString());

            try
            {
                var result = await _eventService.UpdateEventAsync(id, updateEventDto, userId, isAdmin);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole(UserRole.Admin.ToString());

            try
            {
                var result = await _eventService.DeleteEventAsync(id, userId, isAdmin);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-events")]
        [Authorize(Roles = "Admin,Curator")]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _eventService.GetEventsByOrganizerAsync(userId);
            return Ok(result);
        }

    }
}