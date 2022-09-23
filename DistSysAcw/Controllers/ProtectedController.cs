using DistSysAcw.Cryptography;
using DistSysAcw.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DistSysAcw.Cryptography.ShaCryptography;

namespace DistSysAcw.Controllers
{
    public class ProtectedController : BaseController
    {
        private readonly UserContext _dbContext; // Not to be changed by the controller.
        public ProtectedController(UserContext dbcontext) : base(dbcontext)// UserContext through dependency injection.
        {
            _dbContext = dbcontext;
        }

        #region 

        [HttpGet] [ActionName("Hello")]  [Authorize(Roles = "Admin, User")]  // api/protected/hello      
        public IActionResult Get([FromHeader] string apiKey)
        {
            var user = UserDatabaseAccess.GetUser(_dbContext, apiKey, null);
            UserDatabaseAccess.LogAction(_dbContext, user,
                $"{user.UserName} requested /protected/hello.");

            return Ok($"Hello {user.UserName}");
        }
       
        [HttpGet] [Authorize(Roles = "Admin, User")] [ActionName("SHA1")] // api/protected/sha1      
        public IActionResult GetSha1([FromHeader] string apiKey, [FromQuery] string message)
        {
            var user = UserDatabaseAccess.GetUser(_dbContext, apiKey, null);
            UserDatabaseAccess.LogAction(_dbContext, user,
                $"{user.UserName} requested /protected/sha1.");

            if (string.IsNullOrWhiteSpace(message)) 
                return BadRequest("Bad Request");
            return Ok(Sha1Encrypt(message));
        }
       
        [HttpGet] [ActionName("SHA256")]  [Authorize(Roles = "Admin, User")] // api/protected/sha256
        public IActionResult GetSha256([FromHeader] string apiKey, [FromQuery] string message)
        {
            var user = UserDatabaseAccess.GetUser(_dbContext, apiKey, null);
            UserDatabaseAccess.LogAction(_dbContext, user,
                $"{user.UserName} requested /protected/sha256.");

            if (string.IsNullOrWhiteSpace(message)) 
                return BadRequest("Bad Request");
            return Ok(Sha256Encrypt(message));
        }

        #endregion

        #region 
     
        [HttpGet] [ActionName("GetPublicKey")] [Authorize(Roles = "Admin, User")]   // api/protected/getpublickey
        public IActionResult GetPublicKey([FromHeader] string apiKey)
        {
            var user = UserDatabaseAccess.GetUser(_dbContext, apiKey, null);
            UserDatabaseAccess.LogAction(_dbContext, user,
                $"{user.UserName} requested /protected/getpublickey.");

            var rsa = RsaCryptography.GetRsaInstance();
            var publicKey = rsa.GetPublicKey();
            if (publicKey is null) 
                return BadRequest("Couldn’t Get the Public Key");
            return Ok(publicKey);
        }
        #endregion

        #region 

        [HttpGet] [Authorize(Roles = "Admin, User")] [ActionName("Sign")]  // api/protected/sign
        public IActionResult Sign([FromHeader] string apiKey, [FromQuery] string message)
        {
            var user = UserDatabaseAccess.GetUser(_dbContext, apiKey, null);
            UserDatabaseAccess.LogAction(_dbContext, user,
                $"{user.UserName} requested /protected/sign.");

            if (string.IsNullOrWhiteSpace(message)) 
                return BadRequest();
            var rsa = RsaCryptography.GetRsaInstance();
            var signed = rsa.Sign(message);
            if (signed is null) 
                return BadRequest();

            return Ok(signed);
        }
        #endregion

        #region 

        [HttpGet] [ActionName("AddFifty")]  [Authorize(Roles = "Admin")]     // api/protected/addfifty
        public IActionResult AddFifty([FromHeader] string apiKey, [FromQuery] string encryptedInteger,
            [FromQuery] string encryptedSymKey, [FromQuery] string encryptedIV)
        {
            var user = UserDatabaseAccess.GetUser(_dbContext, apiKey, null);
            UserDatabaseAccess.LogAction(_dbContext, user,
                $"{user.UserName} requested /protected/addfifty.");

            if (string.IsNullOrWhiteSpace(encryptedInteger) ||
                string.IsNullOrWhiteSpace(encryptedSymKey) ||
                string.IsNullOrWhiteSpace(encryptedIV))
                return BadRequest();

            var rsa = RsaCryptography.GetRsaInstance();
            var addFifty = rsa.AddFifty(encryptedInteger, encryptedSymKey, encryptedIV);
            if (addFifty is null) 
                return BadRequest();

            return Ok(addFifty);
        }
        #endregion
    }
}