using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPlatformBackendAPI.Data;
using SmartPlatformBackendAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartPlatformBackendAPI.Controllers
{
    [Route("api/{userID}/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly SmartPlatformAPIDbContext dbContext;

        public DevicesController(SmartPlatformAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        // GET api/{userID}/Devices/getRequestedState?guid={deviceID}&key={deviceKey}"
        [HttpGet("getRequestedState")]
        public IActionResult Get([FromRoute] Guid userID, Guid deviceID, string deviceKey)
        {
            User? user = dbContext.Users.Find(userID);
            if (user == null || user.Devices == null)
                return NotFound();

            foreach (Device device in user.Devices)
            {
                if (device.DeviceId == deviceID && device.DeviceKey == deviceKey)
                    return Ok(device);
            }
            return NotFound();
        }

        // POST api/<DevicesController>
        [HttpPost]
        public void Post([FromRoute] Guid userID, [FromBody] Device device)
        {


        }

        // PUT api/<DevicesController>/5
        [HttpPut("{deviceID}")]
        public IActionResult Put([FromRoute] Guid userID, Guid deviceID, [FromBody] User user)
        {
            return Ok();
        }

        // DELETE api/{userID}/Devices/{deviceID}"
        [HttpDelete("{deviceID}")]
        public IActionResult Delete([FromRoute] Guid userID, Guid deviceID)
        {
            User? user = dbContext.Users.Find(userID);
            if (user == null || user.Devices == null)
                return NotFound();

            foreach (Device device in user.Devices)
            {
                if (device.DeviceId == deviceID)
                {
                    user.Devices.Remove(device);
                    return Ok();
                }
            }
            return NotFound();
        }
    }
}
