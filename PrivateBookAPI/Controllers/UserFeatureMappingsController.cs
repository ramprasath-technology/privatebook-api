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
    [Route("api/UserFeatureMappings")]
    public class UserFeatureMappingsController : Controller
    {
        private readonly PrivateBookContext _context;

        public UserFeatureMappingsController(PrivateBookContext context)
        {
            _context = context;
        }

        // GET: api/UserFeatureMappings
        [HttpGet]
        public IEnumerable<UserFeatureMapping> GetUserFeatureMappings()
        {
            return _context.UserFeatureMappings;
        }

        // GET: api/UserFeatureMappings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserFeatureMapping([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userFeatureMapping = await _context.UserFeatureMappings.SingleOrDefaultAsync(m => m.MappingId == id);

            if (userFeatureMapping == null)
            {
                return NotFound();
            }

            return Ok(userFeatureMapping);
        }

        // PUT: api/UserFeatureMappings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserFeatureMapping([FromRoute] int id, [FromBody] UserFeatureMapping userFeatureMapping)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userFeatureMapping.MappingId)
            {
                return BadRequest();
            }

            _context.Entry(userFeatureMapping).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserFeatureMappingExists(id))
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

        // POST: api/UserFeatureMappings
        [HttpPost]
        public async Task<IActionResult> PostUserFeatureMapping([FromBody] UserFeatureMapping userFeatureMapping)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.UserFeatureMappings.Add(userFeatureMapping);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUserFeatureMapping", new { id = userFeatureMapping.MappingId }, userFeatureMapping);
            }
            catch(Exception ex)
            {
                return BadRequest("Bad request");
            }
        }

        // DELETE: api/UserFeatureMappings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserFeatureMapping([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userFeatureMapping = await _context.UserFeatureMappings.SingleOrDefaultAsync(m => m.MappingId == id);
            if (userFeatureMapping == null)
            {
                return NotFound();
            }

            _context.UserFeatureMappings.Remove(userFeatureMapping);
            await _context.SaveChangesAsync();

            return Ok(userFeatureMapping);
        }

        [HttpGet("GetFeatureByUser/{userId}", Name = "GetFeaturesByUser")]
        public async Task<IActionResult> GetFeaturesByUser(int userId)
        {
            var features = await _context.UserFeatureMappings.Include(x => x.Feature).Where(x => x.UserId == userId).ToListAsync();

            return Ok(features);
        }

        private bool UserFeatureMappingExists(int id)
        {
            return _context.UserFeatureMappings.Any(e => e.MappingId == id);
        }
    }
}