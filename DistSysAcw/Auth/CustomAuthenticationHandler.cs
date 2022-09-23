using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DistSysAcw.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DistSysAcw.Auth
{
    /// <summary>
    ///     Authenticates clients by API Key.
    ///     Inherits from AuthenticationHandler class with custom auth options, defined bellow.
    /// </summary>
    public class CustomAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationHandler
    {
        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            UserContext dbContext)
            : base(options, logger, encoder, clock)
        {
            DbContext = dbContext;
        }

        private Models.UserContext DbContext { get; }

        // Method that will either pass the authentication or cause it to fail.
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            #region 

            // If an ApiKey header does not exist.
            if (!Request.Headers.ContainsKey("ApiKey")) return Task.FromResult(AuthenticateResult.NoResult());

            var apiKey = Context.Request.Headers["ApiKey"];

            // If the ApiKey is not valid, the user does not exists in the DB.
            var user = UserDatabaseAccess.GetUser(DbContext, apiKey, null);
            if (user is null)
                // Fail authentication, causing HandleChallengeAsync to be called.
                return Task.FromResult(AuthenticateResult.Fail("ApiKey not valid."));

            // Create Claims.
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Add claims to ClaimsIdentity.
            var identity = new ClaimsIdentity(claims, "ApiKey");

            // ClaimsPrincipal created from the identity.
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Generate a new AuthenticationTicket from claimsPrincipal.
            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            // Return a Success AuthenticateResult.
            return Task.FromResult(AuthenticateResult.Success(ticket));

            #endregion
        }

        // Deal with 401 challenge concerns, when user fails authentication.
        // We don't challenge the user, just deny access.
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationhandler-1.handlechallengeasync?view=aspnetcore-5.0.
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var messagebytes = Encoding.ASCII.GetBytes("Unauthorized. Check ApiKey in Header is correct.");
            Context.Response.StatusCode = 401;
            Context.Response.ContentType = "application/json";
            await Context.Response.Body.WriteAsync(messagebytes, 0, messagebytes.Length);
        }
    }
}