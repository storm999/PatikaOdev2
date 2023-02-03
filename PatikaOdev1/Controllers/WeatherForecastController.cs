using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PatikaOdev1.Controllers
{
    public class WeatherForecast
    {
        public WeatherForecast() { }
        public WeatherForecast(DateTime _date, int _temperatureC, string _summary)
        {
            Date = _date;
            TemperatureC = _temperatureC;
            Summary = _summary;
        }

        [Required]
        public DateTime? Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

    public static class staticData
    {
        public static List<WeatherForecast> forecastList = new List<WeatherForecast>() { {new WeatherForecast(new DateTime(2) ,2,"cold") },
                                                                                            {new WeatherForecast(new DateTime(1) ,1,"warm") }};
    }

    //[Authorize(Roles = "Member")]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        /*private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };*/

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;

        }

        [HttpGet("GetWeatherForecastOrdered")]
        public IActionResult GetOrdered()
        {
            try
            {
                if (staticData.forecastList.Count == 0)
                {
                    return NoContent();
                }
            }
            catch
            {
                return BadRequest("An error occured");
            }
            return Ok(staticData.forecastList.OrderBy(t => t.TemperatureC));
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            try
            {
                if (staticData.forecastList.Count == 0)
                {
                    return NoContent();
                }
            }
            catch
            {
                return BadRequest("An error occured");
            }
            return Ok(staticData.forecastList);
        }

        [HttpPost(Name = "PostWeatherForecast")]
        public IActionResult Post([FromQuery] WeatherForecast wf)
        {
            try
            {
                staticData.forecastList.Add(wf);
                return Ok(staticData.forecastList);
            }
            catch
            {
                return BadRequest("An error occured");
            }    
        }

        [HttpPut(Name = "PutWeatherForecast")]
        public IActionResult Put(int id, WeatherForecast wf)
        {
            try
            {
                staticData.forecastList[id] = wf;
                return Ok(staticData.forecastList);
            }
            catch
            {
                return BadRequest("An error occured");
            }
        }

        [HttpPatch(Name = "PatchWeatherForecast")]
        public IActionResult Patch(int id, WeatherForecast wf)
        {
            try
            {
                foreach (PropertyInfo propertyInfo in wf.GetType().GetProperties())
                {
                    /*var b = wf.GetType().GetProperty(propertyInfo.Name);
                    var a = wf.GetType().GetProperty(propertyInfo.Name).GetValue(wf); debugging purpose*/
                    if (wf.GetType().GetProperty(propertyInfo.Name).GetValue(wf) != null && wf.GetType().GetProperty(propertyInfo.Name).CanWrite)
                    {
                        staticData.forecastList[id].GetType().GetProperty(propertyInfo.Name).SetValue(staticData.forecastList[id], wf.GetType().GetProperty(propertyInfo.Name).GetValue(wf));
                    }
                }
                return Ok(staticData.forecastList);
            }
            catch
            {
                return BadRequest("An error occured");
            }
        }

        [HttpDelete(Name = "PutWeatherForecast")]
        public IActionResult Delete(int id)
        {
            try
            {
                staticData.forecastList.RemoveAt(id);
                return Ok(staticData.forecastList);
            }
            catch
            {
                return BadRequest("An error occured");
            }
        }
    }
}