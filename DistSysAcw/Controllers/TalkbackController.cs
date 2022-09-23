using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcw.Controllers
{
    public class TalkbackController : BaseController
    {
        
        /// Constructs a TalkBack controller, taking UserContext through dependency injection
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkbackController(Models.UserContext dbcontext) : base(dbcontext) { }
        #region TASK1
                                                                            // api/talkback/hello
        [ActionName ("Hello")] [HttpGet]
        public IActionResult Get()                                           // Use action result for one return type, Ok.
        {
            return Ok ("Hello World");                                       // Ok creates a new Ok Object Result(200).
        }
        #endregion
        #region TASK2
                                                                           // api/talkback/sort?integers=2&integers=5&integers=8
        [ActionName ("Sort")] [HttpGet]
        public IActionResult Get([FromQuery] string[] integers)              // Accepting string[] so I can handle bad request.
        {
            if (integers == null) return Ok(new int[0]);                    // If no values are submitted.
            try                                                             // If submitted values are valid integers.
            {
                var intArray = Array.ConvertAll (integers, int.Parse);
                Array.Sort (intArray);
                return Ok (intArray);
            }
            catch (Exception)                                              // catch values submitted that are invalid, like a char.
            {
                return BadRequest ("BadRequest");
            }
        }
        #endregion
    }
}
