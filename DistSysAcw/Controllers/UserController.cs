using System;
using DistSysAcw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DistSysAcw.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserContext _dbContext;     // Not changed by the controller.

       
        public UserController(UserContext dbcontext) : base(dbcontext)  // Pass in UserContext through dependency injection.
        {
            _dbContext = dbcontext;
        }

        #region 
      
        [HttpGet] [ActionName("New")]     
        public ActionResult Get([FromQuery] string username) // api/user/new?username=UserOne
        {
            return Ok(UserDatabaseAccess.UserNameExists(_dbContext, username)
                ? "True - User Does Exist! Did you mean to do a POST to create a new user?"
                : "False - User Does Not Exist! Did you mean to do a POST to create a new user?");
        }

      
        [HttpPost]  [ActionName("New")]  // api/user/new     
        public ActionResult Post([FromBody] string jsonUsername)
        {
            if (string.IsNullOrWhiteSpace(jsonUsername))
                return BadRequest(
                    "Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            if (UserDatabaseAccess.UserNameExists(_dbContext, jsonUsername)) // If the username is already taken.
                return StatusCode(403, "Oops. This username is already in use. Please try again with a new username."); 
            var newUser = UserDatabaseAccess.PostUser(_dbContext, jsonUsername);
            return Ok(newUser.ApiKey); // Create a new user and return the ApiKey to the client.
        }
        #endregion

        #region 

        [HttpDelete] [ActionName("RemoveUser")] // api/user/removeuser?username=UserOne
        [Authorize(Roles = "Admin, User")]   // Authenticate responder CustomAuthenticationHandler.
        public ActionResult Delete([FromHeader] string apiKey, [FromQuery] string username) // Verify the authentic user is authorised.
        {
            var user = UserDatabaseAccess.GetUser(_dbContext, apiKey, null);
            UserDatabaseAccess.LogAction(_dbContext, user,
                $"/user/removeuser called for {user.UserName}.");
            if (UserDatabaseAccess.DeleteUser(_dbContext, apiKey, username))
            {
                return Ok("True");
            }
            return Ok("False");
        }
        #endregion

        #region 

        [ActionName("ChangeRole")] [Authorize(Roles = "Admin")]  // api/user/changerole                            
        public ActionResult ChangeRole([FromHeader] string apiKey, [FromBody] ChangeRole jsonRequest)
        {
            var user = UserDatabaseAccess.GetUser(_dbContext, apiKey, null);
            UserDatabaseAccess.LogAction(_dbContext, user,
                $"/user/changerole called for {jsonRequest.Username} by {user.UserName}");
            ChangeRole jsonChangeRole;
            try
            {
                jsonChangeRole = jsonRequest;
            }
            catch (Exception)
            {
                return BadRequest("NOT DONE: An error occurred");
            } 
            if (!UserDatabaseAccess.UserNameExists(_dbContext, jsonChangeRole.Username))  // If username not exist.
                return BadRequest("NOT DONE: Username does not exist");
            if (!Enum.IsDefined(typeof(User.Roles), jsonChangeRole.Role)) // If role not exist.
                return BadRequest("NOT DONE: Role does not exist");
            if (UserDatabaseAccess.ChangeUserRole(_dbContext, jsonChangeRole)) return Ok("DONE");
            return BadRequest("NOT DONE: An error occurred");
        }
        #endregion
    }
}