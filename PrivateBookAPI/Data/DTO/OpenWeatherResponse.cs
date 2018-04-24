using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateBookAPI.Data.DTO
{
    // Class for open weather response
    public class OpenWeatherResponse
    {
        public string Name { get; set; }

        public IEnumerable<WeatherDescription> Weather { get; set; }

        public Main Main { get; set; }
    }

    // Class for weather description
    public class WeatherDescription
    {
        public string Main { get; set; }
        public string Description { get; set; }
    }

    // Class for temperature
    public class Main
    {
        public string Temp { get; set; }
    }
}
