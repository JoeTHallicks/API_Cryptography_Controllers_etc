using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DistSysAcw.Controllers
{
    [Route("api/[Controller]/[Action]")][ApiController]
    public abstract class BaseController : ControllerBase
    {  
        protected Models.UserContext DbContext { get; set; }
        public BaseController(Models.UserContext dbcontext)
        {
            DbContext = dbcontext;
        }
    }
}