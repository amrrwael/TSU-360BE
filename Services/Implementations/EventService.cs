﻿// EventService.cs (in Services/Implementations folder)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}