using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace DistSysAcw.Auth
{ 
    public class CustomAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationHandler
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        public CustomAuthorizationHandler(IHttpContextAccessor httpContext) { HttpContextAccessor = httpContext;}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            #region 
            //Authorises clients by role.
            #endregion
            // If the users identity has not been authenticated (from CustomAuthenticationHandler).
            if (context.User == null || !context.User.Identity.IsAuthenticated) return Task.CompletedTask;

            //  required role to perform the action asked for.
            if (requirement.AllowedRoles.Any(role => context.User.IsInRole(role)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Fail();
            HttpContextAccessor.HttpContext.Response.StatusCode = 403;
            HttpContextAccessor.HttpContext.Response.WriteAsync("Forbidden. Admin access only.");

            return Task.CompletedTask;

        }
    }
}