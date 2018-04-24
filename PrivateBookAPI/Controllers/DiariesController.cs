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
    [Route("api/Diaries")]
    public class DiariesController : Controller
    {
        private readonly PrivateBookContext _context;

        public DiariesController(PrivateBookContext context)
        {
            _context = context;
        }

        // GET: api/Diaries
        // Get all diary entries
        [HttpGet]
        public IEnumerable<Diary> GetDiary()
        {
            return _context.Diary;
        }

        // GET: api/Diaries/5
        // Get diary entry by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiary([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diary = await _context.Diary.SingleOrDefaultAsync(m => m.EntryId == id);

            if (diary == null)
            {
                return NotFound();
            }

            return Ok(diary);
        }

        // PUT: api/Diaries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiary([FromRoute] int id, [FromBody] Diary diary)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != diary.EntryId)
            {
                return BadRequest();
            }

            _context.Entry(diary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiaryExists(id))
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

        // POST: api/Diaries
        //Submit a new diary entry
        [HttpPost]
        public async Task<IActionResult> PostDiary([FromBody] Diary diary)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Diary.Add(diary);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiary", new { id = diary.EntryId }, diary);
        }

        // DELETE: api/Diaries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiary([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diary = await _context.Diary.SingleOrDefaultAsync(m => m.EntryId == id);
            if (diary == null)
            {
                return NotFound();
            }

            _context.Diary.Remove(diary);
            await _context.SaveChangesAsync();

            return Ok(diary);
        }

        //Get diary entries for a particular user
        [HttpGet("GetDiaryEntriesByUser/{userId}", Name = "GetEntriesByUser")]
        public async Task<IActionResult> GetEntriesByUser([FromRoute] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diaryEntries = await _context.Diary.Where(x => x.UserId == userId).OrderByDescending(x => x.EntryId).ToListAsync();

            return this.Ok(diaryEntries);
        }

        //Check if diary exists
        private bool DiaryExists(int id)
        {
            return _context.Diary.Any(e => e.EntryId == id);
        }
    }
}