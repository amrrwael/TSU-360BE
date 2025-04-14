// EventService.cs (in Services/Implementations folder)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TSU360.Database;
using TSU360.DTOs;
using TSU360.Models.Entities;
using TSU360.Models.Enums;
using TSU360.Services.Interfaces;

namespace TSU360.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDto, string userId)
        {
            var newEvent = new Event
            {
                Title = createEventDto.Title,
                Description = createEventDto.Description,
                StartDate = createEventDto.StartDate,
                EndDate = createEventDto.EndDate,
                Location = createEventDto.Location,
                ImageUrl = createEventDto.ImageUrl,
                Status = EventStatus.Draft,
                CreatedById = userId
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(newEvent.Id);
        }

        public async Task<EventDto> GetEventByIdAsync(Guid id)
        {
            var eventEntity = await _context.Events
                .Include(e => e.CreatedBy)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null)
                return null;

            return MapToDto(eventEntity);
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _context.Events
                .Include(e => e.CreatedBy)
                .ToListAsync();

            return events.Select(MapToDto);
        }

        public async Task<EventDto> UpdateEventAsync(Guid id, UpdateEventDto updateEventDto, string userId, bool isAdmin)
        {
            var eventEntity = await _context.Events.FindAsync(id);

            if (eventEntity == null)
                throw new Exception("Event not found");

            // Check if user is admin or the creator of the event
            if (!isAdmin && eventEntity.CreatedById != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this event");

            eventEntity.Title = updateEventDto.Title;
            eventEntity.Description = updateEventDto.Description;
            eventEntity.StartDate = updateEventDto.StartDate;
            eventEntity.EndDate = updateEventDto.EndDate;
            eventEntity.Location = updateEventDto.Location;
            eventEntity.ImageUrl = updateEventDto.ImageUrl;
            eventEntity.Status = updateEventDto.Status;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(id);
        }

        public async Task<bool> DeleteEventAsync(Guid id, string userId, bool isAdmin)
        {
            var eventEntity = await _context.Events.FindAsync(id);

            if (eventEntity == null)
                throw new Exception("Event not found");

            // Check if user is admin or the creator of the event
            if (!isAdmin && eventEntity.CreatedById != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this event");

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<EventDto>> GetEventsByOrganizerAsync(string userId)
        {
            var events = await _context.Events
                .Include(e => e.CreatedBy)
                .Where(e => e.CreatedById == userId)
                .ToListAsync();

            return events.Select(MapToDto);
        }

        private static EventDto MapToDto(Event eventEntity)
        {
            return new EventDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                Location = eventEntity.Location,
                Status = eventEntity.Status,
                ImageUrl = eventEntity.ImageUrl,
                CreatedAt = eventEntity.CreatedAt,
                CreatedById = eventEntity.CreatedById,
                CreatedBy = $"{eventEntity.CreatedBy.FirstName} {eventEntity.CreatedBy.LastName}"
            };
        }
        // Add these methods to EventService
        public async Task<SurveyQuestion> AddSurveyQuestionAsync(Guid eventId, string questionText, bool isRequired, QuestionType questionType, string userId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
                throw new Exception("Event not found");

            if (eventEntity.CreatedById != userId)
                throw new UnauthorizedAccessException("Only event creator can add questions");

            var question = new SurveyQuestion
            {
                QuestionText = questionText,
                IsRequired = isRequired,
                QuestionType = questionType,
                EventId = eventId
            };

            _context.SurveyQuestions.Add(question);
            await _context.SaveChangesAsync();

            return question;
        }

        public async Task<IEnumerable<SurveyQuestion>> GetEventSurveyQuestionsAsync(Guid eventId)
        {
            return await _context.SurveyQuestions
                .Where(q => q.EventId == eventId)
                .ToListAsync();
        }

        public async Task<VolunteerApplication> ApplyToVolunteerAsync(Guid eventId, string userId, Dictionary<Guid, string> answers)
        { 
            // Check if application already exists
            var existingApplication = await _context.VolunteerApplications
                .FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId);

            if (existingApplication != null)
                throw new Exception("You have already applied to this event");

            // Validate all required questions are answered
            var requiredQuestions = await _context.SurveyQuestions
                .Where(q => q.EventId == eventId && q.IsRequired)
                .ToListAsync();

            foreach (var question in requiredQuestions)
            {
                if (!answers.ContainsKey(question.Id))
                    throw new Exception($"Required question not answered: {question.QuestionText}");
            }

            // Create application
            var application = new VolunteerApplication
            {
                UserId = userId,
                EventId = eventId,
                Status = ApplicationStatus.Pending
            };

            _context.VolunteerApplications.Add(application);
            await _context.SaveChangesAsync();

            // Add responses
            foreach (var answer in answers)
            {
                var response = new SurveyResponse
                {
                    QuestionId = answer.Key,
                    ApplicationId = application.Id,
                    Answer = answer.Value
                };
                _context.SurveyResponses.Add(response);
            }

            await _context.SaveChangesAsync();

            // Explicitly load the responses we just created
            await _context.Entry(application)
                .Collection(a => a.Responses)
                .LoadAsync();

            return application;
        }

        public async Task<VolunteerApplication> ProcessVolunteerApplicationAsync(Guid applicationId, ApplicationStatus status, string curatorId)
        {
            var application = await _context.VolunteerApplications
                .Include(a => a.Event)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                throw new Exception("Application not found");

            if (application.Event.CreatedById != curatorId)
                throw new UnauthorizedAccessException("Only event curator can process applications");

            application.Status = status;
            application.ProcessedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<IEnumerable<VolunteerApplication>> GetEventApplicationsAsync(Guid eventId, string curatorId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
                throw new Exception("Event not found");

            if (eventEntity.CreatedById != curatorId)
                throw new UnauthorizedAccessException("Only event curator can view applications");

            return await _context.VolunteerApplications
                .Include(a => a.User)
                .Include(a => a.Responses)
                .ThenInclude(r => r.Question)
                .Where(a => a.EventId == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<VolunteerApplication>> GetUserApplicationsAsync(string userId)
        {
            return await _context.VolunteerApplications
                .Include(a => a.Event)
                .Include(a => a.Responses)
                .ThenInclude(r => r.Question)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
      
    }
}