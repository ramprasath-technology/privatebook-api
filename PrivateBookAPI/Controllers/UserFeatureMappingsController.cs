using System;
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
    [Route("api/UserFeatureMappings")]
    public class UserFeatureMappingsController : Controller
    {
        private readonly PrivateBookContext _context;

        public UserFeatureMappingsController(PrivateBookContext context)
        {
            _context = context;
        }

        // GET: api/UserFeatureMappings
        // Get all feature mappings
        [HttpGet]
        public IEnumerable<UserFeatureMapping> GetUserFeatureMappings()
        {
            return _context.UserFeatureMappings;
        }

        // GET: api/UserFeatureMappings/5
        // Get user feature mapping
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
        // Update a mapping
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
        // Create a new mapping
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
        // Delete a particular mapping
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

        // Get features for a user
        [HttpGet("GetFeatureByUser/{userId}", Name = "GetFeaturesByUser")]
        public async Task<IActionResult> GetFeaturesByUser(int userId)
        {
            var features = await _context.UserFeatureMappings.Include(x => x.Feature).Where(x => x.UserId == userId).ToListAsync();

            return Ok(features);
        }

        // Add all features for a user
        [HttpGet("AddFeaturesToUser/{userId}", Name = "AddFeaturesToUser")]
        public async Task<IActionResult> AddFeaturesToUser(int userId)
        {
                var features = _context.Features;
                foreach(var feature in features)
                {
                    UserFeatureMapping mapping = new UserFeatureMapping()
                    {
                        FeatureId = feature.FeatureId,
                        UserId = userId
                    };
                    _context.UserFeatureMappings.Add(mapping);
                }

            await _context.SaveChangesAsync();
            return this.Ok();
        }

        // Add feature to user
        [HttpPost("UserFeatureToAdd", Name = "AddFeatureToUser")]
        public async Task<IActionResult> AddFeatureToUser([FromBody] UserFeatures userFeature)
        {
            UserFeatureMapping mapping = new UserFeatureMapping()
            {
                FeatureId = userFeature.FeatureId,
                UserId = userFeature.UserId
            };

            _context.Add(mapping);
            await _context.SaveChangesAsync();
            return this.Ok();
        }

        //Remove feature for a user
        [HttpPost("UserFeatureToRemove", Name = "RemoveFeatureForUser")]
        public async Task<IActionResult> RemoveFeatureForUser([FromBody] UserFeatures userFeature)
        {
            UserFeatureMapping mapping = await _context.UserFeatureMappings.FirstOrDefaultAsync(x => x.UserId == userFeature.UserId && x.FeatureId == userFeature.FeatureId);

            if(mapping != null)
            {
                _context.UserFeatureMappings.Remove(mapping);
            }

            await _context.SaveChangesAsync();
            return this.Ok();
        }

        //Check if mapping exists
        private bool UserFeatureMappingExists(int id)
        {
            return _context.UserFeatureMappings.Any(e => e.MappingId == id);
        }
    }
}