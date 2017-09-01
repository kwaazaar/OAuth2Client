using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace SecuredApi.Controllers
{
    // GEEN Authorize-attribuut: niet deze service, maar de API-Management schil is beveiligd!
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static ConcurrentBag<string> _values = new ConcurrentBag<string>();

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _values.ToArray();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return id < _values.Count ? _values.Skip(id).FirstOrDefault() : null;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            _values.Add(value);
        }

        // DELETE api/value
        [HttpDelete]
        public void DeleteLast()
        {
            _values.TryTake(out _);
        }
    }
}
