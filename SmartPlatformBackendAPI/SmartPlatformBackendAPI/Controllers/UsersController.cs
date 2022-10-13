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
            return Ok(dbContext.Users);
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
            User? user = dbContext.Users.Find(username);
            if (user == null)
                return NotFound();

            else if (user.Password != pass)
                return BadRequest();

            return Ok(user);
        }

        // POST api/<UsersController>
        [HttpPost]
        public IActionResult CreateNewUser([FromBody] AddNewUserModel newUser)
        {
            if (dbContext.Users.Find(newUser.UserName) != null)
            {
                return Conflict();
            }

            User user = new User()
            {
                UserID = Guid.NewGuid(),
                UserName = newUser.UserName,
                Password = newUser.Password,
                Devices = new List<Device>(),
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return Ok(user);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{userName}")]
        public IActionResult Put(string userName, [FromBody] string value)
        {
            return Ok();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
