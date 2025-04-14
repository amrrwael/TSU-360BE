// IEventService.cs (in Services/Interfaces folder)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSU360.DTOs;
using TSU360.Models.Entities;
using TSU360.Models.Enums;

namespace TSU360.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventDto> CreateEventAsync(CreateEventDto createEventDto, string userId);
        Task<EventDto> GetEventByIdAsync(Guid id);
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<EventDto> UpdateEventAsync(Guid id, UpdateEventDto updateEventDto, string userId, bool isAdmin);
        Task<bool> DeleteEventAsync(Guid id, string userId, bool isAdmin);
        Task<IEnumerable<EventDto>> GetEventsByOrganizerAsync(string userId);
        Task<SurveyQuestion> AddSurveyQuestionAsync(Guid eventId, string questionText, bool isRequired, QuestionType questionType, string userId);
        Task<IEnumerable<SurveyQuestion>> GetEventSurveyQuestionsAsync(Guid eventId);
        Task<VolunteerApplication> ApplyToVolunteerAsync(Guid eventId, string userId, Dictionary<Guid, string> answers);
        Task<VolunteerApplication> ProcessVolunteerApplicationAsync(Guid applicationId, ApplicationStatus status, string curatorId);
        Task<IEnumerable<VolunteerApplication>> GetEventApplicationsAsync(Guid eventId, string curatorId);
        Task<IEnumerable<VolunteerApplication>> GetUserApplicationsAsync(string userId);

    }
}