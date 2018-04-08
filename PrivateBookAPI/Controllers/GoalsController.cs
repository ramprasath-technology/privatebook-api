using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrivateBookAPI.Data;

namespace PrivateBookAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Goals")]
    public class GoalsController : Controller
    {
        private readonly PrivateBookContext _context;

        public GoalsController(PrivateBookContext context)
        {
            _context = context;
        }

        // GET: api/Goals
        [HttpGet]
        public IEnumerable<Goals> GetGoals()
        {
            return _context.Goals;
        }

        // GET: api/Goals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGoals([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var goals = await _context.Goals.SingleOrDefaultAsync(m => m.GoalId == id);

            if (goals == null)
            {
                return NotFound();
            }

            return Ok(goals);
        }

        // PUT: api/Goals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoals([FromRoute] int id, [FromBody] Goals goals)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != goals.GoalId)
            {
                return BadRequest();
            }

            _context.Entry(goals).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoalsExists(id))
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

        // POST: api/Goals
        [HttpPost]
        public async Task<IActionResult> PostGoals([FromBody] Goals goals)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Goals.Add(goals);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoals", new { id = goals.GoalId }, goals);
        }

        // DELETE: api/Goals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoals([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var goals = await _context.Goals.SingleOrDefaultAsync(m => m.GoalId == id);
            if (goals == null)
            {
                return NotFound();
            }

            _context.Goals.Remove(goals);
            await _context.SaveChangesAsync();

            return Ok(goals);
        }

        [HttpGet("user/{userId}", Name = "GetGoalsByUser")]
        public async Task<IActionResult> GetGoalsByUser(int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var goals = await _context.Goals.Where(x => x.UserId == userId).ToListAsync();

            return Ok(goals);
        }

        private bool GoalsExists(int id)
        {
            return _context.Goals.Any(e => e.GoalId == id);
        }
    }
}