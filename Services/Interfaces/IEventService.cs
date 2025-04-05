// IEventService.cs (in Services/Interfaces folder)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSU360.DTOs;

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
    }
}