using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PrivateBookAPI.Data.DTO;

namespace PrivateBookAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Weather")]
    public class WeatherController : Controller
    {
        // GET: api/Weather
        private IConfiguration configuration;

        public WeatherController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // POST: api/Weather
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Coordinates coordinates)
        {
            var location = coordinates;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://api.openweathermap.org");
                    string key = configuration.GetValue<string>("APIKeys:OpenWeatherMap");
                    var response = await client.GetAsync($"/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={key}&units=metric");
                    response.EnsureSuccessStatusCode();
                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeatherData = JsonConvert.DeserializeObject<OpenWeatherResponse>(stringResult);

                    WeatherDetails weather = new WeatherDetails()
                    {
                        City = rawWeatherData.Name,
                        Temperature = rawWeatherData.Main.Temp,
                        Description = rawWeatherData.Weather.FirstOrDefault().Description                      
                    };

                    return Ok(weather);
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
            }
        }

        // PUT: api/Weather/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
