using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PrivateBookAPI.Data;

namespace PrivateBookAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Stocks")]
    public class StocksController : Controller
    {
        private readonly PrivateBookContext _context;
        private IConfiguration configuration;

        public StocksController(PrivateBookContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
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

            bool currentStocks =  _context.Stocks.Any(x => x.UserId == stock.UserId && x.StockSymbol == stock.StockSymbol);
            if(currentStocks == true)
            {
                return Ok("Already Exists");
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

         
        [HttpGet("StockValue/{symbol}", Name = "GetStockDetails")]
        public async Task<IActionResult> GetStockDetails(string symbol)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://www.alphavantage.co");
                    string key = configuration.GetValue<string>("APIKeys:AlphaVantage");
                    var response = await client.GetAsync($"/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={symbol}&apikey={key}");
                    var stringResult = await response.Content.ReadAsStringAsync();
                    var jObj = JObject.Parse(stringResult);
                    //var metadata = jObj["Meta Data"].ToObject<Dictionary<string, string>>();
                    //var timeseries = jObj["Time Series (1min)"].ToObject<Dictionary<string, Dictionary<string, string>>>();
                    // return this.Ok(timeseries);
                    return this.Ok(jObj);
                }
            }
            catch(Exception ex)
            {
                
            }          
            return this.BadRequest();
        }

        [HttpGet("StocksForUser/{userId}", Name = "GetStockByUser")]
        public async Task<IActionResult> GetStockByUser(int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stocks = await _context.Stocks.Where(x => x.UserId == userId).ToListAsync();

            return this.Ok(stocks);
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockMappingId == id);
        }
    }
}