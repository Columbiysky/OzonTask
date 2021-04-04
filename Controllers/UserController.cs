using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OzonTask.Models;

namespace OzonTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UsersContext userContext;
        public UserController (UsersContext u)
        {
            userContext = u;
        }

        // api/user/c.7843543@gmail.com
        [HttpGet("{Email}")]
        public ActionResult<User> GetUserByEmail(string Email)
        {
            int count = userContext.GetUser(Email);
            return new ObjectResult(count);
        }

        [HttpPost]
        public ActionResult<User> PostUser(User user)
        {
            if (user == null)
                return BadRequest();
            userContext.AddUser(user);
            return Ok();
        }
    }
}
