using Microsoft.AspNetCore.Mvc;
using SmartPlatformBackendAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartPlatformBackendAPI.Controllers
{
    [Route("api/{userID}/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        // GET: api/<DevicesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DevicesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DevicesController>
        [HttpPost]
        public void Post([FromBody] User user)
        {
        }

        // PUT api/<DevicesController>/5
        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] Guid userID, int id, [FromBody] User user)
        {
            return Ok();
        }

        // DELETE api/<DevicesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
