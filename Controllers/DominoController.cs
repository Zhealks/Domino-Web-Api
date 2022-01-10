using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DominoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DominoController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DominoController> _logger;

        public DominoController(ILogger<DominoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public string SendNotes([FromBody] string parms)
        {
            dynamic dynParms = JsonConvert.DeserializeObject(Convert.ToString(parms));   
            string receivers = dynParms.receivers;
            string topic = dynParms.topic;
            string body = dynParms.body;
            string copies = dynParms.copies;
            string name = dynParms.name;
            string nsf = dynParms.nsf;
            string passwd = dynParms.passwd;

            Domino domino = new Domino();
            string res = domino.SendNotes(receivers, topic, body, copies, name, nsf, passwd);
            _logger.LogInformation("[USER] res: " + res);

            return res;
        }
    }
}
