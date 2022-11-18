using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPlatformBackendAPI.Data;
using SmartPlatformBackendAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartPlatformBackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SmartPlatformAPIDbContext dbContext;

        public UsersController(SmartPlatformAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public IActionResult Get()
        {
            //return Ok("Hello Anton!");
            return Ok(dbContext.Users.Include(d => d.DeviceDescriptors).ToList());
        }

        // GET api/<UsersController>/5
        [HttpGet("{userID}")]
        public IActionResult Get(Guid userID)
        {
            User? user = dbContext.Users.Find(userID);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // Old login
        // GET api/<UsersController>/loginAttempt?username={first%20last}&pass={password}
        [HttpGet("loginAttempt")]
        public IActionResult GetFromUserNameAndPassword(string username, string pass)
        {
            User? user = dbContext.Users.Include(d => d.DeviceDescriptors).AsNoTracking().SingleOrDefault(u => u.UserName == username && u.Password == pass);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        // New login
        [HttpPost("login")]
        public IActionResult PostLogin(UserCredentials userCredentials)
        {
            User? user = dbContext.Users.Include(d => d.DeviceDescriptors).AsNoTracking().SingleOrDefault(u => u.UserName == userCredentials.UserName && u.Password == userCredentials.Password);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        // Updated with credential checking
        // POST api/<UsersController>
        [HttpPost] 
        public IActionResult AddDeviceToUser([FromBody] DeviceDecriptor newDevice)
        {
            try
            {
                User? user = dbContext.Users.Include(d => d.DeviceDescriptors).Single(u => u.UserID == newDevice.UserID);

                if (user != null)
                {
                    user.DeviceDescriptors.Add(newDevice);
                    dbContext.DeviceDecriptors.Add(newDevice);
                    dbContext.SaveChanges();
                    return Ok(user);
                }
                else
                    return NotFound();
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        // PUT api/<UsersController>
        [HttpPut]
        public IActionResult CreateNewUser([FromBody] AddNewUserModel newUser)
        {
            newUser.UserName = newUser.UserName.Trim();

            if (dbContext.Users.Count() != 0 && dbContext.Users.Single(u => u.UserName == newUser.UserName) != null)
            {
                return Conflict();
            }

            User user = new User()
            {
                UserID = Guid.NewGuid(),
                UserName = newUser.UserName,
                Password = newUser.Password,
                DeviceDescriptors = new List<DeviceDecriptor>()
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return Ok(user);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{userID}")]
        public IActionResult Delete([FromRoute] Guid userID, [FromBody] UserCredentials userCredentials)
        {
            try
            {
                User user = dbContext.Users.Single(u => u.UserID == userID && u.UserName == userCredentials.UserName && u.Password == userCredentials.Password);
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                return Unauthorized();
            }            
        }

        // DELETE api/{userID}/Devices/{deviceID}"
        [HttpDelete("{userID}/{deviceID}")]
        public IActionResult Delete([FromRoute] Guid userID, [FromRoute] Guid deviceID)
        {
            try
            {
                User? user = dbContext.Users.Include(d => d.DeviceDescriptors).Single(u => u.UserID == userID);

                if (user != null || user.DeviceDescriptors != null)
                {
                    foreach (DeviceDecriptor device in user.DeviceDescriptors)
                    {
                        if (device.DeviceID == deviceID)
                        {
                            user.DeviceDescriptors.Remove(device);
                            dbContext.SaveChanges();
                            return Ok(user);
                        }
                    }
                }
              
                return NotFound();
            }
            catch (Exception)
            {
                return Unauthorized();
            }            
        }
    }
}
