// VolunteerController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TSU360.Database;
using TSU360.DTOs;
using TSU360.Models.DTO_s;
using TSU360.Models.Enums;
using TSU360.Services.Interfaces;

namespace TSU360.Controllers
{
    [Route("api/events/{eventId}/[controller]")]
    [ApiController]
    [Authorize]
    public class VolunteerController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ApplicationDbContext _context;

        public VolunteerController(IEventService eventService, ApplicationDbContext context)
        {
            _eventService = eventService;
            _context = context;

        }

        [HttpPost("questions")]
        [Authorize(Roles = "Admin,Curator")]
        public async Task<IActionResult> AddSurveyQuestion(Guid eventId, [FromBody] AddQuestionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var question = await _eventService.AddSurveyQuestionAsync(
                    eventId,
                    dto.QuestionText,
                    dto.IsRequired,
                    dto.QuestionType,
                    userId);

                return Ok(question);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("questions")]
        public async Task<IActionResult> GetSurveyQuestions(Guid eventId)
        {
            var questions = await _eventService.GetEventSurveyQuestionsAsync(eventId);
            return Ok(questions);
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyToVolunteer(Guid eventId, [FromBody] SubmitVolunteerApplicationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var application = await _eventService.ApplyToVolunteerAsync(eventId, userId, dto.Answers);

                // Get questions for response text
                var questions = await _context.SurveyQuestions
                    .Where(q => q.EventId == eventId)
                    .ToDictionaryAsync(q => q.Id, q => q.QuestionText);

                var result = new VolunteerApplicationResponseDto
                {
                    Id = application.Id,
                    UserId = application.UserId,
                    EventId = application.EventId,
                    Status = application.Status,
                    AppliedAt = application.AppliedAt,
                    ProcessedAt = application.ProcessedAt,
                    Responses = application.Responses.Select(r => new SurveyResponseDto
                    {
                        Id = r.Id,
                        QuestionId = r.QuestionId,
                        QuestionText = questions.GetValueOrDefault(r.QuestionId, "Unknown question"),
                        Answer = r.Answer
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("applications")]
        [Authorize(Roles = "Admin,Curator")]
        public async Task<IActionResult> GetApplications(Guid eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var applications = await _eventService.GetEventApplicationsAsync(eventId, userId);

                var result = applications.Select(a => new VolunteerApplicationResponseDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    EventId = a.EventId,
                    Status = a.Status,
                    AppliedAt = a.AppliedAt,
                    ProcessedAt = a.ProcessedAt,
                    Responses = a.Responses?.Select(r => new SurveyResponseDto
                    {
                        Id = r.Id,
                        QuestionId = r.QuestionId,
                        QuestionText = r.Question?.QuestionText ?? "Unknown question",
                        Answer = r.Answer
                    }).ToList() ?? new List<SurveyResponseDto>()
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("applications/{applicationId}")]
        [Authorize(Roles = "Admin,Curator")]
        public async Task<IActionResult> ProcessApplication(Guid eventId, Guid applicationId, [FromBody] ProcessApplicationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var application = await _eventService.ProcessVolunteerApplicationAsync(
                    applicationId,
                    dto.Status,
                    userId);

                return Ok(application);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
