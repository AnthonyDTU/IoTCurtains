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

        // POST api/<UsersController>
        [HttpPost]
        public IActionResult AddDeviceToUser([FromBody] DeviceDecriptor newDevice)
        {
            User? userToUpdate = dbContext.Users.Include(d => d.DeviceDescriptors).Single(u => u.UserID == newDevice.UserID);

            if (userToUpdate != null)
            {
                userToUpdate.DeviceDescriptors.Add(newDevice);
                dbContext.DeviceDecriptors.Add(newDevice);
                dbContext.SaveChanges();
                return Ok(userToUpdate);
            }
            else
                return NotFound();
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
        public void Delete(Guid userID)
        {
            User user = dbContext.Users.Find(userID);
            dbContext.Users.Remove(user);
            dbContext.SaveChanges();
        }
    }
}
