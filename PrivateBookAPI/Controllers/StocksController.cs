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
    [Route("api/Stocks")]
    public class StocksController : Controller
    {
        private readonly PrivateBookContext _context;

        public StocksController(PrivateBookContext context)
        {
            _context = context;
        }

        // GET: api/Stocks
        [HttpGet]
        public IEnumerable<Stock> GetStocks()
        {
            return _context.Stocks;
        }

        // GET: api/Stocks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _context.Stocks.SingleOrDefaultAsync(m => m.StockMappingId == id);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }

        // PUT: api/Stocks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock([FromRoute] int id, [FromBody] Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stock.StockMappingId)
            {
                return BadRequest();
            }

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
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

        // POST: api/Stocks
        [HttpPost]
        public async Task<IActionResult> PostStock([FromBody] Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStock", new { id = stock.StockMappingId }, stock);
        }

        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _context.Stocks.SingleOrDefaultAsync(m => m.StockMappingId == id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            return Ok(stock);
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockMappingId == id);
        }
    }
}