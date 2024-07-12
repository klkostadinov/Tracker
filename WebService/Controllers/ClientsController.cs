using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Flurl.Http;
using Newtonsoft.Json;
using WebService.Models;

namespace WebService.Controllers
{

    public class ClientsController : ApiController
    {
        private string token = Guid.NewGuid().ToString("N");


        [Route("api/clients/subscribe")]
        public IHttpActionResult Get(string date, string callback)
        {
            IEnumerable<string> headerValues;
            if (!Request.Headers.TryGetValues("Accept-Client", out headerValues) || headerValues.FirstOrDefault() != "Fourth-Monitor")
                return Unauthorized();

            var newClient = new Client()
            {
                Url = new Uri(callback),
                Token = token
            };

            new Simulator(newClient).Simulate(DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.CurrentCulture));
            return Ok(new { newClient.Token, Expires = DateTime.UtcNow.AddHours(8).ToString("u").Replace(" ","T") });
        }
        

    }

    public class Simulator
    {
        private readonly IList<JsonEmployee> _employees;

        private readonly Client _client;

        public Simulator(Client client)
        {
            _client = client;
            _employees = JsonConvert.DeserializeObject<IList<JsonEmployee>>(new StreamReader(HttpContext.Current.Server.MapPath("~/bin/Data/employees.json")).ReadToEnd());
        }

        public void Simulate(DateTime when)
        {
            Task.Factory.StartNew(async () =>
            {
                bool stop = false;
                while (!stop)
                {
                    var random = new Random(Environment.TickCount);
                    Thread.Sleep(1000 * random.Next(1, 50)); //Wait between 1 and 50 secs

                    var data = SimulateData(when);
                    if (!data.Any())
                    {
                        //Finished simulating today's data
                        stop = true;
                    }
                    int count = 10;
                    while (count > 0)
                        try
                        {
                            await _client.Url.ToString().WithHeader("X-Fourth-Token", _client.Token).PostJsonAsync(data);
                            count = 0;
                        }
                        catch (Exception ex)
                        {
                            --count;
                            Thread.Sleep(1000);
                        }
                }
            });
        }

        readonly Dictionary<int, JsonEmployee> _simulated = new Dictionary<int, JsonEmployee>();
        readonly object _locker = new object();

        private IList<SimulationData> SimulateData(DateTime when)
        {
            //So we dont overlap requests
            lock (_locker)
            {
                var random = new Random(Environment.TickCount);
                var count = random.Next(5, 101); //from 5 to 100 employees
                var employees =
                    _employees.Where(x => !_simulated.ContainsKey(x.Id))
                        .OrderBy(x => Guid.NewGuid())
                        .Take(count)
                        .ToList();
                foreach (var e in employees)
                {
                    _simulated.Add(e.Id, e);
                }
                return employees.Select(x =>
                    new SimulationData
                    {
                        EmployeeId = x.Id,
                        When = when.AddHours(8).AddMinutes(random.Next(120)).ToString("u").Replace(" ", "T"),
                    }).ToList();
            }
        }
    }

    public class JsonEmployee
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Role { get; set; }
        public int? ManagerId { get; set; }
        public int Id { get; set; }
        public List<string> Teams { get; set; }
    }

    public class SimulationData
    {
        public int EmployeeId { get; set; }
        public string When { get; set; }
    }
}
