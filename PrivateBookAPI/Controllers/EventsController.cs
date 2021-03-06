﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrivateBookAPI.Data;
using PrivateBookAPI.Data.DTO;

namespace PrivateBookAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Events")]
    public class EventsController : Controller
    {
        private readonly PrivateBookContext _context;

        public EventsController(PrivateBookContext context)
        {
            _context = context;
        }

        // GET: api/Events
        // Get events for user
        [HttpGet]
        public IEnumerable<Event> GetEvents()
        {
            return _context.Events;
        }

        // GET: api/Events/5
        // Get event by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            return Ok(@event);
        }

        // PUT: api/Events/5
        // Update event
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent([FromRoute] int id, [FromBody] Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != @event.EventId)
            {
                return BadRequest();
            }

            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Events
        // Save new event
        [HttpPost]
        public async Task<IActionResult> PostEvent([FromBody] Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = @event.EventId }, @event);
        }

        // DELETE: api/Events/5
        // Delete an event
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return Ok(@event);
        }

        //Get all events for user
        [HttpGet("user/{userId}", Name = "GetEventsByUser")]
        public async Task<IActionResult> GetEventsByUser(int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var events = await _context.Events.Where(x => x.UserId == userId && x.Time >= DateTime.Today).ToListAsync();

            return Ok(events);
        }

        //Search events for user
        [HttpGet("search/{startDate}/{endDate}/{userId}", Name = "SearchEvents")]
        public async Task<IActionResult> SearchEvents(DateTime startDate, DateTime endDate, int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var events = await _context.Events.Where(x => x.UserId == userId && x.Time >= startDate && x.Time <= endDate).ToListAsync();
            //var events = await _context.Events.Where(x => x.UserId == userId).ToListAsync();

            return Ok(events);
        }

        //Check if event exists
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}