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
    [Route("api/Features")]
    public class FeaturesController : Controller
    {
        private readonly PrivateBookContext _context;

        public FeaturesController(PrivateBookContext context)
        {
            _context = context;
        }

        // GET: api/Features
        // Get all features
        [HttpGet]
        public IEnumerable<Feature> GetFeatures()
        {
            return _context.Features;
        }

        // GET: api/Features/5
        // Get feature by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeature([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feature = await _context.Features.SingleOrDefaultAsync(m => m.FeatureId == id);

            if (feature == null)
            {
                return NotFound();
            }

            return Ok(feature);
        }

        // PUT: api/Features/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeature([FromRoute] int id, [FromBody] Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feature.FeatureId)
            {
                return BadRequest();
            }

            _context.Entry(feature).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeatureExists(id))
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

        // POST: api/Features
        // Post a new feature
        [HttpPost]
        public async Task<IActionResult> PostFeature([FromBody] Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Features.Add(feature);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFeature", new { id = feature.FeatureId }, feature);
        }

        // DELETE: api/Features/5
        // Delete a feature
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeature([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feature = await _context.Features.SingleOrDefaultAsync(m => m.FeatureId == id);
            if (feature == null)
            {
                return NotFound();
            }

            _context.Features.Remove(feature);
            await _context.SaveChangesAsync();

            return Ok(feature);
        }

        // Check if a feature exists
        private bool FeatureExists(int id)
        {
            return _context.Features.Any(e => e.FeatureId == id);
        }
    }
}