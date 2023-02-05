using Microsoft.AspNetCore.Cors.Infrastructure;
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

    public interface IDataOperations
    {
        public List<WeatherForecast> GetOrdered();
        public List<WeatherForecast> GetAll();
        public List<WeatherForecast> Post([FromQuery] WeatherForecast wf);
        public List<WeatherForecast> Put(int id, WeatherForecast wf);
        public List<WeatherForecast> Patch(int id, WeatherForecast wf);
        public List<WeatherForecast> Delete(int id);
    }

    public class DataOperations : IDataOperations
    {
        public List<WeatherForecast> GetOrdered()
        {
            return staticData.forecastList.OrderBy(t => t.TemperatureC).ToList();
        }

        public List<WeatherForecast> GetAll()
        {
            return staticData.forecastList;
        }

        public List<WeatherForecast> Post([FromQuery] WeatherForecast wf)
        {
            try
            {
                staticData.forecastList.Add(wf);
                return staticData.forecastList;
            }
            catch
            {
                return null; //Normall DTO object contains a field related with error handling.
            }
        }

        public List<WeatherForecast> Put(int id, WeatherForecast wf)
        {
            try
            {
                staticData.forecastList.Add(wf);
                return staticData.forecastList;
            }
            catch
            {
                return null; //Normall DTO object contains a field related with error handling.
            }
        }

        public List<WeatherForecast> Patch(int id, WeatherForecast wf)
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
                return staticData.forecastList;
            }
            catch
            {
                return null;
            }
        }

        public List<WeatherForecast> Delete(int id)
        {
            try
            {
                staticData.forecastList.RemoveAt(id);
                return staticData.forecastList;
            }
            catch
            {
                return null;
            }
        }
    }

    public static class DependencyExtension
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddTransient<IDataOperations, DataOperations>();
        }
    }

    public class Logger 
    {
        RequestDelegate requestDelegate;

        public Logger(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            finally
            {
                Console.WriteLine(context.Request?.Method + context.Request?.Path.Value + context.Response?.StatusCode);
            }
        }
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

        private readonly IDataOperations _dataOperations;

        public WeatherForecastController(IDataOperations dataOperations)
        {
            _dataOperations = dataOperations;
        }

        [HttpGet("GetWeatherForecastOrdered")]
        public IActionResult GetOrdered()
        {
            List<WeatherForecast> ret;
            try
            {
                //In a complete solution, <weather forecast> type should be converted to DTO via mapper.
                ret = _dataOperations.GetOrdered();
                if (ret.Count == 0)
                {
                    return NoContent();
                }
            }
            catch
            {
                return BadRequest("An error occured");
            }

            return Ok(ret);
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult GetAll()
        {
            List<WeatherForecast> ret;
            try
            {
                ret = _dataOperations.GetAll();
                if (ret.Count == 0)
                {
                    return NoContent();
                }
            }
            catch
            {
                return BadRequest("An error occured");
            }
            return Ok(ret);
        }

        [HttpPost(Name = "PostWeatherForecast")]
        public IActionResult Post([FromQuery] WeatherForecast wf)
        {
            List<WeatherForecast> ret = _dataOperations.Post(wf);

            if (ret == null)
            {
                return BadRequest("An error occured");
            }
            else
            {
                return Ok(ret);
            }
        }

        [HttpPut(Name = "PutWeatherForecast")]
        public IActionResult Put(int id, WeatherForecast wf)
        {
            List<WeatherForecast> ret = _dataOperations.Put(id, wf);
            if (ret == null)
            {
                return BadRequest("An error occured");
            }
            else
            {
                return Ok(ret);
            }
        }

        [HttpPatch(Name = "PatchWeatherForecast")]
        public IActionResult Patch(int id, WeatherForecast wf)
        {
            List<WeatherForecast> ret = _dataOperations.Patch(id, wf);
            if (ret == null)
            {
                return BadRequest("An error occured");
            }
            else
            {
                return Ok(ret);
            }
        }

        [HttpDelete(Name = "PutWeatherForecast")]
        public IActionResult Delete(int id)
        {
            List<WeatherForecast> ret = _dataOperations.Delete(id);
            if (ret == null)
            {
                return BadRequest("An error occured");
            }
            else
            {
                return Ok(ret);
            }
        }
    }
}
