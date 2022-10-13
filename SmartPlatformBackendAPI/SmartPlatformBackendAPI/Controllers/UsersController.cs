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
        [HttpGet("{id}")]
        public IActionResult Get(Guid userID)
        {
            User? user = dbContext.Users.Find(userID);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // GET api/<UsersController>/5
        [HttpGet("{credentials}")]
        public IActionResult GetFromUserNameAndPassword(UserCredentials credentials)
        {
            User? user = dbContext.Users.Find(credentials.UserName);
            if (user == null)
                return NotFound();

            else if (user.Password != credentials.Password)
                return BadRequest();

            return Ok(user);
        }

        // POST api/<UsersController>
        [HttpPost]
        public IActionResult Post([FromBody] AddNewUserModel newUser)
        {
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
